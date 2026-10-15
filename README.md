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
- **OpenStreetMap Overpass API**

## Lokalni zagon

### Predpogoji

- .NET SDK (LTS)
- SQL Server, LocalDB ali Azure SQL

### Namestitev

```bash
git clone https://github.com/nihadajdaroski/SmartPark.git
cd SmartPark
dotnet ef database update
dotnet run
```

Pred zagonom v `appsettings.json` nastavi ustrezen `DefaultConnection` connection string.

## Namen projekta

Projekt je izdelan za izobraževalne in demonstracijske namene ter predstavlja primer spletne aplikacije z uporabniškim, rezervacijskim in administratorskim delom.
