# Instructions — couche `ProjectZero.TvShows.Infrastructure`

> Conventions **impératives** pour les adaptateurs pilotés (persistance). Elles priment sur
> les habitudes par défaut. Toute personne (ou agent) qui ajoute du code dans ce projet doit
> les respecter. Prompt d'implémentation associé :
> [`doc/specs/add-postgres-persistence.prompt.md`](../specs/add-postgres-persistence.prompt.md). Les
> conventions des **entités de persistance / `DbContext` / migrations** vivent dans le projet base de données :
> [`database-instructions.md`](database-instructions.md).

## Rôle de la couche

`ProjectZero.TvShows.Infrastructure` contient les **adaptateurs pilotés** (driven adapters) qui
**implémentent les ports secondaires** déclarés dans `ProjectZero.TvShows.Application.Ports.Out`. Elle
dépend de `Application` (pour les ports), de `Domain` (pour les entités) et de
`ProjectZero.Database` (pour les entités de persistance / le `DbContext`), **jamais l'inverse**. Le `Domain` reste
pur : aucun attribut ni navigation EF n'y vit ; ces préoccupations vivent sur les **entités de
persistance**, dans `ProjectZero.Database`.

```
ProjectZero.TvShows.Api → ProjectZero.TvShows.Application, ProjectZero.TvShows.Infrastructure
ProjectZero.TvShows.Infrastructure → ProjectZero.TvShows.Application, ProjectZero.TvShows.Domain, ProjectZero.Database
ProjectZero.Database  → (EF Core, Npgsql)        [aucune référence projet]
```

## Règle d'encapsulation (non négociable)

- **Tout est `internal`** dans ce projet : mappers et repositories.
- Les entités de persistance et le `DbContext` (projet `ProjectZero.Database`) sont eux aussi `internal` et ne sont
  visibles ici **que** grâce à `InternalsVisibleTo("ProjectZero.TvShows.Infrastructure")`. Ils ne doivent
  jamais ressortir d'ici.
- **La seule surface `public`** est la méthode d'extension `AddInfrastructure(...)` de
  `DependencyInjection`. C'est le seul point de câblage exposé aux autres couches.
- Conséquence : aucune entité de persistance ne doit apparaître dans une signature de `Application` ou d'`Api`.
  Les repositories renvoient des **entités de domaine**, pas des entités de persistance.

## 1. Mapping entité de persistance → Domaine

- Un mapper dédié (`Mapping/`) traduit `XxxEntity` (de `ProjectZero.Database`) → entité de domaine.
- Le mapping est **explicite** : pas de fuite d'entité de persistance vers les couches hautes, pas d'AutoMapper
  imposé. On ne remonte pas les back-références des relations (pas de cycle).

## 2. Repositories

- `internal sealed`, sous `Repositories/`, **implémentent un port secondaire**
  (`ProjectZero.TvShows.Application.Ports.Out.I…Repository`).
- Ils renvoient des **entités de domaine** (via le mapper), jamais des entités de persistance.
- Le `CancellationToken` est **propagé jusqu'à EF** (`AsNoTracking()`, `Include(...)`,
  `ToListAsync(cancellationToken)`).

```csharp
internal sealed class TvShowRepository(TvShowDbContext dbContext) : ITvShowRepository
{
    public async Task<IReadOnlyCollection<Domain.TvShow>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await dbContext.TvShows
            .AsNoTracking()
            .Include(t => t.Directors).Include(t => t.Writers)
            .Include(t => t.Stars).Include(t => t.Genres)
            .ToListAsync(cancellationToken);

        return entities.Select(TvShowMapper.ToDomain).ToList();
    }
}
```

- L'adaptateur `InMemoryTvShowRepository` (sans base) est lui aussi `internal sealed` et
  implémente le même port.

## 3. Sélection du provider & DI

- `AddInfrastructure(IServiceCollection, IConfiguration)` lit `Persistence:Provider`
  (`InMemory` | `Postgres`, défaut `InMemory`) et câble l'implémentation correspondante du
  port. C'est le **seul** point d'entrée public.
- Provider `Postgres` ⇒ `AddDbContext<TvShowDbContext>(o => o.UseNpgsql(connectionString))`
  + `TvShowRepository`. Provider `InMemory` ⇒ `InMemoryTvShowRepository`.
- EF Core / Npgsql sont fournis **transitivement** par `ProjectZero.Database` (pas de
  `PackageReference` EF dupliqué ici).
- Connection string : `ConnectionStrings:TvShowDb` dans `appsettings`. **Aucun secret
  committé** ; le défaut vise un Postgres **local** (`Host=localhost;Port=5432;…`), les vraies
  valeurs passent par `appsettings.*.json` / variables d'environnement (gitignorés). Un
  `docker-compose` pour la base locale pourra être ajouté ultérieurement.

## Checklist d'ajout d'un adaptateur de persistance

- [ ] Entités de persistance / `DbContext` ajoutés côté `ProjectZero.Database` (voir
      [`database-instructions.md`](database-instructions.md)).
- [ ] Mapping `entité → domaine` ajouté sous `Mapping/` ; aucune entité de persistance exposée hors de l'infra.
- [ ] Repository `internal sealed` implémentant le port secondaire, `CancellationToken`
      propagé.
- [ ] Câblage uniquement via `AddInfrastructure` (rien d'autre en `public`).
