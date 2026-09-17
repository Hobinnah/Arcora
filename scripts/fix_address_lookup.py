from pathlib import Path

root = Path(r'C:\Users\P12A03B\Downloads\_OnBoarding\Projects\repos\Arcora')

program_path = root / 'src' / 'Arcora.Api' / 'Program.cs'
program_text = program_path.read_text(encoding='utf-8')
old = "builder.Services.AddControllers();\nbuilder.Services.AddSignalR();\n"
new = old + "builder.Services.AddHttpClient();\n"
if old not in program_text:
    raise SystemExit('Program.cs insertion point not found')
program_path.write_text(program_text.replace(old, new), encoding='utf-8')

controller_path = root / 'src' / 'Arcora.Api' / 'Controllers' / 'AddressController.cs'
controller_text = '''// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Arcora.Api.DTOs;
using Arcora.Api.Models;
using Arcora.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arcora.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AddressController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // GET: api/<AddressController>
        [Authorize(Roles = "Viewer, User, Landlord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllAddresses")]
        public async Task<IActionResult> Get([FromServices] IAddressService addressService, [FromQuery] Paging paging)
        {
            return Ok(await addressService.GetAll(paging));
        }

        // GET api/<AddressController>/5
        [Authorize(Roles = "Viewer, User, Landlord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetAddressByID")]
        public async Task<IActionResult> GetAddressByID([FromServices] IAddressService addressService, Guid id)
        {
            var result = await addressService.GetID(id);
            if (result == null)
                return NotFound(new { message = "Address with the specified ID was not found." });
            return Ok(result);
        }

        [Authorize(Roles = "Viewer, User, Landlord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        [HttpGet("LookupPostalCode", Name = "LookupPostalCode")]
        public async Task<IActionResult> LookupPostalCode([FromQuery] string postalCode, [FromQuery] string country = "CA")
        {
            if (string.IsNullOrWhiteSpace(postalCode))
                return BadRequest(new { message = "postalCode is required." });

            if (string.IsNullOrWhiteSpace(country))
                country = "CA";

            var canadaPostUrl = _configuration["CanadaPostAddress:Url"];
            var canadaPostToken = _configuration["CanadaPostAddress:Token"];

            if (string.IsNullOrWhiteSpace(canadaPostUrl) || string.IsNullOrWhiteSpace(canadaPostToken))
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    message = "Canada Post address lookup is not configured. Set CanadaPostAddress:Url and CanadaPostAddress:Token in appsettings.json."
                });

            using var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(20);

            using var request = new HttpRequestMessage(HttpMethod.Post, canadaPostUrl.Trim());
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", canadaPostToken.Trim());

            var payload = new Dictionary<string, string>
            {
                ["country"] = country.Trim(),
                ["postalCode"] = postalCode.Trim(),
                ["language"] = "en"
            };

            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            try
            {
                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, new { message = "Canada Post address lookup failed.", details = responseBody });

                var suggestions = ParseCanadaPostSuggestions(responseBody);
                return Ok(suggestions);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status502BadGateway, new { message = "Failed to reach the Canada Post address lookup service.", detail = ex.Message });
            }
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

        // POST api/<AddressController>
        [Authorize(Roles = "User, Landlord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateAddress")]
        public async Task<IActionResult> CreateAddress([FromServices] IAddressService addressService, [FromBody] AddressDto addressDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await addressService.CreateAddress(addressDto);
            if (result.AddressID != null && result.AddressID != Guid.Empty)
                return CreatedAtRoute("GetAddressByID", new { id = result.AddressID }, result);
            return BadRequest(new { message = "Failed to create address. A address with the same name may already exist." });
        }

        // PUT api/<AddressController>/5
        [Authorize(Roles = "User, Landlord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateAddress")]
        public async Task<IActionResult> UpdateAddress([FromServices] IAddressService addressService, Guid id, [FromBody] AddressDto addressDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await addressService.UpdateAddress(id, addressDto);
            if (result == null)
                return NotFound(new { message = "Address with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<AddressController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteAddress")]
        public async Task<IActionResult> DeleteAddress([FromServices] IAddressService addressService, Guid id)
        {
            try
            {
                await addressService.DeleteAddress(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Address with the specified ID was not found." });
            }
        }
    }
}
'''
controller_path.write_text(controller_text, encoding='utf-8')
print('Updated Program.cs and AddressController.cs')
