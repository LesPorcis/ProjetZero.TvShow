# Instructions — projet `ProjectZero.Database`

> Conventions **impératives** pour le projet base de données (EF Core + PostgreSQL). Elles
> priment sur les habitudes par défaut. Prompt d'implémentation associé :
> [`doc/specs/add-postgres-persistence.prompt.md`](../specs/add-postgres-persistence.prompt.md). Voir aussi
> les conventions de l'adaptateur dans
> [`infrastructure-instructions.md`](infrastructure-instructions.md).

## Rôle du projet

`ProjectZero.Database` est le **projet technique de persistance** : il contient les **entités
de persistance** (entités EF), le `DbContext`, la **fabrique design-time** et les **migrations**.
C'est un projet **feuille** : il ne référence **aucun** autre projet de la solution (ni `Domain`,
ni `Application`). Il porte les packages EF Core / Npgsql.

```
ProjectZero.Database  →  (EF Core, Npgsql, EF Core Design)   [aucune référence projet]
        ▲
        │ référencé uniquement par
ProjectZero.TvShows.Infrastructure  (qui mappe les entités de persistance vers le domaine et implémente les ports)
```

## Règle d'encapsulation (non négociable)

- **Entités de persistance et `DbContext` sont `internal`.** Ils ne doivent jamais être visibles
  de `Application`, `Domain` ou `Api`.
- L'accès est ouvert **au seul adaptateur d'infrastructure** via, dans le `.csproj` :

```xml
<ItemGroup>
  <InternalsVisibleTo Include="ProjectZero.TvShows.Infrastructure" />
</ItemGroup>
```

- Aucune entité de persistance ne franchit la frontière du projet autrement que **mappée en
  entité de domaine** (le mapping vit côté `ProjectZero.TvShows.Infrastructure`).

## 1. Entités de persistance

- **Suffixe `Entity` obligatoire** : `TvShowEntity`, `DirectorEntity`, `WriterEntity`,
  `StarEntity`, `GenreEntity`, … `internal sealed`, sous `Entities/`.
- C'est l'entité de persistance — **pas le domaine** — qui porte les préoccupations EF :
  navigations, clés, colonnes, contraintes.
- **Chaque entité porte sa propre configuration EF Core** : elle **implémente
  `IEntityTypeConfiguration<XxxEntity>`** et la méthode `Configure(...)` vit **dans la classe**.
  Pas de classe de configuration séparée, pas de Fluent API épars dans `OnModelCreating`.

```csharp
namespace ProjectZero.Database.Entities;

internal sealed class TvShowEntity : IEntityTypeConfiguration<TvShowEntity>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    // …

    // La config EF Core vit DANS la classe entité.
    // Nommage par défaut EF Core (PascalCase) : pas de ToTable()/mapping de colonnes forcé.
    public void Configure(EntityTypeBuilder<TvShowEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(256).IsRequired();
    }
}
```

### Relations entre entités

- Les relations (one-to-many, many-to-many) se configurent dans le `Configure` de l'entité
  **propriétaire** (p. ex. `TvShowEntity` configure ses liens vers
  `DirectorEntity`/`WriterEntity`/`StarEntity`/`GenreEntity`). Les entités « possédées » ne
  déclarent que leur clé/colonnes + la skip navigation inverse.
- Pour le many-to-many, **nommer explicitement la table de jointure** afin de ne pas exposer
  le suffixe interne `Entity` dans le schéma SQL :

```csharp
builder.HasMany(tvShow => tvShow.Genres).WithMany(genre => genre.TvShows)
    .UsingEntity(join => join.ToTable("TvShowGenres"));
```

## 2. `DbContext`

- `internal sealed`, à la racine du projet, expose les `DbSet<XxxEntity>` et un **constructeur
  public** `TvShowDbContext(DbContextOptions<TvShowDbContext>)` (requis par EF Core même si la
  classe est `internal`).
- `OnModelCreating` **n'écrit pas** de configuration à la main : il **charge toutes les configs
  portées par les entités** via `ApplyConfigurationsFromAssembly`.

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(TvShowDbContext).Assembly);
}
```

## 3. Fabrique design-time & migrations

- `TvShowDbContextFactory` (`internal`, `IDesignTimeDbContextFactory<TvShowDbContext>`) permet
  aux outils EF de construire le contexte **sans** que l'`Api` référence EF/Design. Elle lit la
  connection string depuis la variable d'environnement `ConnectionStrings__TvShowDb` (ou un
  défaut Postgres local).
- Le projet est **son propre startup-project** pour les outils EF :

```bash
dotnet ef migrations add <Name> \
  --project ProjectZero.Database --startup-project ProjectZero.Database --output-dir Migrations

dotnet ef database update \
  --project ProjectZero.Database --startup-project ProjectZero.Database
```

- **Pas de migration automatique au démarrage** : application explicite via la CLI.
- Le seed des données de référence se fait via `HasData(...)` dans la config de l'entité concernée.
- **Versionner les migrations dans un commit dédié** : les fichiers générés sous `Migrations/`
  (`<timestamp>_<Name>.cs`, `.Designer.cs` et `TvShowDbContextModelSnapshot.cs`) sont committés
  **seuls**, isolés des changements de code applicatif. Un diff de migration séparé est plus
  simple à relire, à régénérer (`ef migrations remove` puis `add`) et à revert.

> **Versions des packages** : le provider stable `Npgsql.EntityFrameworkCore.PostgreSQL`
> (10.0.2) est compilé contre `Microsoft.EntityFrameworkCore.Relational 10.0.4`. On aligne donc
> les packages EF Core (`Microsoft.EntityFrameworkCore`, `…Design`) sur **10.0.4** pour éviter
> un conflit d'assembly (`MSB3277`). Le tooling `dotnet-ef` peut être plus récent.

## Checklist d'ajout d'une entité persistée

- [ ] Entité suffixée `Entity`, `internal sealed`, sous `Entities/`.
- [ ] Entité implémente `IEntityTypeConfiguration<XxxEntity>` (config **dans la classe**).
- [ ] `DbSet<XxxEntity>` ajouté au `DbContext`.
- [ ] Relations configurées côté propriétaire ; tables de jointure nommées.
- [ ] Migration générée (`dotnet ef migrations add …` sur `ProjectZero.Database`).
- [ ] Le mapping `entité → domaine` est ajouté côté `ProjectZero.TvShows.Infrastructure`.
