# Instructions — `ProjectZero.Web`

> Conventions impératives pour le point d'entrée / composition root. Le code fait foi.

## Rôle

**Point de démarrage** de l'application et **composition root** : `Program.cs`, configuration
(`appsettings.json`), câblage DI des couches et génération OpenAPI. Aucune logique métier.

## Dépendances

Référence **uniquement `Api`** ; `Application`, `Infrastructure` et `Database` arrivent
**transitivement** (Web → Api → Infrastructure → Database). SDK `Microsoft.NET.Sdk.Web`.

## Conventions

- `Program.cs` câble tout au même endroit : `AddControllers()`, `AddOpenApi()`, `AddModules()`
  et `AddDatabase(builder.Configuration)`, puis `UseHttpsRedirection()` et `MapControllers()`.
- Le câblage des couches est regroupé dans `ServiceCollectionExtensions/` :
  - `BusinessExtensions.AddModules()` agrège les **modules** (→ `AddTvShowsModule()` d'`Api`).
  - `DatabaseExtensions.AddDatabase(configuration)` câble la base (→ `AddEfPostgreSql()` de `Database`).
- Les controllers vivent dans `Api` (bibliothèque) mais sont **découverts automatiquement** via
  la `ProjectReference` : pas besoin de `AddApplicationPart`.
- **OpenAPI** : généré au build (`openapi.json`), via les réglages `OpenApiGenerate*` du `.csproj`.
- **Configuration** : `appsettings.json` porte `ConnectionStrings:TvShowDb` (lu par
  `AddEfPostgreSql`). Aucun secret committé.

## À ne pas faire

- Aucun controller ni ViewModel ici (ils vivent dans `Api`).
- Aucune logique métier ni accès direct à EF / `Database`.
- Le câblage des couches reste dans `ServiceCollectionExtensions/` (`AddModules` / `AddDatabase`) —
  pas d'enregistrement de service en vrac dans `Program.cs`.

## Points d'entrée

- **Nouveau module** : exposer son `AddXxxModule()` (dans le projet du module) puis l'ajouter à
  `BusinessExtensions.AddModules()`.
- **Câblage transverse** (middleware, auth, services techniques) : `Program.cs`.
