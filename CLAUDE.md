# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commandes

Solution : `ProjectZero.TvShows.sln` — .NET 10 (`net10.0`).

```bash
dotnet build ProjectZero.TvShows.sln                 # build de toute la solution
dotnet run --project ProjectZero.Web                 # lance l'API (host = ProjectZero.Web)
```

> Aucun projet de test n'existe dans la solution à ce jour : il n'y a donc pas de commande `dotnet test` pertinente.

### Migrations EF Core

`ProjectZero.Database` est **son propre startup-project** (l'`Api`/`Web` ne référence pas EF/Design) :

```bash
dotnet ef migrations add <Name> --project ProjectZero.Database --startup-project ProjectZero.Database --output-dir Migrations
dotnet ef database update      --project ProjectZero.Database --startup-project ProjectZero.Database
```

- **Pas de migration automatique au démarrage** : application explicite via la CLI.
- La fabrique design-time lit la connection string depuis la variable d'environnement `ConnectionStrings__TvShowDb` (sinon un Postgres local par défaut).
- **Committer les fichiers de `Migrations/` dans un commit dédié**, isolé du code applicatif.

### Choix du provider de persistance

Réglé par configuration, pas par recompilation — clé `Persistence:Provider` dans `ProjectZero.Web/appsettings.json` :
- `InMemory` (défaut) → `MockedTvShowsRepository`, aucune base requise.
- `Postgres` → `TvShowsRepository` + `TvShowDbContext` sur `ConnectionStrings:TvShowDb`.

## Architecture

Architecture **hexagonale** (ports & adapters). Le domaine est au centre ; les dépendances pointent vers l'intérieur. Flux d'une requête :

```
ProjectZero.Web (host)
  → ProjectZero.TvShows.Api          (driving adapter : Controllers, ViewModels, Mappers)
    → ProjectZero.TvShows.Application (Ports.In / UseCases)
      → ProjectZero.TvShows.Application.Ports.Out
        → ProjectZero.TvShows.Infrastructure (driven adapter : Repositories, Mapping)
          → ProjectZero.Database       (EF Core : Entities, DbContext, Migrations)
```

Les 6 projets et leurs rôles :

| Projet | Rôle | Référence |
|---|---|---|
| **Domain** | Entités métier pures (`TvShow`, `Director`, `Writer`, `Star`, `Genre`). `public sealed`, constructeurs primaires, propriétés en lecture seule. **Zéro dépendance**, aucune préoccupation EF. | — |
| **Application** | Cœur applicatif. Ports primaires (`Ports/In/ITvShowsCatalog`) et secondaires (`Ports/Out/ITvShowsRepository`), use cases (`UseCases/TvShowsCatalog`, `internal sealed`). Expose `AddApplication()`. | Domain |
| **Infrastructure** | Adaptateurs **pilotés** : implémentent les ports `Out`. Repositories + mapping persistance→domaine. Expose `AddInfrastructure(IConfiguration)`. | Application, Domain, Database |
| **Database** | Projet technique de persistance (**feuille, aucune référence projet**). Entités EF, `DbContext`, fabrique design-time, migrations. Porte les packages EF Core / Npgsql. | — |
| **Api** | Adaptateur **primaire** : Controllers, ViewModels, mappers domaine→ViewModel. Bibliothèque de classes (`FrameworkReference Microsoft.AspNetCore.App`), **pas** le host. | Application, Domain |
| **Web** | Host / point d'entrée (`Program.cs`, `appsettings.json`). Câble tout, génère `openapi.json` au build. | Api, Application, Infrastructure |

> Les contrôleurs vivent dans `Api` (lib) et sont chargés par `Web` via `AddApplicationPart(typeof(TvShowsController).Assembly)`.

### Règles impératives (priment sur les habitudes par défaut)

- **Encapsulation de la persistance** : les entités de persistance (`*Entity`) et le `DbContext` sont `internal sealed` dans `ProjectZero.Database`, exposés **au seul** `Infrastructure` via `InternalsVisibleTo`. Une entité de persistance ne franchit jamais la frontière du projet autrement que **mappée en entité de domaine**. Dans `Infrastructure`, tout est `internal` sauf `AddInfrastructure(...)`.
- **Domaine pur** : aucun attribut ni navigation EF dans `Domain`. Les préoccupations EF (clés, colonnes, relations) vivent sur les entités de persistance.
- **Config EF dans l'entité** : chaque `*Entity` implémente `IEntityTypeConfiguration<XxxEntity>` et porte sa méthode `Configure(...)`. `OnModelCreating` se contente de `ApplyConfigurationsFromAssembly`. Relations configurées côté entité propriétaire ; tables de jointure many-to-many nommées explicitement (ex. `"TvShowGenres"`) pour ne pas exposer le suffixe `Entity` dans le schéma SQL.
- **Mapping explicite** (pas d'AutoMapper), via méthodes d'extension : `.ToDomain()`/`.ToDomains()` côté `Infrastructure`, `.ToViewModel()`/`.ToViewModels()` côté `Api`. On ne remonte pas les back-références des relations (pas de cycle).
- **Repositories** : `internal sealed`, implémentent un port `Out`, renvoient des **entités de domaine**, propagent le `CancellationToken` jusqu'à EF (`AsNoTracking()` + `Include(...)` + `ToListAsync(ct)`).
- **Aucun secret committé** : la connection string par défaut vise un Postgres local ; les vraies valeurs passent par `appsettings.*.json` / variables d'environnement.

La documentation détaillée de ces conventions (avec checklists d'ajout d'entité / d'adaptateur) vit dans :
- [`doc/instructions/database-instructions.md`](doc/instructions/database-instructions.md)
- [`doc/instructions/infrastructure-instructions.md`](doc/instructions/infrastructure-instructions.md)
