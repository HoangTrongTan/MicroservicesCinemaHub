namespace Movies.API.Movies.GetMovies;
public record GetMoviesRequest(
    string? Title,
    string? Genre = null,
    MovieStatus? Status = null,
    int? PageNumber = 1,
    int? PageSize = 10,
    DateOnly? ReleaseDateFrom = null,
    DateOnly? ReleaseDateTo = null,
    string? SortBy = null,
    string? SortOrder = "desc");
public record GetMoviesResponse(IEnumerable<Movie> Movies, long TotalCount, long TotalPages, bool HasNextPage, bool HasPreviousPage);
public class GetMoviesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/movies", async ([AsParameters] GetMoviesRequest request, ISender sender) =>
        {
            var query = request.Adapt<GetMoviesQuery>();

            var results = await sender.Send(query);

            var response = results.Adapt<GetMoviesResponse>();

            return Results.Ok(response);
        })
        .WithName("Get Movies")
        .Produces<GetMoviesResponse>(StatusCodes.Status200OK)
        .WithSummary("Get Movies")
        .WithDescription("Get Movies");
    }
}