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
            //Cinema
            config.Schema.For<Cinema>().UniqueIndex(x => x.Name, x => x.City);
            //Auditorium
            config.Schema.For<Auditorium>().UniqueIndex(x => x.CinemaId, x => x.Name);
        }).UseLightweightSessions();

        return services;
    }
}