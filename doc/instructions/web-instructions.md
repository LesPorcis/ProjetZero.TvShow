# Instructions — `ProjectZero.Web`

> Conventions impératives pour le point d'entrée / composition root. Le code fait foi.

## Rôle

**Point de démarrage** de l'application et **composition root** : `Program.cs`, configuration
(`appsettings.json`), câblage DI des couches et génération OpenAPI. Aucune logique métier.

## Dépendances

`Api` (controllers), `Application` (`AddApplication`), `Infrastructure` (`AddInfrastructure`).
SDK `Microsoft.NET.Sdk.Web`.

## Conventions

- `Program.cs` câble tout au même endroit : `AddControllers()`, `AddOpenApi()`,
  `AddApplication()`, `AddInfrastructure(builder.Configuration)`, puis `UseHttpsRedirection()`
  et `MapControllers()`.
- Les controllers vivent dans `Api` (bibliothèque) mais sont **découverts automatiquement** via
  la `ProjectReference` : pas besoin de `AddApplicationPart`.
- **OpenAPI** : généré au build (`openapi.json`), via les réglages `OpenApiGenerate*` du `.csproj`.
- **Configuration** : `appsettings.json` porte `Persistence:Provider` et
  `ConnectionStrings:TvShowDb`. Aucun secret committé.

## À ne pas faire

- Aucun controller ni ViewModel ici (ils vivent dans `Api`).
- Aucune logique métier ni accès direct à EF / `Database`.
- Le câblage des couches passe **uniquement** par `AddApplication` / `AddInfrastructure`.

## Points d'entrée

- **Câblage transverse** (middleware, auth, services techniques) : `Program.cs`.
- **Basculer la persistance** : `Persistence:Provider` (`InMemory` | `Postgres`) dans `appsettings.json`.
