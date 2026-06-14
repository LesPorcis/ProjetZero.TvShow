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

- **Tout est `internal`** ; seule surface `public` = `AddInfrastructure()` (sans paramètre).
- **Repositories** (`Repositories/`) : `internal sealed`, implémentent un port Out, renvoient des
  **entités de `Domain`** (via mapper). Accès EF par `dbContext.Set<XxxEntity>()`, avec
  `AsNoTracking()`, `Include`/`ThenInclude`, `ToListAsync(cancellationToken)`.
- **Mappers** (`Mapping/`) : `internal static`, **un fichier par type**, `ToDomain`/`ToDomains`
  (entité de persistance → domaine). Explicite, sans AutoMapper, sans back-référence (pas de cycle).
- **DI** (`DependencyInjection.cs`) : `AddInfrastructure()` enregistre les repositories du module
  (`ITvShowsRepository → TvShowsRepository`, scoped). Le `DbContext` n'est **pas** câblé ici — il
  l'est par `ProjectZero.Database` via `AddEfPostgreSql` (voir
  [`database-instructions.md`](database-instructions.md)). Postgres est le seul provider (plus de
  repository en mémoire).

## À ne pas faire

- Ne jamais laisser ressortir une entité de persistance hors de l'infra (toujours mappée en domaine).
- Pas de `PackageReference` EF/Npgsql dupliqué (transitif via `Database`).
- Aucune dépendance vers `Api` / `Web`.

## Points d'entrée

- **Nouvel adaptateur de persistance** : mapper `ToDomain` sous `Mapping/` + repository
  `internal sealed` implémentant le port Out, enregistré dans `AddInfrastructure`.
