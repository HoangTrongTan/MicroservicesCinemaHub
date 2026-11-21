namespace Movies.API.Movies.GetMovieById;
public record GetMovieByIdResponse(Movie Movie);
public class GetMovieByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/movies/{id}", async (Guid Id, ISender sender) =>
        {
            var result = await sender.Send(new GetMovieByIdQuery(Id));

            var response = result.Adapt<GetMovieByIdResponse>();

            return Results.Ok(response);
        })
        .WithName("Get Movie By Id Request")
        .Produces<GetMovieByIdResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithDescription("Get Movie By Id")
        .WithSummary("Get Movie By Id");
    }
}