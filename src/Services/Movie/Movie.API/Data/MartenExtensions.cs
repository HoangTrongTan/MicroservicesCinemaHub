namespace Movies.API.Data;

public static class MartenExtensions
{
    public static IServiceCollection AddMartenDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMarten(config =>
        {
            config.Connection(configuration.GetConnectionString("Database")!);
            
            config.DatabaseSchemaName = "movies";

            // Movies
            config.Schema.For<Movie>().UniqueIndex(x => x.Title, x => x.ReleaseDate);
        }).UseLightweightSessions();

        return services;
    }
}