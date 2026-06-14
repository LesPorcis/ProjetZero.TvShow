using ProjectZero.TvShows.Api;

namespace ProjectZero.Web.ServiceCollectionExtensions;

internal static class BusinessExtensions
{
    public static IServiceCollection AddModules(this IServiceCollection services) =>
        services.AddTvShowsModule();
}