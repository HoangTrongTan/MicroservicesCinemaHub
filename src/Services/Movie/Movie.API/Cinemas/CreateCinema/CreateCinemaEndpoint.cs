namespace Movies.API.Cinemas.CreateCinema;
public record CreateCinemaRequest(
    string Name,
    string Address,
    string City,
    string PhoneNumber);
public record CreateCinemaResponse(Guid Id);
public class CreateCinemaEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/cinemas", async (CreateCinemaRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateCinemaCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<CreateCinemaResponse>();

            return Results.Created($"/cinemas/{response.Id}", response);
        })
        .WithName("Create Cinema")
        .Produces<CreateCinemaResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Create Cinema")
        .WithDescription("Khai trương rạp chiếu phim mới");
    }
}