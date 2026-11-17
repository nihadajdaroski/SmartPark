using SmartPark.Models;

namespace SmartPark.Data;

public static class FallbackParking
{
    public static List<Parkirisce> Create() =>
    [
        new() { Naslov = "BS/4 I", Latitude = 46.087027, Longitude = 14.514159, SteviloMest = 30, CenaNaUro = 1.0m, DelovniCas = "06:00–22:00; 22:00–06:00" },
        new() { Naslov = "Bežigrad", Latitude = 46.062506, Longitude = 14.508283, SteviloMest = 64, CenaNaUro = 1.0m, DelovniCas = "06:00–22:00; 22:00–06:00" },
        new() { Naslov = "Dolenjska cesta (Strelišče)", Latitude = 46.039293, Longitude = 14.517513, SteviloMest = 67, CenaNaUro = 0.7m, DelovniCas = "06:00–22:00; 22:00–06:00" },
        new() { Naslov = "Gosarjeva ulica", Latitude = 46.074511, Longitude = 14.518057, SteviloMest = 193, CenaNaUro = 0.7m, DelovniCas = "06:00–22:00; 22:00–06:00" },
        new() { Naslov = "Gospodarsko razstavišče", Latitude = 46.061857, Longitude = 14.510096, SteviloMest = 400, CenaNaUro = 1.5m, DelovniCas = "Ni navedeno na strani LPT" },
        new() { Naslov = "Komanova ulica", Latitude = 46.088342, Longitude = 14.513068, SteviloMest = 30, CenaNaUro = 1.5m, DelovniCas = "Ni navedeno na strani LPT" },
        new() { Naslov = "Kranjčeva ulica", Latitude = 46.070224, Longitude = 14.522091, SteviloMest = 112, CenaNaUro = 0.7m, DelovniCas = "06:00–22:00; 22:00–06:00" },
        new() { Naslov = "Linhartova", Latitude = 46.062907, Longitude = 14.509215, SteviloMest = 121, CenaNaUro = 1.0m, DelovniCas = "22:00–06:00" },
        new() { Naslov = "P+R  Stanežiče", Latitude = 46.107329, Longitude = 14.44779, SteviloMest = 30, CenaNaUro = 1.5m, DelovniCas = "00:00–24:00" },
        new() { Naslov = "P+R Barje", Latitude = 46.026936, Longitude = 14.499946, SteviloMest = 30, CenaNaUro = 1.5m, DelovniCas = "00:00–24:00" },
        new() { Naslov = "P+R Dolgi Most", Latitude = 46.036583, Longitude = 14.463289, SteviloMest = 30, CenaNaUro = 1.5m, DelovniCas = "Ni navedeno na strani LPT" },
        new() { Naslov = "P+R Ježica", Latitude = 46.098201, Longitude = 14.516134, SteviloMest = 30, CenaNaUro = 1.5m, DelovniCas = "00:00–24:00" },
        new() { Naslov = "PH ŠRC Stožice", Latitude = 46.080505, Longitude = 14.523683, SteviloMest = 30, CenaNaUro = 1.5m, DelovniCas = "06:00–22:00; 22:00–06:00" },
        new() { Naslov = "Povšetova ulica", Latitude = 46.051135, Longitude = 14.529101, SteviloMest = 89, CenaNaUro = 0.7m, DelovniCas = "06:00–22:00; 22:00–06:00" },
        new() { Naslov = "Sanatorij Emona", Latitude = 46.053746, Longitude = 14.508921, SteviloMest = 25, CenaNaUro = 1.5m, DelovniCas = "06:00–22:00; 22:00–06:00" },
        new() { Naslov = "Slovenčeva ulica", Latitude = 46.088637, Longitude = 14.507347, SteviloMest = 130, CenaNaUro = 1.5m, DelovniCas = "Ni navedeno na strani LPT" },
        new() { Naslov = "Tacen (Kajakaška cesta)", Latitude = 46.118257, Longitude = 14.461505, SteviloMest = 69, CenaNaUro = 0.7m, DelovniCas = "06:00–22:00; 22:00–06:00" },
        new() { Naslov = "Tivoli I.", Latitude = 46.056527, Longitude = 14.500493, SteviloMest = 352, CenaNaUro = 1.0m, DelovniCas = "06:00–22:00; 22:00–06:00" },
        new() { Naslov = "Tivoli II.", Latitude = 46.059854, Longitude = 14.497147, SteviloMest = 154, CenaNaUro = 1.0m, DelovniCas = "06:00–22:00; 22:00–06:00" },
        new() { Naslov = "Trg mladinskih delovnih brigad", Latitude = 46.047154, Longitude = 14.495949, SteviloMest = 24, CenaNaUro = 1.0m, DelovniCas = "06:00–22:00; 22:00–06:00" },
        new() { Naslov = "Trg prekomorskih brigad", Latitude = 46.068574, Longitude = 14.489327, SteviloMest = 67, CenaNaUro = 0.7m, DelovniCas = "06:00–22:00; 22:00–06:00" },
        new() { Naslov = "Štembalova ulica", Latitude = 46.079123, Longitude = 14.515369, SteviloMest = 157, CenaNaUro = 1.5m, DelovniCas = "Ni navedeno na strani LPT" },
        new() { Naslov = "Žale I.", Latitude = 46.068624, Longitude = 14.528837, SteviloMest = 128, CenaNaUro = 0.7m, DelovniCas = "06:00–22:00; 22:00–06:00" },
        new() { Naslov = "Žale II.", Latitude = 46.06822, Longitude = 14.533084, SteviloMest = 87, CenaNaUro = 0.7m, DelovniCas = "06:00–22:00; 22:00–06:00" },
        new() { Naslov = "Žale III.", Latitude = 46.067967, Longitude = 14.526028, SteviloMest = 91, CenaNaUro = 0.7m, DelovniCas = "06:00–22:00; 22:00–06:00" },
        new() { Naslov = "Žale IV.", Latitude = 46.069503, Longitude = 14.52705, SteviloMest = 82, CenaNaUro = 0.7m, DelovniCas = "06:00–22:00; 22:00–06:00" },
        new() { Naslov = "Žale V.", Latitude = 46.0748, Longitude = 14.531262, SteviloMest = 288, CenaNaUro = 0.7m, DelovniCas = "06:00–22:00; 22:00–06:00" }
    ];
}
