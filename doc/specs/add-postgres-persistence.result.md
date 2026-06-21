# Résultat — Persistance PostgreSQL via EF Core + projet `ProjectZero.Database`

> Compte rendu de l'implémentation décrite dans
> [`add-postgres-persistence.prompt.md`](add-postgres-persistence.prompt.md).
> Conventions associées :
> [`../instructions/database-instructions.md`](../instructions/database-instructions.md) et
> [`../instructions/infrastructure-instructions.md`](../instructions/infrastructure-instructions.md).
>
> **PR** : [LesPorcis/ProjetZero.TvShow #10](https://github.com/LesPorcis/ProjetZero.TvShow/pull/10)
> (base `refactor/fix-hexago`, branche `feat/add-postgres-persistence`).

## Projet dédié `ProjectZero.Database`

Projet **feuille** (aucune référence projet), ajouté à la solution :

- `Entities/` : `TvShowEntity`, `DirectorEntity`, `WriterEntity`, `StarEntity`, `GenreEntity` — `internal`, suffixe
  `Entity`, **configuration EF Core dans la classe** (`IEntityTypeConfiguration<XxxEntity>`).
- `TvShowDbContext.cs` (`internal`, constructeur public) + `TvShowDbContextFactory.cs`
  (design-time, pour `dotnet ef`).
- `Migrations/` : migration `InitialCreate` (namespace `ProjectZero.Database.Migrations`).
- Packages **EF Core 10.0.4 + Npgsql.EntityFrameworkCore.PostgreSQL 10.0.2 + EF Core Design**.
- `<InternalsVisibleTo Include="TvShow.Infrastructure" />` ⇒ entités de persistance et `DbContext` restent
  `internal`, visibles uniquement de l'infrastructure.

## `TvShow.Infrastructure` (adaptateurs uniquement)

- `Mapping/TvShowMapper.cs` : mapping entité → domaine (sans back-référence → pas de cycle).
- `Repositories/` : `TvShowRepository` (EF, `Include` des relations) + `InMemoryTvShowRepository`,
  tous deux `internal sealed`, sur le port secondaire `ITvShowRepository`.
- `DependencyInjection.cs` : `AddInfrastructure(IServiceCollection, IConfiguration)` — sélecteur
  de provider `Persistence:Provider` (`InMemory` défaut | `Postgres`). Seule surface `public`.
- `.csproj` : référence vers `ProjectZero.Database` ; EF Core/Npgsql **transitifs** (aucun
  `PackageReference` EF dupliqué).

## Direction des dépendances

```
TvShow.Api            → TvShow.Application, TvShow.Infrastructure
TvShow.Infrastructure → TvShow.Application, TvShow.Domain, ProjectZero.Database
ProjectZero.Database  → (EF Core, Npgsql)        [aucune référence projet]
TvShow.Application    → TvShow.Domain
TvShow.Domain         → (rien)
```

## Schéma de base

9 tables : `TvShows`, `Directors`, `Writers`, `Stars`, `Genres` + tables de jointure
many-to-many **nommées explicitement** `TvShowDirectors`, `TvShowWriters`, `TvShowStars`,
`TvShowGenres` (pour ne pas exposer le suffixe `Entity` dans le SQL). Seed des deux séries
existantes (*Breaking Bad*, *The Last of Us*) via `HasData`.

## Décisions clés

- **Nommage par défaut EF Core** (PascalCase) pour les tables d'entités.
- **Provider `InMemory` par défaut** → l'application tourne sans base.
- **Pas de migration automatique au démarrage** : application explicite via
  `dotnet ef database update --project ProjectZero.Database --startup-project ProjectZero.Database`.
- **EF Core figé en 10.0.4** (version contre laquelle Npgsql 10.0.2 est compilé) pour éviter le
  conflit d'assembly `MSB3277`.
- `docker-compose` pour la base locale : reporté.

## Vérifications

- `dotnet build` (solution, 5 projets) → **0 erreur, 0 warning**.
- Migration générée depuis `ProjectZero.Database` (son propre *startup-project* via la fabrique
  design-time).
- **Testé en local sur un vrai PostgreSQL** (binaires portables Zonky, sans install) :
  migration appliquée (9 tables), `GET /api/tvshows` → **HTTP 200**, log EF Core confirmant la
  requête PostgreSQL sur `"TvShows"` avec `LEFT JOIN` vers les tables de jointure. Cluster
  temporaire arrêté et supprimé.

## Hors périmètre / suites possibles

- Renommage des namespaces vers `ProjectZero.TvShow.*` et purification du domaine
  (`[Required]`/navigations EF encore présents sur les entités de domaine) — chantier décrit
  dans [`../refacto-archi-hexagonale.prompt.md`](../refacto-archi-hexagonale.prompt.md).
- `docker-compose` Postgres local + seed de relations.
