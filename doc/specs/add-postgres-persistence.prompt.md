# Prompt — Persistance PostgreSQL via EF Core (ProjetZero.TvShow)

> **Usage** : ce fichier est un prompt destiné à un agent (Claude Code) qui implémentera la
> persistance. Il décrit le contexte, l'objectif, l'état cible, les conventions imposées, les
> étapes et les critères d'acceptation. Suis-le pas à pas, en gardant le build vert après
> chaque étape. Les conventions détaillées sont dans
> [`doc/instructions/database-instructions.md`](../instructions/database-instructions.md)
> (projet base de données : DAO, `DbContext`, migrations) et
> [`doc/instructions/infrastructure-instructions.md`](../instructions/infrastructure-instructions.md)
> (adaptateurs : repositories, mapping, DI) — ces fichiers priment en cas de doute.

---

## 1. Contexte

Solution .NET 10 (`net10.0`, `Nullable`/`ImplicitUsings` activés) en **architecture
hexagonale (ports & adaptateurs)**, déjà refactorée sur `refactor/fix-hexago` :

| Projet | Rôle | Dépendances |
|---|---|---|
| `TvShow.Domain` | Cœur métier (entités) | aucune |
| `TvShow.Application` | Ports In/Out + use cases + modèles | → Domain |
| `ProjectZero.Database` | Projet base de données : DAO, `DbContext`, migrations (EF Core/Npgsql) | aucune (projet feuille) |
| `TvShow.Infrastructure` | Adaptateurs pilotés (repositories, mapping, DI) | → Application, Domain, Database |
| `TvShow.Api` | Adaptateur pilote (REST) + composition root | → Application, Infrastructure |

Aujourd'hui la seule implémentation du **port secondaire** `ITvShowRepository`
(`TvShow.Application.Ports.Out`) est `InMemoryTvShowRepository` : données en dur
(*Breaking Bad*, *The Last of Us*), aucune base réelle. Fonctionnalité à préserver :
`GET /api/tvshows` qui renvoie `Id`, `Name`, `ReleasedAt`, `Seasons`, `Episodes`.

**Objectif de cette PR** : ajouter une persistance **PostgreSQL via Entity Framework Core**
derrière le même port secondaire, sans casser l'hexagone ni le contrat HTTP existant.

---

## 2. Décisions arrêtées (cadre de cette PR)

0. **Projet base de données dédié `ProjectZero.Database`** : il contient les DAO, le
   `DbContext`, la fabrique design-time et les migrations (+ packages EF Core/Npgsql). C'est un
   projet **feuille** (aucune référence projet). DAO et `DbContext` sont `internal`, exposés au
   seul `TvShow.Infrastructure` via `InternalsVisibleTo`.
1. **DAO suffixés `Dao`** et **portant eux-mêmes leur configuration EF Core**.
   Chaque DAO implémente `IEntityTypeConfiguration<XxxDao>` ; la méthode
   `Configure(EntityTypeBuilder<XxxDao> builder)` vit **dans la classe DAO**. Le `DbContext`
   se contente d'`ApplyConfigurationsFromAssembly(...)`.
2. **Repositories `internal`** implémentant le **port secondaire** `ITvShowRepository`,
   exposés uniquement via `AddInfrastructure(...)` (encapsulation : rien de `public` ne fuit
   hors de l'infra à part l'extension DI).
3. **Coexistence** : `InMemoryTvShowRepository` est conservé (passé `internal`) et **le
   provider est sélectionnable par configuration** (`Persistence:Provider = InMemory | Postgres`,
   défaut `InMemory`). On ajoute `TvShowRepository` (EF/Postgres).
4. **Périmètre** : modéliser **toutes les entités** (`TvShowDao`, `DirectorDao`, `WriterDao`,
   `StarDao`, `GenreDao`) avec les **relations many-to-many** série ↔ personnes/genres
   (mapping → domaine). Les tables de jointure sont **nommées explicitement**
   (`TvShowDirectors`, `TvShowWriters`, `TvShowStars`, `TvShowGenres`) pour ne pas exposer le
   suffixe `Dao` dans le schéma.
5. **Migration initiale + seed** des deux séries existantes pour préserver le comportement de
   l'endpoint. Connection string dans `appsettings`, défaut local (docker-compose ultérieur).
6. **Namespaces** : on **conserve** la racine `TvShow.*` actuelle (le renommage vers
   `ProjectZero.TvShow.*` reste hors périmètre ; l'alias `using TvShowEntity = TvShow.Domain.TvShow;`
   est toléré là où il est déjà nécessaire).

---

## 3. État cible

### 3.1 `ProjectZero.Database` — DAO + DbContext + migrations
- **`Daos/*.cs`** : entités de persistance `internal sealed`, suffixe `Dao` (`TvShowDao`,
  `DirectorDao`, `WriterDao`, `StarDao`, `GenreDao`), chacune implémentant
  `IEntityTypeConfiguration<XxxDao>`. Elles portent les préoccupations EF (clé, longueurs,
  contraintes, navigations) ; le domaine reste pur. `TvShowDao` configure les relations
  many-to-many (tables de jointure nommées).
- **`TvShowDbContext.cs`** : `internal`, `DbSet<XxxDao>`, constructeur public, et
  `OnModelCreating` ⇒ `modelBuilder.ApplyConfigurationsFromAssembly(typeof(TvShowDbContext).Assembly)`.
- **`TvShowDbContextFactory.cs`** : `internal`, `IDesignTimeDbContextFactory<TvShowDbContext>`
  pour les outils EF (`dotnet ef`), afin que l'Api n'ait pas à référencer `EF Core.Design`.
- **`Migrations/`** : migration initiale + seed.
- **`.csproj`** : `Microsoft.EntityFrameworkCore`, `Npgsql.EntityFrameworkCore.PostgreSQL`,
  `Microsoft.EntityFrameworkCore.Design` (`PrivateAssets=all`) + `InternalsVisibleTo` vers
  `TvShow.Infrastructure`. **Alignement de versions** : le provider stable Npgsql (10.0.2) est
  compilé contre `EntityFrameworkCore.Relational 10.0.4` ⇒ pin EF Core sur **10.0.4** pour
  éviter le conflit d'assembly `MSB3277`.

### 3.2 `TvShow.Infrastructure` — repositories + mapping + DI
- **`Mapping/TvShowDaoMapper.cs`** : mapping `TvShowDao` → entité de domaine `TvShow`, y compris
  les collections liées (sans remonter les back-références → pas de cycle).
- **`Repositories/TvShowRepository.cs`** : `internal sealed`, implémente `ITvShowRepository`,
  lit via `TvShowDbContext` (`Include` des relations), **mappe DAO → domaine**, propage le
  `CancellationToken` (`AsNoTracking`, `ToListAsync(cancellationToken)`).
- **`Repositories/InMemoryTvShowRepository.cs`** : `internal sealed`, comportement inchangé.
- **`DependencyInjection.AddInfrastructure`** : lit `Persistence:Provider` ; si `Postgres`,
  enregistre `TvShowDbContext` (`UseNpgsql(...)`) + `TvShowRepository` ; sinon
  `InMemoryTvShowRepository`. Reste la **seule surface publique** de l'infra.
- **`.csproj`** : référence projet vers `ProjectZero.Database` ; EF Core/Npgsql obtenus
  **transitivement** (pas de `PackageReference` EF dupliqué).

### 3.3 `TvShow.Api` — composition root inchangé dans son rôle
- `Program.cs` continue d'appeler `AddApplication()` + `AddInfrastructure(...)` et garde
  **Scalar** (ne pas réintroduire Swashbuckle/`UseSwaggerUI`).
- `appsettings.json` : section `Persistence` (`Provider`) + `ConnectionStrings:TvShowDb`.
- **Pas de migration automatique au démarrage** : l'application du schéma se fait
  explicitement via `dotnet ef database update` (l'Api ne référence pas EF/Design ; les
  outils EF passent par la fabrique design-time de `ProjectZero.Database`, qui est aussi son
  propre *startup-project*).

### 3.4 Domaine & Application — **inchangés**
- Pas de fuite EF dans `Domain`/`Application`. Le port `ITvShowRepository` ne change pas de
  signature (renvoie des entités de **domaine**).

---

## 4. Étapes suggérées (build vert à chaque palier)

1. **Projet `ProjectZero.Database`** : créer le projet (EF Core + Npgsql + Design,
   `InternalsVisibleTo` vers l'infra), l'ajouter à la solution. Compiler.
2. **DAO + config** : créer les `XxxDao` (`internal`, `IEntityTypeConfiguration<XxxDao>`,
   relations sur le propriétaire). Compiler.
3. **DbContext + fabrique** : `TvShowDbContext` (`ApplyConfigurationsFromAssembly`) +
   `TvShowDbContextFactory`. Compiler.
4. **Mapping + repository EF** : `TvShowDaoMapper` + `TvShowRepository` (`internal sealed`, port
   secondaire, token propagé) côté `TvShow.Infrastructure` (référence vers `ProjectZero.Database`).
   Compiler.
5. **DI + config** : `AddInfrastructure` sélecteur de provider ; `InMemory…` → `internal` ;
   `appsettings` (`Persistence:Provider`, `ConnectionStrings:TvShowDb`). Compiler.
6. **Migration + seed** :
   `dotnet ef migrations add InitialCreate --project ProjectZero.Database --startup-project ProjectZero.Database`
   ; seed des 2 séries via `HasData`. Compiler.
7. **Vérifier** : `GET /api/tvshows` renvoie le même JSON ; provider `InMemory` par défaut
   fonctionne sans base ; en `Postgres`, migration appliquée puis endpoint OK.

---

## 5. Critères d'acceptation

- [ ] `dotnet build` réussit (0 erreur, 0 nouveau warning).
- [ ] `ProjectZero.Database` porte EF Core + Npgsql ; `Domain` et `Application` n'ont
      **aucune** dépendance EF/Npgsql ; `ProjectZero.Database` ne référence **aucun** projet.
- [ ] Les `XxxDao` existent dans `ProjectZero.Database`, sont **`internal`**, suffixés `Dao`, et
      **portent leur config EF Core** (`IEntityTypeConfiguration<XxxDao>` dans la classe).
- [ ] DAO et `DbContext` sont visibles de la seule `TvShow.Infrastructure` via
      `InternalsVisibleTo`.
- [ ] Le `DbContext` charge les configs via `ApplyConfigurationsFromAssembly` (pas de Fluent
      épars dans `OnModelCreating`).
- [ ] `TvShowRepository` et `InMemoryTvShowRepository` sont **`internal`** et implémentent le
      port secondaire `ITvShowRepository` ; seule `AddInfrastructure` est `public`.
- [ ] Mapping DAO → domaine en place (côté infra) ; aucun `Dao` ne fuit hors de l'infra.
- [ ] Provider sélectionnable par configuration ; `InMemory` par défaut marche sans base.
- [ ] `CancellationToken` propagé jusqu'à EF (`ToListAsync(cancellationToken)`).
- [ ] Une migration initiale existe ; le seed reproduit *Breaking Bad* et *The Last of Us*.
- [ ] `GET /api/tvshows` renvoie le **même JSON** qu'avant (mêmes champs et valeurs).

---

## 6. Contraintes

- Cible **`net10.0`**, `Nullable` + `ImplicitUsings` activés.
- Ne pas modifier le comportement public observable de l'API (même route, même schéma).
- Conserver **Scalar** (ne pas réintroduire Swashbuckle).
- Respecter les conventions de
  [`doc/instructions/database-instructions.md`](../instructions/database-instructions.md) et
  [`doc/instructions/infrastructure-instructions.md`](../instructions/infrastructure-instructions.md).
- Commits petits et ciblés, alignés sur les étapes du §4.
- Ne pas committer de secret : la connection string par défaut vise un Postgres **local**
  (docker-compose), les surcharges réelles passent par `appsettings.*.json`/variables d'env
  (déjà gitignorés).
