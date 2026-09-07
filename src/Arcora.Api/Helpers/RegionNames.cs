namespace Arcora.Api.Helpers;

/// <summary>
/// Resolves province/state codes to their full display names.
/// Covers Canadian provinces/territories and US states.
/// </summary>
public static class RegionNames
{
    private static readonly IReadOnlyDictionary<string, string> Names =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // Canadian provinces and territories
            ["AB"] = "Alberta",
            ["BC"] = "British Columbia",
            ["MB"] = "Manitoba",
            ["NB"] = "New Brunswick",
            ["NL"] = "Newfoundland and Labrador",
            ["NS"] = "Nova Scotia",
            ["NT"] = "Northwest Territories",
            ["NU"] = "Nunavut",
            ["ON"] = "Ontario",
            ["PE"] = "Prince Edward Island",
            ["QC"] = "Quebec",
            ["SK"] = "Saskatchewan",
            ["YT"] = "Yukon",

            // US states
            ["AL"] = "Alabama",
            ["AK"] = "Alaska",
            ["AZ"] = "Arizona",
            ["AR"] = "Arkansas",
            ["CA"] = "California",
            ["CO"] = "Colorado",
            ["CT"] = "Connecticut",
            ["DE"] = "Delaware",
            ["FL"] = "Florida",
            ["GA"] = "Georgia",
            ["HI"] = "Hawaii",
            ["ID"] = "Idaho",
            ["IL"] = "Illinois",
            ["IN"] = "Indiana",
            ["IA"] = "Iowa",
            ["KS"] = "Kansas",
            ["KY"] = "Kentucky",
            ["LA"] = "Louisiana",
            ["ME"] = "Maine",
            ["MD"] = "Maryland",
            ["MA"] = "Massachusetts",
            ["MI"] = "Michigan",
            ["MN"] = "Minnesota",
            ["MS"] = "Mississippi",
            ["MO"] = "Missouri",
            ["MT"] = "Montana",
            ["NE"] = "Nebraska",
            ["NV"] = "Nevada",
            ["NH"] = "New Hampshire",
            ["NJ"] = "New Jersey",
            ["NM"] = "New Mexico",
            ["NY"] = "New York",
            ["NC"] = "North Carolina",
            ["ND"] = "North Dakota",
            ["OH"] = "Ohio",
            ["OK"] = "Oklahoma",
            ["OR"] = "Oregon",
            ["PA"] = "Pennsylvania",
            ["RI"] = "Rhode Island",
            ["SC"] = "South Carolina",
            ["SD"] = "South Dakota",
            ["TN"] = "Tennessee",
            ["TX"] = "Texas",
            ["UT"] = "Utah",
            ["VT"] = "Vermont",
            ["VA"] = "Virginia",
            ["WA"] = "Washington",
            ["WV"] = "West Virginia",
            ["WI"] = "Wisconsin",
            ["WY"] = "Wyoming",
        };

    /// <summary>
    /// Returns the full region name for the given code, or the original code
    /// when no mapping is found (or when it is null/whitespace).
    /// </summary>
    public static string? GetName(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return code;
        }

        return Names.TryGetValue(code.Trim(), out var name) ? name : code;
    }
}
