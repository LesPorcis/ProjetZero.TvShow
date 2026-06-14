# Instructions — `ProjectZero.Database`

> Conventions impératives pour la persistance (EF Core + PostgreSQL). Le code fait foi.
> Adaptateur qui consomme ce projet : [`infrastructure-instructions.md`](infrastructure-instructions.md).

## Rôle

Projet technique de persistance : **entités de persistance** (EF), `DbContext`, **fabrique
design-time** et **migrations**.

## Dépendances

Projet **feuille** : aucune référence projet. Porte EF Core / Npgsql / EF Core Design. Entités et
`DbContext` restent `internal`, ouverts au seul adaptateur via
`<InternalsVisibleTo Include="ProjectZero.TvShows.Infrastructure" />` ; seule la DI
(`AddEfPostgreSql`) est `public`.

## Conventions

- **Tout est `internal sealed`** (entités + `DbContext`).
- **Entités** (`Entities/`) : suffixe `Entity` obligatoire, propriétés `{ get; init; }`. Chaque
  entité implémente `IEntityTypeConfiguration<XxxEntity>` et porte sa `Configure(...)` **dans la
  classe** (pas de classe de config séparée, pas de Fluent dans `OnModelCreating`).
- **Nommer les tables** via `ToTable("...")` (au pluriel) pour ne pas exposer le suffixe `Entity`
  en SQL.
- **Rôles** (`Director`/`Writer`/`Star`) = **entités de jointure** entre `PersonEntity` et
  `TvShowEntity` (clé composite `{ PersonId, TvShowId }`). **Genre** = many-to-many avec TvShow
  (`UsingEntity(join => join.ToTable("TvShowGenres"))`).
- **`DbContext`** : constructeur primaire `(DbContextOptions<TvShowDbContext>)`. **Aucun `DbSet<>`
  exposé** : `OnModelCreating` fait `ApplyConfigurationsFromAssembly(...)`, l'accès se fait via
  `Set<XxxEntity>()` (côté Infrastructure).
- **DI** (`DependencyInjection.cs`) : `public static AddEfPostgreSql(services, configuration)`
  enregistre `AddDbContext<TvShowDbContext>(o => o.UseNpgsql(...))` en lisant
  `ConnectionStrings:TvShowDb`. **Seule surface `public`** du projet (entités et `DbContext`
  restent `internal`) ; appelée depuis le composition root (`ProjectZero.Web`).
- **Fabrique design-time** `TvShowDbContextFactory` (`IDesignTimeDbContextFactory`) : lit
  `ConnectionStrings__TvShowDb` ou un défaut Postgres local. Le projet est **son propre
  startup-project** pour `dotnet ef`.
- **Versions EF alignées sur 10.0.4** (compat `Npgsql.EntityFrameworkCore.PostgreSQL` 10.0.2)
  pour éviter le conflit d'assembly `MSB3277`.
- Seed des données de référence via `HasData(...)` dans la `Configure` concernée.

## À ne pas faire

- Aucune entité de persistance ne franchit la frontière du projet autrement que **mappée en
  domaine** (le mapping vit côté `Infrastructure`).
- Aucune référence à `Domain` / `Application`.
- Pas de migration automatique au démarrage (application explicite via CLI).

## Points d'entrée

- **Nouvelle entité persistée** : `Entity` `internal sealed` sous `Entities/` +
  `IEntityTypeConfiguration` (config dans la classe) + relations côté propriétaire, puis
  `dotnet ef migrations add <Name> --project ProjectZero.Database --startup-project ProjectZero.Database`,
  enfin le mapping `entité → domaine` côté `Infrastructure`.
- **Migrations** committées **seules**, dans un commit dédié (diff isolé, plus simple à relire/revert).
