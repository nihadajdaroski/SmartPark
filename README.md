# SmartPark

SmartPark je demo spletna aplikacija za iskanje parkirišč v Ljubljani, rezervacijo parkirnih mest in simulacijo plačil.

Aplikacija prikazuje parkirišča na zemljevidu ter omogoča uporabniške račune, rezervacije in administratorski pregled. Plačila so simulirana in ne bremenijo bančne kartice.

## Zagon na Windows

Za lokalni zagon ne potrebuješ SQL Serverja, Azure računa ali predhodnih izkušenj z .NET. Aplikacija v razvojnem načinu uporablja SQLite, lokalno datotečno bazo.

### 1. Namesti potrebno programsko opremo

1. Namesti **.NET 8 SDK** z uradne strani: [Prenos .NET 8](https://dotnet.microsoft.com/download/dotnet/8.0). Izberi **SDK**, ne samo Runtime.
2. Namesti **Git for Windows** s strani [git-scm.com](https://git-scm.com/download/win), če želiš prenesti projekt z ukazom `git clone`. Git ni potreben, če projekt preneseš kot ZIP z GitHubove strani.
3. Ko je namestitev končana, odpri novo okno PowerShella in preveri .NET:

   ```powershell
   dotnet --list-sdks
   ```

   Na seznamu mora biti različica `8.0.x`.

### 2. Prenesi projekt

V PowerShellu zaženi:

```powershell
git clone https://github.com/nihadajdaroski/SmartPark.git
cd SmartPark
```

Če si projekt že prenesel ali razširil ZIP arhiv, odpri PowerShell v mapi, kjer je datoteka `SmartPark.csproj`, in preskoči ukaz `git clone`.

### 3. Zaženi aplikacijo

V mapi projekta zaženi:

```powershell
dotnet restore
dotnet run --launch-profile http
```

Prvi ukaz prenese potrebne knjižnice. Drugi zažene aplikacijo v razvojnem načinu in izpiše lokalni naslov, običajno:

```text
http://localhost:5143
```

Odpri izpisani naslov v brskalniku. Zaustavi aplikacijo v PowerShellu z `Ctrl+C`.

## Prijava in lokalni podatki

Ob prvem zagonu se samodejno ustvari lokalna SQLite baza `smartpark.local.db` in demo administratorski račun:

- **E-pošta:** `admin@smartpark.local`
- **Geslo:** `Admin!234`

Uporabljaj ta račun samo za lokalni demo. Če spremeniš ali izbrišeš lokalno bazo, se demo podatki ob naslednjem zagonu ponovno inicializirajo.

Podatki o parkiriščih se osvežujejo v ozadju iz javnih virov JP LPT. Za pridobivanje svežih podatkov potrebuješ internetno povezavo. Če vir ni dosegljiv, aplikacija uporabi nadomestne podatke, kadar so na voljo.

## Odpravljanje pogostih težav

- **`dotnet` ni prepoznan:** preveri, da si namestil .NET 8 SDK, nato zapri in ponovno odpri PowerShell.
- **Projekt se ne zažene iz trenutne mape:** z ukazom `cd` pojdi v mapo, v kateri je `SmartPark.csproj`.
- **Naslova `localhost:5143` ni mogoče odpreti:** preveri naslov, ki ga je izpisal `dotnet run`; aplikacija se lahko zažene na drugem prostem naslovu.
- **Obnova knjižnic ne uspe:** preveri internetno povezavo in ponovno zaženi `dotnet restore`.

## Tehnologije

- C# in ASP.NET Core MVC (.NET 8)
- Entity Framework Core in SQLite za lokalni razvoj
- SQL Server za okolja, ki ga uporabljajo
- ASP.NET Core Identity
- Leaflet.js in OpenStreetMap
- Javni podatki JP LPT

Projekt je namenjen izobraževalnim in demonstracijskim namenom.
