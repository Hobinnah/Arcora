using System.Net;
using System.Text.RegularExpressions;
using AutoMapper;
using Arcora.Api.Configurations;
using Arcora.Api.DTOs;
using Arcora.Api.Entities;
using Arcora.Api.Enums;
using Arcora.Api.Models;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Services.Implementations;

public class LeaseContractTemplateService : ILeaseContractTemplateService
{
    private static readonly string TemplatesRoot = Path.Combine(AppContext.BaseDirectory, "Templates");
    private const string StandardTemplateFileName = "StandardResidentialLease.html";
    private static readonly HashSet<int> NonEditableSections = new() { 1, 2, 3, 4, 5, 31, 32, 33, 34 };

    private readonly IMapper mapper;
    private readonly IMemoryCache cache;
    private readonly ILogger<LeaseContractTemplateService> logger;
    private readonly ILeaseContractTemplateRepository templateRepository;
    private readonly ILeaseRepository leaseRepository;
    private readonly IFileStorageService fileStorageService;
    private readonly IOptions<CacheConfiguration> options;

    public LeaseContractTemplateService(
        IMapper mapper,
        IMemoryCache cache,
        IOptions<CacheConfiguration> options,
        ILogger<LeaseContractTemplateService> logger,
        ILeaseContractTemplateRepository templateRepository,
        ILeaseRepository leaseRepository,
        IFileStorageService fileStorageService)
    {
        this.mapper = mapper;
        this.cache = cache;
        this.logger = logger;
        this.templateRepository = templateRepository;
        this.leaseRepository = leaseRepository;
        this.fileStorageService = fileStorageService;
        this.options = options;

        if (this.options.Value.ExpirationTimeInMinutes <= 0)
            this.options.Value.ExpirationTimeInMinutes = 15;
    }

    public async Task<PagedResult<LeaseContractTemplateDto>> GetByOrganizationID(Guid organizationID, Paging paging)
    {
        paging ??= new Paging();
        var cacheKey = GetCacheKey(organizationID);
        IEnumerable<LeaseContractTemplate> entities;

        try
        {
            entities = cache.Get<IEnumerable<LeaseContractTemplate>>(cacheKey) ?? Enumerable.Empty<LeaseContractTemplate>();
            if (!entities.Any())
            {
                entities = await templateRepository.GetByOrganizationIDAsync(organizationID);
                if (!entities.Any())
                {
                    await EnsureDefaultTemplateForOrganizationAsync(organizationID);
                    entities = await templateRepository.GetByOrganizationIDAsync(organizationID);
                }

                if (entities.Any())
                    cache.Set(cacheKey, entities, DateTime.UtcNow.AddMinutes(options.Value.ExpirationTimeInMinutes));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch lease contract templates for organization {OrganizationID}.", organizationID);
            return new PagedResult<LeaseContractTemplateDto> { Data = new List<LeaseContractTemplateDto>(), TotalCount = 0 };
        }

        var filtered = entities;
        if (!string.IsNullOrWhiteSpace(paging?.Search))
        {
            filtered = filtered.Where(x =>
                (!string.IsNullOrWhiteSpace(x.TemplateName) && x.TemplateName.Contains(paging.Search, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(x.Description) && x.Description.Contains(paging.Search, StringComparison.OrdinalIgnoreCase)));
        }

        var totalCount = filtered.Count();
        var paged = filtered
            .Skip((paging.PageNumber - 1) * paging.PageSize)
            .Take(paging.PageSize)
            .ToList();

        var dtos = mapper.Map<IEnumerable<LeaseContractTemplateDto>>(paged);
        return new PagedResult<LeaseContractTemplateDto>
        {
            Data = dtos,
            TotalCount = totalCount
        };
    }

    public async Task<LeaseContractTemplateDto?> GetByID(Guid templateID)
    {
        var entity = await templateRepository.GetByTemplateIDAsync(templateID);
        return entity == null ? null : mapper.Map<LeaseContractTemplateDto>(entity);
    }

    public async Task<IEnumerable<LeaseContractTemplateVersionDto>> GetVersions(Guid templateID)
    {
        var versions = await templateRepository.GetVersionsAsync(templateID);
        return mapper.Map<IEnumerable<LeaseContractTemplateVersionDto>>(versions);
    }

    public async Task<LeaseContractTemplateDto> CreateTemplate(LeaseContractTemplateUpsertDto request)
    {
        var sanitizedHtml = SanitizeHtml(request.HtmlContent ?? string.Empty);
        var standardHtml = await GetStandardTemplateHtml();
        var changedSections = GetChangedSections(standardHtml, sanitizedHtml);
        EnsureNoRestrictedSectionsChanged(changedSections);
        EnsureDeclaredSectionsMatchChanges(request.EditedSectionNumbers, changedSections);
        EnsureNoChangesOutsideSections(standardHtml, sanitizedHtml);

        var existing = await templateRepository.GetByOrganizationIDAsync(request.OrganizationID);
        var shouldBeDefault = request.IsDefault || !existing.Any();

        if (shouldBeDefault)
            await templateRepository.ClearDefaultFlagsAsync(request.OrganizationID);

        var entity = new LeaseContractTemplate
        {
            LeaseContractTemplateID = Guid.NewGuid(),
            OrganizationID = request.OrganizationID,
            TemplateName = request.TemplateName,
            Description = request.Description,
            HtmlContent = sanitizedHtml,
            IsDefault = shouldBeDefault,
            IsActive = request.IsActive,
            IsSystemGenerated = false,
            CapturedBy = request.UpdatedBy,
            CapturedDate = DateTime.UtcNow
        };

        await templateRepository.Create(entity);

        await templateRepository.AddVersionAsync(new LeaseContractTemplateVersion
        {
            LeaseContractTemplateVersionID = Guid.NewGuid(),
            LeaseContractTemplateID = entity.LeaseContractTemplateID,
            VersionNumber = 1,
            HtmlContent = sanitizedHtml,
            ChangeSummary = string.IsNullOrWhiteSpace(request.ChangeSummary) ? "Initial version" : request.ChangeSummary,
            CapturedBy = request.UpdatedBy,
            CapturedDate = DateTime.UtcNow
        });

        await templateRepository.Save();
        InvalidateCache(request.OrganizationID);

        var hydrated = await templateRepository.GetByTemplateIDAsync(entity.LeaseContractTemplateID) ?? entity;
        return mapper.Map<LeaseContractTemplateDto>(hydrated);
    }

    public async Task<LeaseContractTemplateDto?> UpdateTemplate(Guid templateID, LeaseContractTemplateUpsertDto request)
    {
        var existing = await templateRepository.GetByTemplateIDAsync(templateID);
        if (existing == null)
            return null;

        var sanitizedHtml = SanitizeHtml(request.HtmlContent ?? string.Empty);
        var changedSections = GetChangedSections(existing.HtmlContent ?? string.Empty, sanitizedHtml);
        EnsureNoRestrictedSectionsChanged(changedSections);
        EnsureDeclaredSectionsMatchChanges(request.EditedSectionNumbers, changedSections);
        EnsureNoChangesOutsideSections(existing.HtmlContent ?? string.Empty, sanitizedHtml);

        if (request.IsDefault)
            await templateRepository.ClearDefaultFlagsAsync(existing.OrganizationID, existing.LeaseContractTemplateID);

        existing.TemplateName = request.TemplateName;
        existing.Description = request.Description;
        existing.HtmlContent = sanitizedHtml;
        existing.IsActive = request.IsActive;
        existing.IsDefault = request.IsDefault || existing.IsDefault;
        existing.UpdatedBy = request.UpdatedBy;
        existing.UpdatedDate = DateTime.UtcNow;

        var versionNumber = (await templateRepository.GetLatestVersionNumberAsync(existing.LeaseContractTemplateID)) + 1;
        await templateRepository.AddVersionAsync(new LeaseContractTemplateVersion
        {
            LeaseContractTemplateVersionID = Guid.NewGuid(),
            LeaseContractTemplateID = existing.LeaseContractTemplateID,
            VersionNumber = versionNumber,
            HtmlContent = sanitizedHtml,
            ChangeSummary = string.IsNullOrWhiteSpace(request.ChangeSummary) ? "Template updated" : request.ChangeSummary,
            CapturedBy = request.UpdatedBy,
            CapturedDate = DateTime.UtcNow
        });

        await templateRepository.Update(existing);
        await templateRepository.Save();
        InvalidateCache(existing.OrganizationID);

        var hydrated = await templateRepository.GetByTemplateIDAsync(templateID) ?? existing;
        return mapper.Map<LeaseContractTemplateDto>(hydrated);
    }

    public async Task<LeaseContractTemplateDto?> SetDefaultTemplate(Guid templateID, string? updatedBy)
    {
        var existing = await templateRepository.GetByTemplateIDAsync(templateID);
        if (existing == null)
            return null;

        await templateRepository.ClearDefaultFlagsAsync(existing.OrganizationID, existing.LeaseContractTemplateID);
        existing.IsDefault = true;
        existing.UpdatedBy = updatedBy;
        existing.UpdatedDate = DateTime.UtcNow;

        await templateRepository.Update(existing);
        await templateRepository.Save();
        InvalidateCache(existing.OrganizationID);

        var hydrated = await templateRepository.GetByTemplateIDAsync(templateID) ?? existing;
        return mapper.Map<LeaseContractTemplateDto>(hydrated);
    }

    public async Task<LeaseContractRenderResultDto?> RenderContract(LeaseContractRenderRequestDto request)
    {
        var lease = await leaseRepository.GetLeaseWithContractContextByIDAsync(request.LeaseID);
        if (lease == null)
            return null;

        LeaseContractTemplate? selectedTemplate = null;
        var isFallbackTemplate = false;

        if (request.LeaseContractTemplateID.HasValue && request.LeaseContractTemplateID.Value != Guid.Empty)
            selectedTemplate = await templateRepository.GetByTemplateIDAsync(request.LeaseContractTemplateID.Value);

        selectedTemplate ??= await templateRepository.GetDefaultByOrganizationIDAsync(lease.OrganizationID);
        selectedTemplate ??= await EnsureDefaultTemplateForOrganizationAsync(lease.OrganizationID);

        string rawHtml;
        string? templateName;
        Guid? templateID;

        if (selectedTemplate == null)
        {
            rawHtml = await GetStandardTemplateHtml();
            templateName = "Standard Residential Lease";
            templateID = null;
            isFallbackTemplate = true;
        }
        else
        {
            rawHtml = selectedTemplate.HtmlContent ?? string.Empty;
            templateName = selectedTemplate.TemplateName;
            templateID = selectedTemplate.LeaseContractTemplateID;
        }

        var brandLogoUrl = await ResolveBrandLogoUrlAsync(lease.Organization?.BrandLogoUrl);
        var rendered = RenderHtml(rawHtml, lease, brandLogoUrl);

        return new LeaseContractRenderResultDto
        {
            LeaseContractTemplateID = templateID,
            TemplateName = templateName,
            IsFallbackTemplate = isFallbackTemplate,
            RawHtml = rawHtml,
            RenderedHtml = rendered
        };
    }

    public async Task<string> GetStandardTemplateHtml()
    {
        var path = Path.Combine(TemplatesRoot, StandardTemplateFileName);
        if (File.Exists(path))
            return await File.ReadAllTextAsync(path);

        return "<html><body><h1>Standard Residential Lease</h1><p>{{Organization.DisplayName}}</p></body></html>";
    }

    private string RenderHtml(string html, Lease lease, string? brandLogoUrl)
    {
        var organization = lease.Organization;
        var listing = lease.Listing;
        var unit = lease.RentalUnit;
        var property = unit?.Property;
        var address = property?.Address;
        var tenant = lease.Tenant;
        var user = tenant?.User;

        var tenantFullName = string.Join(" ", new[] { user?.FirstName, user?.LastName }.Where(x => !string.IsNullOrWhiteSpace(x))).Trim();
        if (string.IsNullOrWhiteSpace(tenantFullName))
            tenantFullName = string.IsNullOrWhiteSpace(user?.DisplayName) ? "Tenant" : user!.DisplayName;

        var addressLine2 = string.IsNullOrWhiteSpace(address?.Line2) ? string.Empty : $", {address.Line2}";
        var fullAddress = address == null
            ? ""
            : $"{address.Line1}{addressLine2}, {address.City}, {address.ProvinceCode} {address.PostalCode}, {address.CountryCode}";

        var effectiveLeaseTermMonths = lease.LeaseTermMonths ?? GetLeaseTermMonthsFromDates(lease.StartDate, lease.EndDate);
        var utilityResponsibility = effectiveLeaseTermMonths < 12
            ? "Because the lease term is less than one (1) year, utilities shall be handled by the Landlord, subject to applicable law and metering limitations."
            : "Because the lease term is one (1) year or longer, utilities shall be handled by the Tenant, subject to applicable law and service transfer availability.";

        var tokens = new Dictionary<string, string?>
        {
            ["Generated.On"] = DateTime.UtcNow.ToString("yyyy-MM-dd"),

            ["Organization.DisplayName"] = organization?.DisplayName,
            ["Organization.LegalName"] = organization?.LegalName,
            ["Organization.BusinessNumber"] = organization?.BusinessNumber,
            ["Organization.CountryCode"] = organization?.CountryCode,
            ["Organization.ProvinceCode"] = organization?.ProvinceCode,
            ["Organization.BrandLogoUrl"] = brandLogoUrl,

            ["Listing.Title"] = listing?.Title,
            ["Listing.Description"] = listing?.Description,
            ["Listing.Currency"] = listing?.Currency,
            ["Listing.BaseMonthlyRentAmount"] = listing?.BaseMonthlyRentAmount.ToString("N2"),
            ["Listing.SecurityDepositAmount"] = listing?.SecurityDepositAmount.ToString("N2"),
            ["Listing.IsPetFriendly"] = listing?.IsPetFriendly == true ? "Yes" : "No",

            ["Address.Line1"] = address?.Line1,
            ["Address.Line2"] = address?.Line2,
            ["Address.City"] = address?.City,
            ["Address.ProvinceCode"] = address?.ProvinceCode,
            ["Address.PostalCode"] = address?.PostalCode,
            ["Address.CountryCode"] = address?.CountryCode,
            ["Address.Full"] = fullAddress,

            ["RentalUnit.Name"] = unit?.Name,
            ["RentalUnit.UnitNumber"] = unit?.UnitNumber,

            ["Tenant.FullName"] = tenantFullName,
            ["Tenant.FirstName"] = user?.FirstName,
            ["Tenant.LastName"] = user?.LastName,
            ["Tenant.Email"] = user?.Email,
            ["Tenant.PhoneNumber"] = tenant?.PhoneNumber,

            ["Lease.LeaseCode"] = lease.LeaseCode,
            ["Lease.LeaseNumber"] = lease.LeaseNumber,
            ["Lease.StartDate"] = lease.StartDate.ToString("yyyy-MM-dd"),
            ["Lease.EndDate"] = lease.EndDate?.ToString("yyyy-MM-dd"),
            ["Lease.LeaseTermMonths"] = lease.LeaseTermMonths?.ToString(),
            ["Lease.BaseRentAmount"] = lease.BaseRentAmount.ToString("N2"),
            ["Lease.Currency"] = lease.Currency,
            ["Lease.GracePeriodDays"] = lease.GracePeriodDays.ToString(),
            ["Lease.LateFeeFixedAmount"] = lease.LateFeeFixedAmount.ToString("N2"),
            ["Lease.LateFeePercentage"] = lease.LateFeePercentage.ToString("N2"),

            ["Utilities.Responsibility"] = utilityResponsibility
        };

        var output = html;
        foreach (var token in tokens)
        {
            output = output.Replace($"{{{{{token.Key}}}}}", WebUtility.HtmlEncode(token.Value ?? string.Empty), StringComparison.Ordinal);
        }

        return output;
    }

    private async Task<string?> ResolveBrandLogoUrlAsync(string? brandLogoUrl)
    {
        if (string.IsNullOrWhiteSpace(brandLogoUrl))
            return brandLogoUrl;

        var reference = ExtractBlobReference(brandLogoUrl);
        if (string.IsNullOrWhiteSpace(reference))
            return brandLogoUrl;

        try
        {
            var sasUrl = await fileStorageService.GetReadSasUrlAsync(StorageCategory.Image, reference!);
            return string.IsNullOrWhiteSpace(sasUrl) ? brandLogoUrl : sasUrl;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to resolve organization brand logo SAS URL. Timestamp: {Timestamp}", DateTime.UtcNow);
            return brandLogoUrl;
        }
    }

    private static string? ExtractBlobReference(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
            return value.Trim().TrimStart('/');

        if (!uri.Scheme.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            return null;

        var segments = uri.AbsolutePath.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length <= 1)
            return null;

        return string.Join('/', segments.Skip(1));
    }

    private static short GetLeaseTermMonthsFromDates(DateTime startDate, DateTime? endDate)
    {
        if (!endDate.HasValue || endDate.Value <= startDate)
            return 0;

        var months = ((endDate.Value.Year - startDate.Year) * 12) + endDate.Value.Month - startDate.Month;
        if (endDate.Value.Day < startDate.Day)
            months--;

        return (short)Math.Max(months, 0);
    }

    private static string SanitizeHtml(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        var sanitized = html;
        sanitized = Regex.Replace(sanitized, "<script\\b[^<]*(?:(?!<\\/script>)<[^<]*)*<\\/script>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        sanitized = Regex.Replace(sanitized, "<iframe\\b[^<]*(?:(?!<\\/iframe>)<[^<]*)*<\\/iframe>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        sanitized = Regex.Replace(sanitized, "<object\\b[^<]*(?:(?!<\\/object>)<[^<]*)*<\\/object>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        sanitized = Regex.Replace(sanitized, "<embed\\b[^>]*>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        sanitized = Regex.Replace(sanitized, "\\son\\w+\\s*=\\s*(\"[^\"]*\"|'[^']*'|[^\\s>]+)", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        sanitized = Regex.Replace(sanitized, "(href|src)\\s*=\\s*([\"'])\\s*javascript:[^\"']*\\2", "$1=\"#\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        return sanitized.Trim();
    }

    private string GetCacheKey(Guid organizationID)
    {
        return $"{Cache.LEASECONTRACTTEMPLATES}_{organizationID}";
    }

    private void InvalidateCache(Guid organizationID)
    {
        cache.Remove(GetCacheKey(organizationID));
    }

    private async Task<LeaseContractTemplate?> EnsureDefaultTemplateForOrganizationAsync(Guid organizationID)
    {
        var existingDefault = await templateRepository.GetDefaultByOrganizationIDAsync(organizationID);
        if (existingDefault != null)
            return existingDefault;

        var standardHtml = await GetStandardTemplateHtml();
        await templateRepository.ClearDefaultFlagsAsync(organizationID);

        var entity = new LeaseContractTemplate
        {
            LeaseContractTemplateID = Guid.NewGuid(),
            OrganizationID = organizationID,
            TemplateName = "Standard Residential Lease",
            Description = "System default lease contract template",
            HtmlContent = standardHtml,
            IsDefault = true,
            IsActive = true,
            IsSystemGenerated = true,
            CapturedBy = "SYSTEM",
            CapturedDate = DateTime.UtcNow
        };

        await templateRepository.Create(entity);
        await templateRepository.AddVersionAsync(new LeaseContractTemplateVersion
        {
            LeaseContractTemplateVersionID = Guid.NewGuid(),
            LeaseContractTemplateID = entity.LeaseContractTemplateID,
            VersionNumber = 1,
            HtmlContent = standardHtml,
            ChangeSummary = "System default template",
            CapturedBy = "SYSTEM",
            CapturedDate = DateTime.UtcNow
        });

        await templateRepository.Save();
        InvalidateCache(organizationID);

        return await templateRepository.GetByTemplateIDAsync(entity.LeaseContractTemplateID);
    }

    private static Dictionary<int, string> ExtractSections(string html)
    {
        var sections = new Dictionary<int, string>();
        if (string.IsNullOrWhiteSpace(html))
            return sections;

        var sectionMatches = Regex.Matches(html, "<section\\s+class=\"section\"\\s*>.*?<\\/section>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        foreach (Match sectionMatch in sectionMatches)
        {
            var numberMatch = Regex.Match(sectionMatch.Value, "<h2>\\s*(\\d+)\\.", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (!numberMatch.Success)
                continue;

            if (int.TryParse(numberMatch.Groups[1].Value, out var sectionNumber))
                sections[sectionNumber] = NormalizeMarkup(sectionMatch.Value);
        }

        return sections;
    }

    private static HashSet<int> GetChangedSections(string oldHtml, string newHtml)
    {
        var oldSections = ExtractSections(oldHtml);
        var newSections = ExtractSections(newHtml);
        var keys = oldSections.Keys.Union(newSections.Keys).ToList();
        var changed = new HashSet<int>();

        foreach (var key in keys)
        {
            oldSections.TryGetValue(key, out var oldValue);
            newSections.TryGetValue(key, out var newValue);
            if (!string.Equals(oldValue ?? string.Empty, newValue ?? string.Empty, StringComparison.Ordinal))
                changed.Add(key);
        }

        return changed;
    }

    private static void EnsureNoRestrictedSectionsChanged(HashSet<int> changedSections)
    {
        var restricted = changedSections.Where(x => NonEditableSections.Contains(x)).OrderBy(x => x).ToList();
        if (restricted.Count == 0)
            return;

        throw new ArgumentException($"Sections 1-5 and 31-34 are locked and cannot be edited. Restricted section updates detected: {string.Join(", ", restricted)}.");
    }

    private static void EnsureDeclaredSectionsMatchChanges(ICollection<int>? declaredSections, HashSet<int> changedSections)
    {
        var changed = changedSections.OrderBy(x => x).ToList();
        var declared = (declaredSections ?? new List<int>()).Distinct().OrderBy(x => x).ToList();

        if (changed.Count == 0 && declared.Count == 0)
            return;

        if (changed.Count == 0 && declared.Count > 0)
            throw new ArgumentException("No template section content changed, but edited sections were supplied.");

        if (declared.Count == 0)
            throw new ArgumentException("You must indicate the exact section numbers being edited.");

        if (!changed.SequenceEqual(declared))
            throw new ArgumentException($"EditedSectionNumbers must exactly match changed sections. Changed: {string.Join(", ", changed)}. Declared: {string.Join(", ", declared)}.");
    }

    private static void EnsureNoChangesOutsideSections(string oldHtml, string newHtml)
    {
        var oldWithoutSections = NormalizeMarkup(Regex.Replace(oldHtml ?? string.Empty, "<section\\s+class=\"section\"\\s*>.*?<\\/section>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline));
        var newWithoutSections = NormalizeMarkup(Regex.Replace(newHtml ?? string.Empty, "<section\\s+class=\"section\"\\s*>.*?<\\/section>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline));

        if (!string.Equals(oldWithoutSections, newWithoutSections, StringComparison.Ordinal))
            throw new ArgumentException("Only agreement sections 1-5 and 31-34 can be edited. Header/footer or other non-section content changes are not allowed.");
    }

    private static string NormalizeMarkup(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        return Regex.Replace(html, "\\s+", " ").Trim();
    }
}
