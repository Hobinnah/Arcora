using System.Text.Json;
using System.Text.Json.Nodes;
using Arcora.Api.Helpers;
using Arcora.Api.Services.Implementations;
using Arcora.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Filters;

// Sanitize the serialized graph, not EF entities or shared cached DTOs. Nested generated DTOs
// can otherwise bypass the marketplace's address policy via a property or lease navigation.
public sealed class PropertyLocationPrivacyFilter(PropertyLocationAccess access, IOptions<JsonOptions> options) : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult { Value: not null } result && result.Value is not ListingMapResultDto)
        {
            var json = JsonSerializer.SerializeToNode(result.Value, options.Value.JsonSerializerOptions);
            var objects = Objects(json).ToList();
            var listingIDs = IDs(objects, "listingID");
            var propertyIDs = IDs(objects, "propertyID");
            var unitIDs = IDs(objects, "rentalUnitID");
            var addressIDs = objects.Where(IsAddress).Select(AddressID).OfType<Guid>().Distinct().ToList();
            if (addressIDs.Count > 0 || listingIDs.Length > 0 || propertyIDs.Length > 0 || unitIDs.Length > 0)
            {
                var resources = await access.ResourceAddressesAsync(listingIDs, propertyIDs, unitIDs, context.HttpContext.RequestAborted);
                var ids = addressIDs.Concat(resources.Values).Distinct().ToArray();
                var allowed = await access.PermissionsAsync(context.HttpContext.User, ids, resources, context.HttpContext.RequestAborted);
                Redact(json, allowed.Addresses, allowed.Resources);
                RedactResources(json, allowed.Resources);
                result.Value = json;
            }
        }
        await next();
    }

    private static IEnumerable<JsonObject> Objects(JsonNode? node)
    {
        if (node is JsonObject obj)
        {
            yield return obj;
            foreach (var child in obj.ToArray().SelectMany(pair => Objects(pair.Value)))
                yield return child;
        }
        else if (node is JsonArray array)
            foreach (var child in array.SelectMany(Objects))
                yield return child;
    }

    private static Guid? AddressID(JsonObject obj) =>
        Guid.TryParse(obj["addressID"]?.ToString(), out var id) ? id : null;
    private static Guid[] IDs(IEnumerable<JsonObject> objects, string field) =>
        objects.Select(obj => Guid.TryParse(obj[field]?.ToString(), out var id) ? (Guid?)id : null).OfType<Guid>().Distinct().ToArray();

    private static bool IsAddress(JsonObject obj) => AddressID(obj) != null && obj.ContainsKey("line1");

    public static void Redact(JsonNode? node, IReadOnlySet<Guid> allowed, IReadOnlySet<Guid>? resources = null)
    {
        foreach (var obj in Objects(node).ToList())
        {
            if (!IsAddress(obj) || (allowed.Contains(AddressID(obj)!.Value) && (resources == null || ParentResourceAllowed(obj, resources))))
                continue;
            var approximate = PublicMapLocation.From(ReadCoordinate(obj["latitude"]), ReadCoordinate(obj["longitude"]));
            var parent = obj.Parent as JsonObject;
            foreach (var key in new[] { "line1", "line2", "postalCode", "latitude", "longitude", "placeProvider", "placeProviderReferenceID" })
                obj[key] = null;
            if (parent != null)
                parent["name"] = null;
            if (parent?.Parent is JsonObject rentalUnit)
            {
                rentalUnit["unitNumber"] = null;
                rentalUnit["name"] = null;
                if (rentalUnit.Parent is JsonObject listing && listing.ContainsKey("listingID"))
                {
                    listing["publicLocation"] = approximate == null ? null :
                        JsonSerializer.SerializeToNode(approximate, new JsonSerializerOptions(JsonSerializerDefaults.Web));
                    listing["checkInDoorCode"] = null;
                    listing["listingAccessInstructions"] = null;
                    listing["leases"] = null;
                }

            }
        }
    }

    public static void RedactResources(JsonNode? node, IReadOnlySet<Guid> allowed)
    {
        foreach (var obj in Objects(node).ToList())
        {
            if (Guid.TryParse(obj["listingID"]?.ToString(), out var listingID) && !allowed.Contains(listingID))
            {
                foreach (var key in new[] { "checkInDoorCode", "wifiNetwork", "wifiPassword", "listingAccessInstructions", "leases" })
                    if (obj.ContainsKey(key)) obj[key] = null;
                if (obj.ContainsKey("listingAccessInstructionID"))
                {
                    obj["instructions"] = null;
                    obj["secretReference"] = null;
                }
            }
            if ((obj.ContainsKey("unitNumber") && Guid.TryParse(obj["rentalUnitID"]?.ToString(), out var unitID) && !allowed.Contains(unitID)) ||
                (obj.ContainsKey("propertyType") && Guid.TryParse(obj["propertyID"]?.ToString(), out var propertyID) && !allowed.Contains(propertyID)))
            {
                obj["name"] = null;
                if (obj.ContainsKey("unitNumber")) obj["unitNumber"] = null;
            }
        }
    }

    private static bool ParentResourceAllowed(JsonObject address, IReadOnlySet<Guid> resources)
    {
        for (var parent = address.Parent; parent != null; parent = parent.Parent)
        {
            if (parent is not JsonObject obj) continue;
            if (obj.ContainsKey("unitNumber") && Guid.TryParse(obj["rentalUnitID"]?.ToString(), out var unitID))
                return resources.Contains(unitID);
            if (obj.ContainsKey("propertyType") && Guid.TryParse(obj["propertyID"]?.ToString(), out var propertyID))
            {
                if (obj.Parent is JsonObject unit && unit.ContainsKey("unitNumber") &&
                    Guid.TryParse(unit["rentalUnitID"]?.ToString(), out var nestedUnitID))
                    return resources.Contains(nestedUnitID);
                return resources.Contains(propertyID);
            }
        }
        return true;
    }

    private static decimal? ReadCoordinate(JsonNode? node) =>
        node is JsonValue value && value.TryGetValue<decimal>(out var coordinate) ? coordinate : null;
}
