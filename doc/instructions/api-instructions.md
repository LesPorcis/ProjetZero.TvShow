# Instructions — `ProjectZero.TvShows.Api`

> Conventions impératives pour l'adaptateur primaire HTTP. Le code fait foi.
> Le démarrage de l'appli vit dans [`web-instructions.md`](web-instructions.md).

## Rôle

**Adaptateur primaire** (REST) : `Controllers`, `ViewModels` et `Mappers` domaine → ViewModel.
C'est une **bibliothèque de classes**, pas le point de démarrage : ses controllers sont
**découverts automatiquement** par `ProjectZero.Web` via la `ProjectReference`. Le projet porte
aussi la **composition DI du module** (`AddTvShowsModule`).

## Dépendances

`Application` (ports In + DI), `Domain` (mappers) et `Infrastructure` (pour composer la DI du
module). SDK `Microsoft.NET.Sdk` + `FrameworkReference Microsoft.AspNetCore.App` (**pas** `Sdk.Web`).

## Conventions

- **Controllers** (`Controllers/`) : `public sealed`, `[ApiController]`, route `api/...`,
  `[Produces("application/json")]`. Injectent un **port In** (constructeur primaire), délèguent
  au cas d'usage puis mappent vers ViewModel. `CancellationToken` propagé.
- **ViewModels** (`ViewModels/`) : `public sealed`, propriétés `required`/`init`, **un fichier
  par type**. DTO de sortie HTTP, découplés du domaine.
- **Mappers** (`Mappers/`) : `internal static`, **un fichier par type**, extensions
  `ToViewModel`/`ToViewModels` (domaine → ViewModel).
- **DI du module** (`DependencyInjection.cs`) : `AddTvShowsModule()` (public) compose le module —
  `AddApplication()` (use cases) puis `AddInfrastructure()` (adaptateurs). C'est la seule raison de
  référencer `Infrastructure`. Agrégée par `ProjectZero.Web` via `AddModules()`.

## À ne pas faire

- Pas d'accès données / EF ni de requête dans les **controllers** : ils ne dépendent que des
  ports (`Application`). `Infrastructure` n'est référencée que pour la DI du module ; aucune
  référence à `Database`.
- Pas de `Program.cs` ni `appsettings.json` (ils vivent dans `ProjectZero.Web`).
- Pas de génération OpenAPI ici (déplacée dans `Web`).
- Ne pas renvoyer une entité de `Domain` : toujours passer par un ViewModel.
- Pas de logique métier dans les controllers.

## Points d'entrée

- **Nouvel endpoint** : controller `public sealed` injectant un port In + ViewModel(s) +
  mapper(s) dédiés.
