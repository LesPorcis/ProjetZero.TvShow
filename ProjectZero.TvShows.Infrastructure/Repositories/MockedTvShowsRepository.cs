using ProjectZero.TvShows.Application.Ports.Out;
using ProjectZero.TvShows.Domain;

namespace ProjectZero.TvShows.Infrastructure.Repositories;

internal sealed class MockedTvShowsRepository : ITvShowsRepository
{
    private static readonly IReadOnlyCollection<TvShow> TvShows =
    [
        new(1, "Breaking Bad", new DateOnly(2008, 1, 20), 5, 62,
            directors:
            [
                new Director(1, "Vince", "Gilligan")
            ],
            writers:
            [
                new Writer(1, "Vince", "Gilligan"),
                new Writer(2, "Peter", "Gould")
            ],
            stars:
            [
                new Star(1, "Bryan", "Cranston"),
                new Star(2, "Aaron", "Paul"),
                new Star(3, "Anna", "Gunn")
            ],
            genres:
            [
                new Genre(1, "Crime", "Récits centrés sur des actes criminels et leurs conséquences."),
                new Genre(2, "Drame", "Intrigues sérieuses portées sur l'évolution des personnages."),
                new Genre(3, "Thriller", "Tension narrative entretenant le suspense.")
            ]),
        new(2, "The Last of Us", new DateOnly(2023, 1, 15), 2, 16,
            directors:
            [
                new Director(2, "Craig", "Mazin"),
                new Director(3, "Neil", "Druckmann")
            ],
            writers:
            [
                new Writer(3, "Craig", "Mazin"),
                new Writer(4, "Neil", "Druckmann")
            ],
            stars:
            [
                new Star(4, "Pedro", "Pascal"),
                new Star(5, "Bella", "Ramsey")
            ],
            genres:
            [
                new Genre(2, "Drame", "Intrigues sérieuses portées sur l'évolution des personnages."),
                new Genre(4, "Action", "Séquences rythmées mêlant affrontements et péripéties."),
                new Genre(5, "Aventure", "Voyages et explorations dans un monde hostile.")
            ])
    ];

    public Task<IReadOnlyCollection<TvShow>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(TvShows);
    }
}
