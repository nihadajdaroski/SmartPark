# SmartPark

**SmartPark** je demo spletna aplikacija za iskanje parkirišč, rezervacijo parkirnih mest in simulacijo plačil v Ljubljani.

Aplikacija uporablja interaktivni zemljevid z OpenStreetMap in Leaflet, podatkovno bazo za upravljanje rezervacij ter ASP.NET Core Identity za prijavo uporabnikov in administratorski del.

> Plačila so simulirana in niso povezana z dejanskim plačilnim ponudnikom.

## Ključne funkcionalnosti

- interaktivni zemljevid parkirišč,
- pregled lokacije, cene, delovnega časa in razpoložljivosti,
- ustvarjanje, pregled in preklic rezervacij,
- simulacija plačila s kartico ali plačilnim nalogom,
- administratorski pregled rezervacij.

## Tehnologije

- **C# / ASP.NET Core MVC**
- **Entity Framework Core**
- **SQL Server / Azure SQL**
- **ASP.NET Core Identity**
- **Leaflet.js**
- **Uradne javne strani JP LPT**
- **OpenStreetMap kot fallback vir**

## Lokalni zagon

### Predpogoji

- .NET 8 SDK
- SQL Server/LocalDB samo za produkcijski profil

### Namestitev

```bash
dotnet restore
dotnet run
```

Razvojni profil uporablja lokalno SQLite bazo `smartpark.local.db`, ki se ustvari samodejno.
Ob zagonu se ustvari demo administrator `admin@smartpark.local` z geslom `Admin!234`.
Podatki o parkiriščih se po zagonu osvežijo v ozadju iz uradnih javnih strani
JP LPT. Če LPT ni dosegljiv, se uporabi OpenStreetMap fallback, ob neuspehu obstoječi
lokalni snapshot oziroma demo parkirišče.

Za produkcijo nastavi `ConnectionStrings:SmartParkContext` v okolju in aplikacijo zaženi z okoljem, ki ni `Development`.

## Namen projekta

Projekt je izdelan za izobraževalne in demonstracijske namene ter predstavlja primer spletne aplikacije z uporabniškim, rezervacijskim in administratorskim delom.
