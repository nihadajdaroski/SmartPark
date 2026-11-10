# SmartPark

SmartPark je demo spletna aplikacija za iskanje parkirišč v Ljubljani, rezervacijo parkirnih mest in simulacijo plačil.

Aplikacija prikazuje parkirišča na zemljevidu ter omogoča uporabniške račune, rezervacije in administratorski pregled. Plačila so simulirana in ne bremenijo bančne kartice.

## Zagon na Windows

Za lokalni zagon ne potrebuješ SQL Serverja, Azure računa ali predhodnih izkušenj z .NET. Aplikacija v razvojnem načinu uporablja SQLite, lokalno datotečno bazo.

### 1. Namesti potrebno programsko opremo

1. Namesti **.NET 10 SDK** z uradne strani: [Prenos .NET 10](https://dotnet.microsoft.com/download/dotnet/10.0). Izberi **SDK**, ne samo Runtime.
2. Namesti **Git for Windows** s strani [git-scm.com](https://git-scm.com/download/win), če želiš prenesti projekt z ukazom `git clone`. Git ni potreben, če projekt preneseš kot ZIP z GitHubove strani.
3. Ko je namestitev končana, odpri novo okno PowerShella in preveri .NET:

   ```powershell
   dotnet --list-sdks
   ```

   Na seznamu mora biti različica `10.0.x`.

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

## Opombe za strežniško okolje

- V ne-razvojnem okolju aplikacija uporablja SQL Server in ob zagonu uporabi čakajoče EF Core migracije. Povezavo podaj prek okoljske spremenljivke `ConnectionStrings__SmartParkContext`; uporabniški račun mora imeti pravice za izvedbo migracij.
- Za varen prvi administratorski račun pred prvim zagonom nastavi `BootstrapAdmin__Email` in `BootstrapAdmin__Password` v zaščiteni konfiguracijski datoteki zunaj repozitorija. Aplikacija ustvari administratorja samo, če račun še ne obstaja. Uporabi močno, unikatno geslo; po uspešnem prvem zagonu odstrani obe bootstrap vrednosti in ponovno zaženi storitev. Demo račun iz lokalnega razvoja ni primeren za strežnik.
- Pri Nginxu naj Kestrel posluša samo na loopback naslovu. Nginx mora nastaviti `X-Forwarded-For` in `X-Forwarded-Proto`; aplikacija zaupa posredovanim glavam samo od lokalnega proxyja.
- `/health/live` preveri, ali aplikacija odgovarja, `/health/ready` pa preveri tudi povezavo z bazo. Swagger je na voljo samo v razvojnem okolju.
- Po petih zaporednih neuspešnih prijavah Identity zaklene račun za 15 minut. Za potrjevanje e-pošte in ponastavitev gesla ni konfiguriran SMTP pošiljatelj; te funkcije niso pripravljene za uporabo na strežniku.
- Plačila so simulirana in ne izvajajo bančnih transakcij.

## Odpravljanje pogostih težav

- **`dotnet` ni prepoznan:** preveri, da si namestil .NET 10 SDK, nato zapri in ponovno odpri PowerShell.
- **Projekt se ne zažene iz trenutne mape:** z ukazom `cd` pojdi v mapo, v kateri je `SmartPark.csproj`.
- **Naslova `localhost:5143` ni mogoče odpreti:** preveri naslov, ki ga je izpisal `dotnet run`; aplikacija se lahko zažene na drugem prostem naslovu.
- **Obnova knjižnic ne uspe:** preveri internetno povezavo in ponovno zaženi `dotnet restore`.

## Tehnologije

- C# in ASP.NET Core MVC (.NET 10)
- Entity Framework Core in SQLite za lokalni razvoj
- SQL Server za okolja, ki ga uporabljajo
- ASP.NET Core Identity
- Leaflet.js in OpenStreetMap
- Javni podatki JP LPT

Projekt je namenjen izobraževalnim in demonstracijskim namenom.
