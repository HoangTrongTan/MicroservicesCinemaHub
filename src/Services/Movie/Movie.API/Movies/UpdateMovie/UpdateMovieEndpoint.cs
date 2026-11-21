namespace Movies.API.Movies.UpdateMovie;
public record UpdateMovieRequest(
    Guid Id,
    string Title,
    List<string> Genres,
    int DurationMinutes,
    AgeRating AgeRating,
    DateOnly ReleaseDate,
    string PosterUrl,
    MovieStatus Status);
public record UpdateMovieResponse(bool IsSuccess);
public class UpdateMovieEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/movies", async (UpdateMovieRequest request, ISender sender) =>
        {
            var command = request.Adapt<UpdateMovieCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<UpdateMovieResponse>();

            return Results.Ok(response);
        })
        .WithName("UpdateMovie")
        .Produces<UpdateMovieResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Update Movie")
        .WithDescription("Update Movie");
    }
}