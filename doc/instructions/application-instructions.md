# Instructions — `ProjectZero.TvShows.Application`

> Conventions impératives pour les cas d'usage et les ports (hexagone). Le code fait foi.
> Adaptateurs associés : [`api-instructions.md`](api-instructions.md) (primaire),
> [`infrastructure-instructions.md`](infrastructure-instructions.md) (pilotés).

## Rôle

Orchestration métier : **cas d'usage** + **ports** (entrants et sortants) qui définissent
la frontière de l'hexagone.

## Dépendances

`Domain` uniquement. Package : `Microsoft.Extensions.DependencyInjection.Abstractions`
(pour `AddApplication`).

## Conventions

- **Ports In** (`Ports/In/`) : interfaces `public` pilotant l'appli, ex. `ITvShowsCatalog`.
- **Ports Out** (`Ports/Out/`) : interfaces `public` des dépendances externes, ex. `ITvShowsRepository`.
- **Cas d'usage** (`UseCases/`) : `internal sealed`, implémentent un port In et dépendent des
  ports Out (constructeur primaire). Ex. `TvShowsCatalog`.
- Surface `public` = **ports + `AddApplication()`**. Le reste est `internal`.
- Types échangés = **entités de `Domain`**. `CancellationToken` propagé, défaut `= default`.

## À ne pas faire

- Aucune référence à `Infrastructure`, `Database` ou `Api`.
- Aucun détail technique (EF, SQL, HTTP, ViewModel, entité de persistance).

## Points d'entrée

- **Nouveau cas d'usage** : port In dans `Ports/In/`, implémentation `internal sealed` dans
  `UseCases/`, enregistrée dans `AddApplication`.
- **Nouvelle dépendance externe** : port Out dans `Ports/Out/` (implémenté côté `Infrastructure`).
