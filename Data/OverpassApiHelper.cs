using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SmartPark.Models;

namespace SmartPark.Data
{
    public class OverpassApiHelper
    {
        private readonly HttpClient _http;

        private static readonly string[] OverpassInterpreterUrls =
        {
            "https://overpass-api.de/api/interpreter",
            "https://overpass.private.coffee/api/interpreter",
            "https://overpass.kumi.systems/api/interpreter"
        };

        public OverpassApiHelper(HttpClient http)
        {
            _http = http;

            if (_http.Timeout == System.Threading.Timeout.InfiniteTimeSpan)
                _http.Timeout = TimeSpan.FromSeconds(120);

            if (!_http.DefaultRequestHeaders.UserAgent.Any())
                _http.DefaultRequestHeaders.UserAgent.ParseAdd("SmartPark/1.0 (+https://github.com/n1hadd/SmartPark)");

            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<Parkirisce>> GetParkiriscaLjubljanaAsync()
        {
            string query = @"[out:json][timeout:10];
                            nwr[""amenity""=""parking""][""access""!~""^(private|no)$""]
                            (45.98,14.38,46.15,14.70);
                            out center tags;";
                            
            Exception? lastException = null;
            foreach (var endpoint in OverpassInterpreterUrls)
            {
                try
                {
                    using var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("data", query)
                    });
                    using var req = new HttpRequestMessage(HttpMethod.Post, endpoint)
                    {
                        Content = content
                    };
                    req.Headers.Accept.Clear();
                    req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    using var resp = await _http.SendAsync(req);
                    var body = await resp.Content.ReadAsStringAsync();

                    if (!resp.IsSuccessStatusCode)
                    {
                        lastException = new HttpRequestException(
                            $"Overpass returned {(int)resp.StatusCode} {resp.ReasonPhrase}.");
                        continue;
                    }

                    return ParseParkirisca(body);
                }
                catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
                {
                    lastException = ex;
                }
            }

            throw lastException ?? new HttpRequestException("No Overpass endpoint was available.");
        }

        private static List<Parkirisce> ParseParkirisca(string body)
        {
            var parkirisca = new List<Parkirisce>();
            using var doc = JsonDocument.Parse(body);
            if (!doc.RootElement.TryGetProperty("elements", out var elements))
                return parkirisca;

            foreach (var e in elements.EnumerateArray())
            {
                // lat/lon iz node ali center iz way/relation
                double lat = 0, lon = 0;

                if (e.TryGetProperty("lat", out var latElem)) lat = latElem.GetDouble();
                else if (e.TryGetProperty("center", out var center) && center.TryGetProperty("lat", out var latC)) lat = latC.GetDouble();

                if (e.TryGetProperty("lon", out var lonElem)) lon = lonElem.GetDouble();
                else if (e.TryGetProperty("center", out var center2) && center2.TryGetProperty("lon", out var lonC)) lon = lonC.GetDouble();

                if (lat == 0 || lon == 0) continue;

                string naslov = "Parkirišče";
                int steviloMest = 0;

                if (e.TryGetProperty("tags", out var tags))
                {
                    if (tags.TryGetProperty("name", out var nameElem))
                        naslov = nameElem.GetString() ?? naslov;

                    if (tags.TryGetProperty("capacity", out var capElem))
                        int.TryParse(capElem.GetString(), out steviloMest);
                }

                if (steviloMest <= 0)
                    steviloMest = 30;

                var rng = Random.Shared;
                var demoCena = Math.Round((decimal)(1.2 + rng.NextDouble() * 2.8), 2); // 1.20–4.00

                parkirisca.Add(new Parkirisce
                {
                    Naslov = naslov,
                    Latitude = lat,
                    Longitude = lon,
                    SteviloMest = steviloMest,
                    CenaNaUro = demoCena,
                    DelovniCas = "00:00–24:00"
                });
            }

            return parkirisca;
        }
    }
}