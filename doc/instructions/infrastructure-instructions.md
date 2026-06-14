# Instructions — `ProjectZero.TvShows.Infrastructure`

> Conventions impératives pour les adaptateurs pilotés (persistance). Le code fait foi.
> Entités de persistance / `DbContext` / migrations : [`database-instructions.md`](database-instructions.md).

## Rôle

**Adaptateurs pilotés** (driven adapters) qui **implémentent les ports Out** de `Application`.
Traduisent les entités de persistance vers le `Domain`.

## Dépendances

`Application` (ports), `Domain` (entités), `ProjectZero.Database` (entités de persistance /
`DbContext`). EF Core / Npgsql arrivent **transitivement** par `Database`.

## Conventions

- **Tout est `internal`** ; seule surface `public` = `AddInfrastructure(IServiceCollection, IConfiguration)`.
- **Repositories** (`Repositories/`) : `internal sealed`, implémentent un port Out, renvoient des
  **entités de `Domain`** (via mapper). Accès EF par `dbContext.Set<XxxEntity>()`, avec
  `AsNoTracking()`, `Include`/`ThenInclude`, `ToListAsync(cancellationToken)`.
- **Mappers** (`Mapping/`) : `internal static`, **un fichier par type**, `ToDomain`/`ToDomains`
  (entité de persistance → domaine). Explicite, sans AutoMapper, sans back-référence (pas de cycle).
- **DI scindée en deux fichiers** :
  - `DependencyInjection.cs` : `AddInfrastructure` (seule API publique) orchestre — appelle
    `AddDatabase(...)` puis une méthode privée **par module** (`AddTvShowsModule`) qui enregistre
    les repositories du module.
  - `DatabaseDependencyInjection.cs` : `AddDatabase(...)` enregistre le `DbContext` / le provider EF.
- **Sélection du provider** via `Persistence:Provider` (défaut `InMemory`), lue par le helper
  `UsePostgres()` :
  - `Postgres` ⇒ `AddDbContext<TvShowDbContext>(o => o.UseNpgsql(...))` + `TvShowsRepository` (scoped).
  - `InMemory` ⇒ `MockedTvShowsRepository` (singleton), pas de `DbContext`.
- Connection string : `ConnectionStrings:TvShowDb`. Défaut = Postgres **local**, secrets hors git.

## À ne pas faire

- Ne jamais laisser ressortir une entité de persistance hors de l'infra (toujours mappée en domaine).
- Pas de `PackageReference` EF/Npgsql dupliqué (transitif via `Database`).
- Aucune dépendance vers `Api` / `Web`.

## Points d'entrée

- **Nouveau module** : méthode privée `AddXxxModule` dans `DependencyInjection.cs`, appelée par
  `AddInfrastructure`, qui enregistre les repositories du module.
- **Nouvel adaptateur de persistance** : mapper `ToDomain` sous `Mapping/` + repository
  `internal sealed` implémentant le port Out, enregistré dans la méthode du module concerné.
