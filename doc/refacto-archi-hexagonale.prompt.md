# Prompt — Refacto : corriger l'architecture hexagonale (ProjetZero.TvShow)

> **Usage** : ce fichier est un prompt destiné à un agent (Claude Code) qui exécutera le
> refacto. Il décrit le contexte, les problèmes constatés, l'état cible, les étapes et les
> critères d'acceptation. Suis-le pas à pas, en gardant le build vert après chaque étape.

---

## 1. Contexte

Solution .NET 10 (`net10.0`, `Nullable`/`ImplicitUsings` activés) découpée en 4 projets qui
visent une **architecture hexagonale (ports & adaptateurs)** :

| Projet | Rôle | Dépendances actuelles |
|---|---|---|
| `TvShow.Domain` | Cœur métier (entités) | aucune |
| `TvShow.Application` | Ports (In/Out) + use cases + modèles | → Domain |
| `TvShow.Infrastructure` | Adaptateurs pilotés (repositories) | → Application, Domain |
| `TvShow.Api` | Adaptateur pilote (REST) + composition root | → Application, Infrastructure |

Fonctionnalité existante unique à préserver : `GET /api/tvshows` qui renvoie la liste des
séries (`Id`, `Name`, `ReleasedAt`, `Seasons`, `Episodes`).

**Direction des dépendances visée** (règle de l'hexagone) :

```
        TvShow.Api  ──────────────┐ (composition root : câble les adaptateurs)
          │                       │
          ▼                       ▼
   TvShow.Application  ◄──── TvShow.Infrastructure
          │                       │
          └──────────► TvShow.Domain ◄──────────┘
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

2. **Conflit de nommage `TvShow.Domain.TvShow`.**
   Le type `TvShow` porte le même segment que la racine du namespace, ce qui force l'alias
   `using TvShowEntity = TvShow.Domain.TvShow;` dans `ITvShowRepository` et
   `InMemoryTvShowRepository`. Symptôme d'un découpage de namespaces à revoir.

3. **Pas de modèle de persistance dédié.**
   `InMemoryTvShowRepository` manipule directement l'entité de domaine. Il manque une
   **entité d'infrastructure** (`TvShowDao`, l'entité EF/persistance qui portera, elle, les
   attributs et la navigation) **mappée vers l'entité de domaine**.

4. **`CancellationToken` déclaré mais jamais propagé.**
   - `TvShowsController.GetAll` reçoit un `CancellationToken` mais appelle
     `listTvShowsUseCase.ExecuteAsync()` sans le passer.
   - `ListTvShowsUseCase.ExecuteAsync` ne transmet pas le token à `_repository.GetAllAsync()`.

5. **Contrats de sortie mal placés.**
   Le contrat HTTP `TvShowResponse` vit dans `TvShow.Application.Models` et est renvoyé tel
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

### 3.1 `TvShow.Domain` — entités pures et encapsulées
- **Aucun** `using System.ComponentModel.DataAnnotations;` ni attribut framework.
- **Supprimer** les propriétés de navigation bidirectionnelles façon EF. Ne conserver que
  les associations qui ont un sens *métier* (si une série « possède » réalisateurs,
  scénaristes, acteurs, genres, c'est une relation orientée série → personnes, jamais
  l'inverse).
- **Encapsulation** : propriétés en lecture seule (`get` / `init` ou `private set`),
  initialisation par **constructeur** ou **factory** qui valide les invariants
  (ex. `Name` non vide, `Seasons`/`Episodes` ≥ 0). Plus de setters publics nus.
- **Résoudre le conflit de nommage** pour faire disparaître l'alias `TvShowEntity`.
  Approche recommandée : adopter une racine de namespace non ambiguë
  (`ProjectZero.TvShow.Domain`, `…Application`, `…Infrastructure`, `…Api`) afin que le type
  `TvShow` ne collisionne plus avec un segment de namespace. Mettre à jour tous les `using`.

### 3.2 `TvShow.Infrastructure` — adaptateur de persistance + DAO + mapping
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

### 3.3 `TvShow.Application` — ports & use cases propres
- **Port Out** `ITvShowRepository` : renvoie des entités de **domaine**
  (`IReadOnlyList<TvShow>`), `CancellationToken` en paramètre.
- **Port In** `IListTvShowsUseCase` : renvoie un **DTO applicatif** technologiquement neutre
  (ex. `ListTvShowsResult` / `TvShowSummary`), pas une entité de domaine ni un contrat HTTP.
- `ListTvShowsUseCase` (`internal sealed`) : mappe domaine → DTO applicatif et **propage le
  `CancellationToken`** jusqu'au repository.
- **Harmoniser** les types de collection (`IReadOnlyList<…>` partout par défaut).
- Retirer de cette couche tout ce qui est contrat HTTP (voir 3.4).

### 3.4 `TvShow.Api` — adaptateur web avec ses propres ViewModels
- **ViewModels d'API dédiés** dans `TvShow.Api/ViewModels` (ex. `TvShowViewModel`) : c'est le
  contrat HTTP de sortie, **distinct** du DTO applicatif. `TvShowResponse` quitte
  `TvShow.Application.Models` (le contrat web ne vit plus dans `Application`).
- **Mappers simples** dans `TvShow.Api/Mappers` : méthodes d'extension statiques, code de
  mapping **explicite** (pas de bibliothèque type AutoMapper) DTO applicatif → ViewModel.
- `TvShowsController` :
  - **route explicite** `[Route("api/tvshows")]` (plus de token `[controller]`, donc fin du
    `/api/TvShows` PascalCase) ;
  - renvoie `IReadOnlyList<TvShowViewModel>`, **mappe** le DTO applicatif → ViewModel ;
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

1. **Renommer les namespaces** vers une racine non ambiguë (`ProjectZero.TvShow.*`) et
   supprimer les alias `TvShowEntity`. Compiler.
2. **Purifier le domaine** : retirer DataAnnotations + navigation EF, encapsuler les entités
   (constructeurs/factories + invariants). Compiler.
3. **Créer `TvShowDao` + mapping** dans `Infrastructure` ; déplacer attributs/navigation EF
   vers les DAO. Adapter `InMemoryTvShowRepository` (DAO → domaine, `internal`, token).
   Compiler.
4. **Nettoyer Application** : port In renvoie un DTO applicatif, port Out renvoie le domaine,
   propager le token, harmoniser les collections. Compiler.
5. **Couche API** : créer les **ViewModels** (`TvShow.Api/ViewModels`) + **mappers simples**
   (`TvShow.Api/Mappers`), route **explicite** `api/tvshows`, le controller mappe DTO
   applicatif → ViewModel et **propage le token**. Compiler.
6. **OpenAPI** : retirer Scalar, générer un **`openapi.json` propre au build** (un seul
   content-type, route `/api/tvshows`) et vérifier l'endpoint.

---

## 5. Critères d'acceptation

- [ ] `dotnet build` réussit (0 erreur, 0 warning nouveau).
- [ ] `TvShow.Domain` ne référence **aucun** package/attribut framework
      (`grep` de `DataAnnotations` ⇒ 0 résultat dans `TvShow.Domain`).
- [ ] Plus aucun alias `using TvShowEntity = …` dans le code.
- [ ] `TvShowDao` existe dans `TvShow.Infrastructure` et un mapping DAO → domaine est en
      place ; les attributs `[Required]`/navigation EF n'existent **que** sur les DAO.
- [ ] Les entités de domaine sont encapsulées (pas de setter public nu ; invariants validés).
- [ ] Le `CancellationToken` est propagé de bout en bout (controller → use case → repository).
- [ ] Le **ViewModel** de sortie (`TvShowViewModel`) est défini dans `TvShow.Api/ViewModels`
      et mappé depuis le DTO applicatif via un mapper simple (`TvShow.Api/Mappers`) ; le port
      In renvoie un DTO applicatif ; le port Out renvoie des entités de domaine.
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
