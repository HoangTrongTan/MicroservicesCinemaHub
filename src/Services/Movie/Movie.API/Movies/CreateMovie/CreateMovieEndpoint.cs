namespace Movies.API.Movies.CreateMovie;
public record CreateMovieRequest
    (string Title, List<string> Genres, int DurationMinutes, AgeRating AgeRating, DateOnly ReleaseDate, string PosterUrl, MovieStatus Status);
public record CreateMovieResponse(Guid Id);
public class CreateMovieEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/movie", async (CreateMovieRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateMovieCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<CreateMovieResponse>();

            return Results.Created($"/movie/{response.Id}", response);
        })
        .WithName("CreateMovie")
        .Produces<CreateMovieResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("CreateMovie")
        .WithDescription("CreateMovie");
    }
}