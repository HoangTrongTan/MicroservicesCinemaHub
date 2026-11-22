namespace Movies.API.Auditoriums.CreateAuditorium;
public record CreateAuditoriumRequest(
    Guid CinemaId,
    string Name,
    List<SeatDefinition> SeatLayout);
public record CreateAuditoriumResponse(Guid Id);
public class CreateAuditoriumEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/auditoriums", async (CreateAuditoriumRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateAuditoriumCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<CreateAuditoriumResponse>();

            return Results.Created($"/auditoriums/{response.Id}", response);
        })
        .WithName("CreateAuditorium")
        .Produces<CreateAuditoriumResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Create Auditorium")
        .WithDescription("Tạo phòng chiếu mới kèm sơ đồ ghế");
    }
}