using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using SmartPark.Models;

namespace SmartPark.Data;

public sealed class LptParkingProvider
{
    private const string AvailabilityUrl =
        "https://www.lpt.si/parkirisca/informacije-za-parkiranje/prikaz-zasedenosti-parkirisc";
    private const string PersonalParkingPath = "/parkirisca-za-osebna-vozila/";

    public async Task<List<Parkirisce>> GetParkiriscaAsync(CancellationToken cancellationToken)
    {
        using var httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
        httpClient.DefaultRequestHeaders.UserAgent.Add(
            new ProductInfoHeaderValue("SmartPark", "1.0"));

        var availabilityHtml = await httpClient.GetStringAsync(AvailabilityUrl, cancellationToken);
        var locations = ParseLocations(availabilityHtml);
        if (locations.Count == 0)
            throw new InvalidOperationException("LPT returned no personal-vehicle parking locations.");

        using var gate = new SemaphoreSlim(6);
        var tasks = locations.Select(async location =>
        {
            await gate.WaitAsync(cancellationToken);
            try
            {
                var html = await httpClient.GetStringAsync(location.Url, cancellationToken);
                return ParseParking(location.Name, html);
            }
            finally
            {
                gate.Release();
            }
        });

        var parking = (await Task.WhenAll(tasks))
            .Where(item => item is not null)
            .Cast<Parkirisce>()
            .ToList();

        if (parking.Count == 0)
            throw new InvalidOperationException("LPT parking detail pages contained no usable locations.");

        return parking;
    }

    private static List<(string Name, string Url)> ParseLocations(string html)
    {
        const string pattern =
            "<a[^>]*title=\"Parkirišče\\s+(?<name>[^\"]+)\"[^>]*href=\"(?<url>[^\"]*" +
            PersonalParkingPath + "[^\"]*)\"";

        return Regex.Matches(html, pattern, RegexOptions.IgnoreCase)
            .Select(match => (
                WebUtility.HtmlDecode(match.Groups["name"].Value).Trim(),
                WebUtility.HtmlDecode(match.Groups["url"].Value)))
            .Where(item => Uri.TryCreate(item.Item2, UriKind.Absolute, out _))
            .DistinctBy(item => item.Item2)
            .ToList();
    }

    private static Parkirisce? ParseParking(string name, string html)
    {
        var coordinates = Regex.Match(
            html,
            "\"lat\":(?<lat>-?\\d+(?:\\.\\d+)?),\"lng\":(?<lng>-?\\d+(?:\\.\\d+)?)",
            RegexOptions.IgnoreCase);

        if (!coordinates.Success ||
            !double.TryParse(coordinates.Groups["lat"].Value, out var latitude) ||
            !double.TryParse(coordinates.Groups["lng"].Value, out var longitude))
            return null;

        var capacityMatch = Regex.Match(
            html,
            @"Na\s+parkirišču\s+je\s+(?<capacity>\d+)\s+parkirnih\s+mest",
            RegexOptions.IgnoreCase);
        var capacity = capacityMatch.Success &&
                       int.TryParse(capacityMatch.Groups["capacity"].Value, out var parsedCapacity)
            ? parsedCapacity
            : 30;

        var priceMatch = Regex.Match(
            html,
            @"Cena\s+parkiranja.*?(?<price>\d+[,.]\d+)\s*&euro;",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);
        var price = priceMatch.Success &&
                    decimal.TryParse(
                        priceMatch.Groups["price"].Value.Replace(',', '.'),
                        System.Globalization.NumberStyles.Number,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var parsedPrice)
            ? parsedPrice
            : 1.50m;

        var workingHours = string.Join(
            "; ",
            Regex.Matches(
                    html,
                    @"<td[^>]*>\s*(?<from>\d{2}:\d{2})\s*-\s*(?<to>\d{2}:\d{2})\s*</td>",
                    RegexOptions.IgnoreCase)
                .Select(match => $"{match.Groups["from"].Value}–{match.Groups["to"].Value}")
                .Distinct());

        return new Parkirisce
        {
            Naslov = name,
            Latitude = latitude,
            Longitude = longitude,
            SteviloMest = capacity,
            CenaNaUro = price,
            DelovniCas = string.IsNullOrWhiteSpace(workingHours)
                ? "Ni navedeno na strani LPT"
                : workingHours
        };
    }
}
