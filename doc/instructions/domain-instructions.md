# Instructions — `ProjectZero.TvShows.Domain`

> Conventions impératives pour le cœur métier. Le code fait foi.

## Rôle

Cœur de l'hexagone : les **entités métier** pures (`TvShow`, `Director`, `Writer`,
`Star`, `Genre`). Aucune préoccupation technique.

## Dépendances

Aucune. Projet **feuille** : pas de référence projet, pas de package NuGet.

## Conventions

- Entités `public sealed` à **constructeur primaire**, propriétés en **lecture seule**
  (`get` only, valorisées par le constructeur).
- Collections exposées en `IReadOnlyCollection<>`.
- C# pur : rien d'EF, de DI, de sérialisation ni de validation framework.

## À ne pas faire

- Aucun attribut de persistance/JSON, aucune navigation EF (ça vit sur les entités de
  persistance, projet `ProjectZero.Database`).
- Aucune référence à `Application`, `Infrastructure`, `Database` ou `Api`.
- Aucune logique d'accès aux données ni d'I/O.

## Points d'entrée

- **Nouvelle entité métier** : classe `public sealed` + constructeur primaire, propriétés
  `get` seules ; collections en `IReadOnlyCollection<>`.
