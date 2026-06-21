using ProjectZero.Database;

namespace ProjectZero.Web.ServiceCollectionExtensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration) =>
        services.AddEfPostgreSql(configuration);
}