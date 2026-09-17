// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.DTOs;
using Arcora.Api.Models;
using Arcora.Api.Enums;
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Services.Interfaces;
using Arcora.Api.Configurations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Arcora.Api.Services.Implementations
{
    public class AddressService : IAddressService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<AddressService> logger;
        private readonly IAddressRepository addressRepository;
        private readonly IPostalLookupRepository postalLookupRepository;
        private readonly IOptions<CacheConfiguration> _options;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public AddressService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<AddressService> logger, IAddressRepository addressRepository, IPostalLookupRepository postalLookupRepository, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.addressRepository = addressRepository;
            this.postalLookupRepository = postalLookupRepository;
            this._options = options;
            this._httpClientFactory = httpClientFactory;
            this._configuration = configuration;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<AddressDto>> GetAll(Paging paging)
        {
            IEnumerable<Address> entities;
            try
            {
                entities = cache.Get<IEnumerable<Address>>(Cache.ADDRESSES.ToString()) ?? new List<Address>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.addressRepository.GetAddressAsync())?.Where(x => x != null) ?? new List<Address>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<Address>>(Cache.ADDRESSES.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Address by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<AddressDto>
                {
                    Data = new List<AddressDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<Address> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Line1) && x.Line1.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.AddressID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<AddressDto>>(pagedEntities);
            return new PagedResult<AddressDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<AddressDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<Address> entities = cache.Get<IEnumerable<Address>>(Cache.ADDRESSES.ToString()) ?? new List<Address>();
                Address? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.AddressID == ID);
                }
                else
                {
                    match = await this.addressRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<AddressDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Address by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<AddressDto> CreateAddress(AddressDto addressDto)
        {
            Address address = new Address();
            IEnumerable<Address?> checkEntity;
            try
            {
                checkEntity = await this.addressRepository.Find(x => x.Line1!.ToLower().Trim() == addressDto.Line1!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    address = this.mapper.Map<Address>(addressDto);
                    address.AddressID = Guid.NewGuid();
                    address.OrganizationID = addressDto.OrganizationID == Guid.Empty ? null : addressDto.OrganizationID;
                    address.PlaceProviderReferenceID = string.IsNullOrEmpty(addressDto.PlaceProviderReferenceID) ? null : addressDto.PlaceProviderReferenceID;
                    address.CapturedDate = DateTime.UtcNow;
                    address = await addressRepository.Create(address) ?? new Address();
                    await addressRepository.Save();
                    cache.Remove(Cache.ADDRESSES.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating Address. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<AddressDto>(address);
        }

        /// <inheritdoc/>
        public async Task<AddressDto?> UpdateAddress(Guid id, AddressDto addressDto)
        {
            try
            {
                var existing = await this.addressRepository.GetByID(id);
                if (existing == null)
                    return null;
                Address address = this.mapper.Map<Address>(addressDto);
                address = await addressRepository.Update(address) ?? new Address();
                await addressRepository.Save();
                cache.Remove(Cache.ADDRESSES.ToString());
                addressDto = this.mapper.Map<AddressDto>(address);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating Address. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return addressDto;
        }

        /// <inheritdoc/>
        public async Task DeleteAddress(Guid ID)
        {
            try
            {
                var address = await this.addressRepository.GetByID(ID);
                if (address == null)
                    throw new KeyNotFoundException("Address with the specified ID was not found.");
                await addressRepository.Delete(address);
                await addressRepository.Save();
                cache.Remove(Cache.ADDRESSES.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting Address . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        // Lookup addresses using Canada Post AddressComplete. Best-effort enumeration for postal-code containers.
        public async Task<List<AddressLookupSuggestionDto>> LookupAddressesByPostalCode(string postalCode, string country = "CAN")
        {
            if (string.IsNullOrWhiteSpace(postalCode))
                return new List<AddressLookupSuggestionDto>();

            var normalizedPostal = postalCode.Trim().ToUpperInvariant();
            var normalizedCountry = string.IsNullOrWhiteSpace(country) ? "CAN" : country.Trim();
            var cacheKey = $"PostalLookup:{normalizedPostal}:{normalizedCountry}";

            try
            {
                // Try cache and DB first
                var pre = await TryGetCachedOrDb(cacheKey, normalizedPostal, normalizedCountry);
                if (pre != null && pre.Any()) return pre;

                // Perform Canada Post lookup (will cache and persist results as needed)
                return await PerformCanadaPostLookup(postalCode, normalizedPostal, normalizedCountry, cacheKey);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "LookupAddressesByPostalCode exception");
                return new List<AddressLookupSuggestionDto>();
            }
        }

        private async Task<List<AddressLookupSuggestionDto>> TryGetCachedOrDb(string cacheKey, string normalizedPostal, string normalizedCountry)
        {
            try
            {
                var cached = cache.Get<List<AddressLookupSuggestionDto>>(cacheKey);
                if (cached != null)
                    return cached;
            }
            catch
            {
                // ignore cache failures
            }

            try
            {
                var dbItems = await postalLookupRepository.GetByLookupKeyAsync(normalizedPostal);
                if (dbItems != null && dbItems.Any())
                {
                    var dtoList = dbItems.Select(d => new AddressLookupSuggestionDto
                    {
                        Id = d.PlaceProviderReferenceID,
                        Text = d.Text,
                        Line1 = d.Line1,
                        Line2 = d.Line2,
                        City = d.City,
                        ProvinceCode = d.ProvinceCode,
                        PostalCode = d.PostalCode,
                        CountryCode = d.CountryCode
                    }).ToList();

                    try { cache.Set<List<AddressLookupSuggestionDto>>(cacheKey, dtoList, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes)); } catch { }
                    return dtoList;
                }
            }
            catch (Exception dbEx)
            {
                logger.LogWarning(dbEx, "Failed reading postal lookup suggestions from DB for key {LookupKey}", normalizedPostal);
            }

            return new List<AddressLookupSuggestionDto>();
        }

        private async Task<List<AddressLookupSuggestionDto>> PerformCanadaPostLookup(string postalCode, string normalizedPostal, string normalizedCountry, string cacheKey)
        {
            var canadaPostToken = _configuration["CanadaPostAddress:Token"];
            var findBase = _configuration["CanadaPostAddress:FindUrl"] ?? _configuration["CanadaPostAddress:Url"] ?? "https://ws1.postescanada-canadapost.ca/AddressComplete/Interactive/Find/v2.10/json3.ws";
            var retrieveBase = _configuration["CanadaPostAddress:RetrieveUrl"] ?? "https://ws1.postescanada-canadapost.ca/AddressComplete/Interactive/Retrieve/v2.11/json3.ws";

            if (string.IsNullOrWhiteSpace(canadaPostToken))
            {
                logger.LogWarning("Canada Post token not configured");
                var emptyRes = new List<AddressLookupSuggestionDto>();
                try { cache.Set<List<AddressLookupSuggestionDto>>(cacheKey, emptyRes, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes * 10)); } catch { }
                return emptyRes;
            }

            using var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(20);

            var cpCountry = normalizedCountry.Length == 2 ? (normalizedCountry.ToUpper() == "CA" ? "CAN" : normalizedCountry.ToUpper()) : normalizedCountry.ToUpper();

            // Initial Find
            var initialFindQuery = new Dictionary<string, string?>
            {
                ["Key"] = canadaPostToken?.Trim(),
                ["SearchTerm"] = postalCode,
                ["Country"] = cpCountry,
                ["LanguagePreference"] = "EN",
                ["MaxSuggestions"] = "50"
            };

            var initialFindUrl = QueryHelpers.AddQueryString(findBase.TrimEnd('/'), initialFindQuery!);
            logger.LogInformation("Canada Post Find URL: {FindUrl}", MaskKeyInUrl(initialFindUrl));

            try
            {
                var response = await client.GetAsync(initialFindUrl);
                var responseBody = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning("Find request failed: {Status} {Body}", (int)response.StatusCode, responseBody);
                    var emptyRes = new List<AddressLookupSuggestionDto>();
                    try { cache.Set<List<AddressLookupSuggestionDto>>(cacheKey, emptyRes, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes * 10)); } catch { }
                    return emptyRes;
                }

                var (id, firstNext) = ParseIdAndFirstNextFromFindResponse(responseBody);

                if (string.IsNullOrWhiteSpace(id))
                {
                    var parsed = ParseCanadaPostSuggestions(responseBody) ?? new List<AddressLookupSuggestionDto>();
                    try { await SaveSuggestionsToDbAsync(parsed, normalizedCountry, normalizedPostal); } catch { }
                    try { cache.Set<List<AddressLookupSuggestionDto>>(cacheKey, parsed, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes * 10)); } catch { }
                    return parsed;
                }

                if (string.Equals(firstNext, "Find", StringComparison.OrdinalIgnoreCase))
                {
                    var foundRetrieveIds = await EnumeratePrefixesAndCollectRetrieveIds(client, canadaPostToken, findBase, id, normalizedCountry);
                    var aggregated = await RetrieveSuggestionsByIds(client, retrieveBase, foundRetrieveIds, normalizedCountry);
                    var dedup = aggregated.GroupBy(a => (a.Line1, a.City, a.PostalCode)).Select(g => g.First()).ToList();
                    try { await SaveSuggestionsToDbAsync(dedup, normalizedCountry, normalizedPostal); } catch { }
                    try { cache.Set<List<AddressLookupSuggestionDto>>(cacheKey, dedup, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes)); } catch { }
                    return dedup;
                }

                // direct retrieve
                var retrieveQuery2 = new Dictionary<string, string?>
                {
                    ["Key"] = canadaPostToken?.Trim(),
                    ["Id"] = id,
                    ["Country"] = normalizedCountry,
                    ["LanguagePreference"] = "EN"
                };
                var retrieveUrlFinal = QueryHelpers.AddQueryString(retrieveBase.TrimEnd('/'), retrieveQuery2!);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                logger.LogInformation("Canada Post Retrieve URL: {RetrieveUrl}", MaskKeyInUrl(retrieveUrlFinal));
                var retrieveRespFinal = await client.GetAsync(retrieveUrlFinal);
                var retrieveBodyFinal = await retrieveRespFinal.Content.ReadAsStringAsync();
                if (!retrieveRespFinal.IsSuccessStatusCode)
                {
                    logger.LogWarning("Retrieve failed status {Status} body {Body}", (int)retrieveRespFinal.StatusCode, retrieveBodyFinal);
                    var emptyRes2 = new List<AddressLookupSuggestionDto>();
                    try { cache.Set<List<AddressLookupSuggestionDto>>(cacheKey, emptyRes2, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes)); } catch { }
                    return emptyRes2;
                }

                var parsedFinal = ParseCanadaPostSuggestions(retrieveBodyFinal) ?? new List<AddressLookupSuggestionDto>();
                try { await SaveSuggestionsToDbAsync(parsedFinal, normalizedCountry, normalizedPostal); } catch { }
                try { cache.Set<List<AddressLookupSuggestionDto>>(cacheKey, parsedFinal, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes)); } catch { }
                return parsedFinal;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "LookupAddressesByPostalCode exception");
                var emptyEx = new List<AddressLookupSuggestionDto>();
                try { cache.Set<List<AddressLookupSuggestionDto>>(cacheKey, emptyEx, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes)); } catch { }
                return emptyEx;
            }
        }

        private (string? id, string? firstNext) ParseIdAndFirstNextFromFindResponse(string responseBody)
        {
            try
            {
                using var doc = JsonDocument.Parse(responseBody);
                var root = doc.RootElement;

                string? id = null;
                if (root.ValueKind == JsonValueKind.Object)
                {
                    if (root.TryGetProperty("Items", out var items) && items.ValueKind == JsonValueKind.Array && items.GetArrayLength() > 0)
                    {
                        var first = items[0];
                        if (first.ValueKind == JsonValueKind.Object && first.TryGetProperty("Id", out var idProp) && idProp.ValueKind == JsonValueKind.String)
                            id = idProp.GetString();
                    }

                    if (string.IsNullOrWhiteSpace(id) && root.TryGetProperty("Id", out var topId) && topId.ValueKind == JsonValueKind.String)
                        id = topId.GetString();
                }

                string? firstNext = null;
                try
                {
                    using var doc2 = JsonDocument.Parse(responseBody);
                    var root2 = doc2.RootElement;
                    if (root2.TryGetProperty("Items", out var items2) && items2.ValueKind == JsonValueKind.Array && items2.GetArrayLength() > 0)
                    {
                        var first = items2[0];
                        if (first.TryGetProperty("Next", out var nextProp) && nextProp.ValueKind == JsonValueKind.String)
                            firstNext = nextProp.GetString();
                    }
                }
                catch
                {
                    // ignore
                }

                return (id, firstNext);
            }
            catch
            {
                return (null, null);
            }
        }

        private async Task<HashSet<string>> EnumeratePrefixesAndCollectRetrieveIds(HttpClient client, string? canadaPostToken, string findBase, string id, string normalizedCountry)
        {
            var prefixes = new List<string>();
            prefixes.AddRange(Enumerable.Range(0, 10).Select(i => i.ToString()));
            for (char c = 'A'; c <= 'Z'; c++) prefixes.Add(c.ToString());

            var foundRetrieveIds = new HashSet<string>();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            foreach (var prefix in prefixes)
            {
                var findQuery = new Dictionary<string, string?>
                {
                    ["Key"] = canadaPostToken?.Trim(),
                    ["SearchTerm"] = prefix,
                    ["LastId"] = id,
                    ["Country"] = normalizedCountry,
                    ["LanguagePreference"] = "EN",
                    ["MaxSuggestions"] = "50"
                };
                var findUrl = QueryHelpers.AddQueryString(findBase.TrimEnd('/'), findQuery!);
                logger.LogInformation("Canada Post Find URL: {FindUrl}", MaskKeyInUrl(findUrl));

                HttpResponseMessage findResp;
                try
                {
                    findResp = await client.GetAsync(findUrl);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Find request failed for prefix {Prefix}", prefix);
                    continue;
                }

                if (!findResp.IsSuccessStatusCode)
                {
                    logger.LogWarning("Find request returned {Status} for prefix {Prefix}", (int)findResp.StatusCode, prefix);
                    continue;
                }

                var findBody = await findResp.Content.ReadAsStringAsync();
                try
                {
                    using var findDoc = JsonDocument.Parse(findBody);
                    var rootFind = findDoc.RootElement;
                    if (rootFind.TryGetProperty("Items", out var items3) && items3.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var itm in items3.EnumerateArray())
                        {
                            if (itm.ValueKind != JsonValueKind.Object) continue;
                            var next = itm.TryGetProperty("Next", out var np) && np.ValueKind == JsonValueKind.String ? np.GetString() : null;
                            var itmId = itm.TryGetProperty("Id", out var ip) && ip.ValueKind == JsonValueKind.String ? ip.GetString() : null;
                            if (string.Equals(next, "Retrieve", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(itmId))
                                foundRetrieveIds.Add(itmId!);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed parsing find response for prefix {Prefix}", prefix);
                    continue;
                }

                if (foundRetrieveIds.Count >= 500) break;
            }

            return foundRetrieveIds;
        }

        private async Task<List<AddressLookupSuggestionDto>> RetrieveSuggestionsByIds(HttpClient client, string retrieveBase, IEnumerable<string> ids, string normalizedCountry)
        {
            var aggregated = new List<AddressLookupSuggestionDto>();
            foreach (var rid in ids)
            {
                var retrieveQuery = new Dictionary<string, string?>
                {
                    ["Key"] = _configuration["CanadaPostAddress:Token"]?.Trim(),
                    ["Id"] = rid,
                    ["Country"] = normalizedCountry,
                    ["LanguagePreference"] = "EN"
                };
                var retrieveUrl2 = QueryHelpers.AddQueryString(retrieveBase.TrimEnd('/'), retrieveQuery!);
                logger.LogInformation("Calling Retrieve for id {Id}", rid);
                try
                {
                    var rresp = await client.GetAsync(retrieveUrl2);
                    var rbody = await rresp.Content.ReadAsStringAsync();
                    if (!rresp.IsSuccessStatusCode)
                    {
                        logger.LogWarning("Retrieve failed for id {Id} status {Status}", rid, (int)rresp.StatusCode);
                        continue;
                    }
                    var list = ParseCanadaPostSuggestions(rbody);
                    if (list != null && list.Any()) aggregated.AddRange(list);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Retrieve call exception for id {Id}", rid);
                }
            }

            return aggregated;
        }

        private static List<AddressLookupSuggestionDto> ParseCanadaPostSuggestions(string responseBody)
        {
            var results = new List<AddressLookupSuggestionDto>();

            try
            {
                using var json = JsonDocument.Parse(responseBody);
                var root = json.RootElement;
                JsonElement items = default;

                foreach (var candidate in new[] { "Items", "items", "Addresses", "addresses", "Suggestions", "suggestions" })
                {
                    if (root.TryGetProperty(candidate, out var value) && value.ValueKind == JsonValueKind.Array)
                    {
                        items = value;
                        break;
                    }
                }

                if (items.ValueKind != JsonValueKind.Array)
                    return results;

                foreach (var item in items.EnumerateArray())
                {
                    var suggestion = new AddressLookupSuggestionDto
                    {
                        Id = GetStringValue(item, "Id", "id", "ItemId", "itemId"),
                        Text = GetStringValue(item, "Text", "text", "Description", "description"),
                        Line1 = GetStringValue(item, "Line1", "line1"),
                        Line2 = GetStringValue(item, "Line2", "line2"),
                        City = GetStringValue(item, "City", "city"),
                        ProvinceCode = GetStringValue(item, "ProvinceCode", "provinceCode", "Province", "province"),
                        PostalCode = GetStringValue(item, "PostalCode", "postalCode", "Postal", "postal"),
                        CountryCode = GetStringValue(item, "CountryCode", "countryCode", "Country", "country")
                    };

                    if (string.IsNullOrWhiteSpace(suggestion.CountryCode))
                        suggestion.CountryCode = "CA";

                    if (string.IsNullOrWhiteSpace(suggestion.Text) && !string.IsNullOrWhiteSpace(suggestion.Line1))
                    {
                        suggestion.Text = suggestion.Line1;
                        if (!string.IsNullOrWhiteSpace(suggestion.City))
                            suggestion.Text = $"{suggestion.Text}, {suggestion.City}";
                        if (!string.IsNullOrWhiteSpace(suggestion.ProvinceCode))
                            suggestion.Text = $"{suggestion.Text}, {suggestion.ProvinceCode}";
                        if (!string.IsNullOrWhiteSpace(suggestion.PostalCode))
                            suggestion.Text = $"{suggestion.Text} {suggestion.PostalCode}";
                    }

                    if (!string.IsNullOrWhiteSpace(suggestion.Text) || !string.IsNullOrWhiteSpace(suggestion.Line1) || !string.IsNullOrWhiteSpace(suggestion.City))
                        results.Add(suggestion);
                }
            }
            catch
            {
                // Intentionally ignore malformed external payloads and return an empty list.
            }

            return results;
        }

        private static string? GetStringValue(JsonElement element, params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                if (element.TryGetProperty(propertyName, out var value) && value.ValueKind != JsonValueKind.Null)
                    return value.ToString();
            }

            return null;
        }

        private static string MaskKeyInUrl(string url)
        {
            try
            {
                var idx = url.IndexOf("Key=", StringComparison.OrdinalIgnoreCase);
                if (idx == -1) return url;
                var start = idx + "Key=".Length;
                var end = url.IndexOf('&', start);
                if (end == -1) end = url.Length;
                return url.Substring(0, start) + "****" + url.Substring(end);
            }
            catch
            {
                return url;
            }
        }

        private async Task SaveSuggestionsToDbAsync(List<AddressLookupSuggestionDto> suggestions, string normalizedCountry, string lookupKey)
        {
            if (suggestions == null || !suggestions.Any()) return;

            try
            {
                foreach (var s in suggestions)
                {
                    if (s == null) continue;
                    // Basic skip for empty entries
                    if (string.IsNullOrWhiteSpace(s.Line1) && string.IsNullOrWhiteSpace(s.Text)) continue;

                    bool exists = false;
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(s.Id))
                            exists = await postalLookupRepository.Any(a => a.PlaceProviderReferenceID == s.Id);

                        if (!exists)
                        {
                            var line1 = s.Line1 ?? s.Text;
                            exists = await postalLookupRepository.Any(a => a.Line1 == line1 && a.City == s.City && a.PostalCode == s.PostalCode);
                        }
                    }
                    catch
                    {
                        // If existence checks fail, skip saving this item to avoid blocking the lookup
                        continue;
                    }

                    if (exists) continue;

                    var addr = new PostalLookupSuggestion
                    {
                        Id = Guid.NewGuid(),
                        Line1 = string.IsNullOrWhiteSpace(s.Line1) ? s.Text : s.Line1,
                        Line2 = s.Line2,
                        City = s.City,
                        ProvinceCode = s.ProvinceCode,
                        PostalCode = s.PostalCode,
                        CountryCode = !string.IsNullOrWhiteSpace(s.CountryCode) && s.CountryCode.Length == 2 ? s.CountryCode : (normalizedCountry.Length == 2 ? normalizedCountry : (s.CountryCode ?? "")),
                        PlaceProvider = "CanadaPost",
                        PlaceProviderReferenceID = s.Id,
                        LookupKey = lookupKey,
                        CapturedDate = DateTime.UtcNow
                    };

                    try
                    {
                        await postalLookupRepository.Create(addr);
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Failed creating postal lookup record for postal suggestion");
                        // continue with next
                    }
                }

                try
                {
                    await postalLookupRepository.Save();
                    // Clear addresses cache so new entries become visible to other flows
                    try { cache.Remove(Cache.ADDRESSES.ToString()); } catch { }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed saving postal lookup suggestions to DB");
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Unhandled error while saving postal lookup suggestions to DB");
            }
        }
    }
}
