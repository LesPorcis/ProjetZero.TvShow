# Prompt — Refacto : corriger l'architecture hexagonale (ProjectZero.TvShows)

> **Usage** : ce fichier est un prompt destiné à un agent (Claude Code) qui exécutera le
> refacto. Il décrit le contexte, les problèmes constatés, l'état cible, les étapes et les
> critères d'acceptation. Suis-le pas à pas, en gardant le build vert après chaque étape.

---

## 1. Contexte

Solution .NET 10 (`net10.0`, `Nullable`/`ImplicitUsings` activés) découpée en 4 projets qui
visent une **architecture hexagonale (ports & adaptateurs)** :

| Projet | Rôle | Dépendances actuelles |
|---|---|---|
| `ProjectZero.TvShows.Domain` | Cœur métier (entités) | aucune |
| `ProjectZero.TvShows.Application` | Ports (In/Out) + use cases + modèles | → Domain |
| `ProjectZero.TvShows.Infrastructure` | Adaptateurs pilotés (repositories) | → Application, Domain |
| `ProjectZero.TvShows.Api` | Adaptateur pilote (REST) + composition root | → Application, Infrastructure |

Fonctionnalité existante unique à préserver : `GET /api/tvshows` qui renvoie la liste des
séries (`Id`, `Name`, `ReleasedAt`, `Seasons`, `Episodes`).

**Direction des dépendances visée** (règle de l'hexagone) :

```
        ProjectZero.TvShows.Api  ──────────────┐ (composition root : câble les adaptateurs)
          │                       │
          ▼                       ▼
   ProjectZero.TvShows.Application  ◄──── ProjectZero.TvShows.Infrastructure
          │                       │
          └──────────► ProjectZero.TvShows.Domain ◄──────────┘
```

Le `Domain` ne dépend de **rien**. `Application` ne connaît que des **ports** (interfaces),
jamais l'`Infrastructure`. L'`Infrastructure` et l'`Api` sont des adaptateurs branchés sur
ces ports.

---

## 2. Problèmes constatés (à corriger)

1. **Le domaine est pollué par des préoccupations techniques.**
   - Les entités (`TvShow`, `Director`, `Writer`, `Star`, `Genre`) portent des attributs
     `[Required]` (`System.ComponentModel.DataAnnotations`) → validation/sérialisation, hors
     du domaine.
   - Elles exposent des **propriétés de navigation bidirectionnelles façon EF Core**
     (`IReadOnlyCollection<TvShow> TvShow { get; set; }` sur `Director`, `Writer`, `Star`,
     `Genre`) → préoccupation de persistance qui fuit dans le métier.
   - Modèle **anémique** : setters publics partout, aucune invariance protégée.

2. **Conflit de nommage** *(résolu)*.
   Le type `TvShow` portait le même segment que son namespace (`TvShow.Domain.TvShow`), ce qui
   forçait l'alias `using TvShowEntity = TvShow.Domain.TvShow;`. **Décision (cf. §3.1)** :
   re-raciner tous les namespaces en **`ProjectZero.TvShows.*`** (pluriel) + renommer
   projets/dossiers/assemblies/solution. Le segment `TvShows` (pluriel) ne collisionne plus
   avec le type `TvShow` (singulier) → l'alias disparaît ; les autres couches référencent
   `TvShow` via `using ProjectZero.TvShows.Domain;`.

3. **Pas de modèle de persistance dédié.**
   `InMemoryTvShowRepository` manipule directement l'entité de domaine. Il manque une
   **entité d'infrastructure** (`TvShowDao`, l'entité EF/persistance qui portera, elle, les
   attributs et la navigation) **mappée vers l'entité de domaine**.

4. **`CancellationToken` déclaré mais jamais propagé.**
   - `TvShowsController.GetAll` reçoit un `CancellationToken` mais appelle
     `listTvShowsUseCase.ExecuteAsync()` sans le passer.
   - `ListTvShowsUseCase.ExecuteAsync` ne transmet pas le token à `_repository.GetAllAsync()`.

5. **Contrats de sortie mal placés.**
   Le contrat HTTP `TvShowResponse` vit dans `ProjectZero.TvShows.Application.Models` et est renvoyé tel
   quel par le use case puis par le controller → l'API n'a pas de contrat propre, et le nom
   « Response » (préoccupation web) habite la couche Application.

6. **Incohérences mineures.**
   - Types de collection hétérogènes : le port In renvoie `IReadOnlyList<…>`, le port Out
     `IReadOnlyCollection<…>`.
   - Visibilité incohérente : `InMemoryTvShowRepository` est `public` alors que
     `ListTvShowsUseCase` est `internal sealed` (les adaptateurs devraient être `internal`,
     exposés uniquement via leur extension DI).
   - **OpenAPI / docs** : la config committée expose Scalar en dev
     (`AddOpenApi()` + `MapOpenApi()` + `MapScalarApiReference()`) mais ne produit aucun
     artefact `openapi.json` versionnable. De plus, le document généré par défaut est
     « sale » : route PascalCase `/api/TvShows` (token `[controller]`), content-types
     parasites (`text/plain`, `text/json`) faute de `[Produces]`, et schéma exposant le
     DTO applicatif `TvShowResponse` au lieu d'un contrat d'API dédié.
     **Décision (cf. §3.4)** : on **abandonne Scalar** au profit d'une génération
     **build-time** d'un `openapi.json` propre, et le controller renvoie des **ViewModels**.
     (Garde-fou : ne **pas** basculer vers `UseSwaggerUI`/Swashbuckle, non référencé → casse
     le build.)

---

## 3. État cible

### 3.1 `ProjectZero.TvShows.Domain` — entités pures et encapsulées
- **Aucun** `using System.ComponentModel.DataAnnotations;` ni attribut framework.
- **Supprimer** les propriétés de navigation bidirectionnelles façon EF. Ne conserver que
  les associations qui ont un sens *métier* (si une série « possède » réalisateurs,
  scénaristes, acteurs, genres, c'est une relation orientée série → personnes, jamais
  l'inverse).
- **Encapsulation (immuable, anémique)** : propriétés en **lecture seule** (`get` only),
  valeurs fixées par **constructeur** d'affectation. Plus de setter public. Choix acté :
  **pas de guards d'invariants** (modèle anémique immuable) — on garantit l'immuabilité, pas
  (encore) la validation métier. Les collections d'associations sont **copiées
  défensivement** à la construction (vraie immuabilité).
- **Associations** : conserver `TvShow` → `Directors`/`Writers`/`Stars`/`Genres` (collections
  immuables, défaut vide via paramètres optionnels du constructeur). **Supprimer** la
  navigation inverse bidirectionnelle façon EF sur `Director`/`Writer`/`Star`/`Genre`.
- **Résoudre le conflit de nommage** : re-raciner les namespaces en **`ProjectZero.TvShows.*`**
  (pluriel) — projets, dossiers, assemblies et solution renommés. Le type **reste `TvShow`**
  (singulier) : le segment de namespace étant `TvShows` (pluriel), le nom simple `TvShow` ne
  collisionne plus → l'alias `TvShowEntity` est supprimé ; les autres couches font
  `using ProjectZero.TvShows.Domain;` + `TvShow`.
  (Note : un singulier `ProjectZero.TvShow.*` n'aurait **pas** suffi — `TvShow` resterait un
  segment de namespace ; c'est le **pluriel** `TvShows` qui lève la collision.)

### 3.2 `ProjectZero.TvShows.Infrastructure` — adaptateur de persistance + DAO + mapping
- Introduire **`TvShowDao`** (et les DAO liés si nécessaire : `DirectorDao`, `WriterDao`,
  `StarDao`, `GenreDao`) : c'est ici que vivent les attributs (`[Required]`, etc.) et la
  **navigation bidirectionnelle** propres à la persistance EF.
- Fournir un **mapper** `TvShowDao` → entité de domaine `TvShow` (méthode d'extension ou
  classe de mapping dédiée).
- `InMemoryTvShowRepository` :
  - devient **`internal sealed`** (exposé uniquement via `AddInfrastructure`),
  - stocke des `TvShowDao` puis **mappe** vers le domaine,
  - **renvoie des entités de domaine** via le port `ITvShowRepository`,
  - propage le `CancellationToken`.

### 3.3 `ProjectZero.TvShows.Application` — ports & use cases propres
- **Port Out** `ITvShowRepository` : renvoie des entités de **domaine**
  (`IReadOnlyCollection<TvShow>` via `using ProjectZero.TvShows.Domain;`), `CancellationToken`
  en paramètre, **sans alias** (plus de `using TvShowEntity = …`).
- **Port In** `IListTvShowsUseCase` : renvoie **directement des entités de domaine**
  (`IReadOnlyCollection<TvShow>`). Décision actée (PR #5, commentaire
  « Pourquoi renvoyer un objet transformé et pas un objet de `Domain` ? ») : **pas de DTO
  applicatif** intermédiaire ; la transformation en contrat de sortie a lieu côté API
  (ViewModel, §3.4).
- `ListTvShowsUseCase` : **primary constructor**
  (`internal sealed class ListTvShowsUseCase(ITvShowRepository repository) : IListTvShowsUseCase`)
  et **propage le `CancellationToken`** jusqu'au repository (ici simple délégation).
- **Supprimer** `ProjectZero.TvShows.Application.Models.TvShowResponse` (contrat web inutile à ce niveau).
- **Harmoniser** les types de collection : `IReadOnlyCollection<…>` partout (In/Out).

### 3.4 `ProjectZero.TvShows.Api` — adaptateur web avec ses propres ViewModels
- **ViewModels d'API dédiés** dans `ProjectZero.TvShows.Api/ViewModels` (ex. `TvShowViewModel`) : c'est le
  contrat HTTP de sortie, **distinct** de l'entité de domaine. `TvShowResponse` est **supprimé**
  (plus aucun contrat web dans `Application`).
- **Mappers simples** dans `ProjectZero.TvShows.Api/Mappers` : méthodes d'extension statiques, code de
  mapping **explicite** (pas de bibliothèque type AutoMapper) entité de domaine → ViewModel.
- `TvShowsController` :
  - **route explicite** `[Route("api/tvshows")]` (plus de token `[controller]`, donc fin du
    `/api/TvShows` PascalCase) ;
  - renvoie `IReadOnlyCollection<TvShowViewModel>`, **mappe** l'entité de domaine → ViewModel ;
  - **propage le `CancellationToken`** reçu jusqu'au use case ;
  - `[Produces("application/json")]` + `[ProducesResponseType<…>(200)]` pour un contrat HTTP
    net (un seul content-type, type de réponse explicite).
- **OpenAPI / `openapi.json` propre** :
  - **Retirer Scalar** : package `Scalar.AspNetCore` + `MapOpenApi()` + `MapScalarApiReference()`.
  - Conserver `AddOpenApi()` et générer un **`openapi.json` au build** via
    `Microsoft.Extensions.ApiDescription.Server` (`OpenApiGenerateDocumentsOnBuild`), déposé
    à la racine du projet et nommé `openapi.json`.
  - Le document doit être **propre** : un seul content-type `application/json`, route
    `/api/tvshows`, schéma `TvShowViewModel` lisible.
- `Program.cs` reste le **composition root** (câble `AddApplication()` + `AddInfrastructure()`).

> **Remarque dépendances** : l'`Api` référence `Infrastructure` uniquement en tant que
> composition root (pour le `AddInfrastructure()`). C'est acceptable ; si tu veux durcir la
> règle, isole le câblage de l'infra dans un point de bootstrap dédié. Ne pas introduire de
> dépendance d'`Application` vers `Infrastructure`.

---

## 4. Étapes suggérées (incrémentales, build vert à chaque palier)

1. **Résoudre la collision** : re-raciner les namespaces en `ProjectZero.TvShows.*` (pluriel) —
   projets/dossiers/assemblies/solution renommés — et supprimer l'alias `TvShowEntity`. Le type
   **reste `TvShow`** ; les couches le référencent via `using ProjectZero.TvShows.Domain;`.
   Compiler.
2. **Purifier le domaine** : retirer DataAnnotations + navigation inverse EF, rendre les
   entités **immuables anémiques** (sealed class, `get` only, constructeur d'affectation),
   conserver les associations `TvShow → …` immuables. Compiler.
3. **Créer `TvShowDao` + mapping** dans `Infrastructure` ; déplacer attributs/navigation EF
   vers les DAO. Adapter `InMemoryTvShowRepository` (DAO → domaine, `internal`, token).
   Compiler.
4. **Nettoyer Application** : ports In **et** Out renvoient des **objets de domaine** (`TvShow`
   via `using`, sans alias), use case en primary constructor, propager le token, harmoniser les
   collections sur `IReadOnlyCollection`, supprimer `TvShowResponse`. Compiler.
5. **Couche API** : créer les **ViewModels** (`ProjectZero.TvShows.Api/ViewModels`) + **mappers simples**
   (`ProjectZero.TvShows.Api/Mappers`), route **explicite** `api/tvshows`, le controller mappe l'entité
   de domaine → ViewModel et **propage le token**. Compiler.
6. **OpenAPI** : retirer Scalar, générer un **`openapi.json` propre au build** (un seul
   content-type, route `/api/tvshows`) et vérifier l'endpoint.

---

## 5. Critères d'acceptation

- [ ] `dotnet build` réussit (0 erreur, 0 warning nouveau).
- [ ] `ProjectZero.TvShows.Domain` ne référence **aucun** package/attribut framework
      (`grep` de `DataAnnotations` ⇒ 0 résultat dans `ProjectZero.TvShows.Domain`).
- [ ] Plus aucun alias `using TvShowEntity = …` dans le code.
- [ ] `TvShowDao` existe dans `ProjectZero.TvShows.Infrastructure` et un mapping DAO → domaine est en
      place ; les attributs `[Required]`/navigation EF n'existent **que** sur les DAO.
- [ ] Namespaces re-racinés en `ProjectZero.TvShows.*` (pluriel) ; le type de domaine **reste
      `TvShow`** (la collision est levée par le pluriel) ; les entités sont **immuables**
      (`get` only, constructeur d'affectation, aucun setter public) — modèle **anémique** (sans
      guards) par choix ; associations `TvShow → …` conservées immuables, nav inverse EF supprimée.
- [ ] Le `CancellationToken` est propagé de bout en bout (controller → use case → repository).
- [ ] Le **ViewModel** de sortie (`TvShowViewModel`) est défini dans `ProjectZero.TvShows.Api/ViewModels`
      et mappé depuis l'**entité de domaine** via un mapper simple (`ProjectZero.TvShows.Api/Mappers`) ;
      les ports In **et** Out renvoient des entités de domaine (pas de DTO applicatif,
      `TvShowResponse` supprimé) ; collections en `IReadOnlyCollection`.
- [ ] Un `openapi.json` **propre** est généré au build (un seul content-type
      `application/json`, route `/api/tvshows`, schéma `TvShowViewModel`) ; Scalar est retiré.
- [ ] Les adaptateurs (`InMemoryTvShowRepository`) sont `internal`, exposés via leur
      extension DI.
- [ ] Direction des dépendances respectée : `Domain` ne dépend de rien ; `Application` ne
      référence pas `Infrastructure`.
- [ ] `GET /api/tvshows` renvoie le **même JSON** qu'avant le refacto (mêmes champs et
      valeurs : Breaking Bad, The Last of Us).

---

## 6. Contraintes

- Cible **`net10.0`**, `Nullable` + `ImplicitUsings` activés ; ne pas changer les versions de
  packages sans nécessité.
- Préserver les **champs et valeurs** de la réponse (Breaking Bad, The Last of Us : mêmes
  `Id`, `Name`, `ReleasedAt`, `Seasons`, `Episodes`). La route est **normalisée** vers la
  forme explicite `/api/tvshows` (cf. §1/§5) ; le schéma de sortie passe de `TvShowResponse`
  à `TvShowViewModel` (mêmes champs).
- Ne **pas** introduire Swashbuckle / `UseSwaggerUI` (non référencé → casse le build). La
  génération OpenAPI passe par `AddOpenApi()` + `Microsoft.Extensions.ApiDescription.Server`
  (build-time), pas par une UI runtime.
- Commits petits et ciblés, alignés sur les étapes du §4.
