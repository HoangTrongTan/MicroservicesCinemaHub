namespace Movies.API.Showtimes.CreateShowtime;
public record CreateShowtimeRequest(
    Guid MovieId,
    Guid AuditoriumId,
    DateTimeOffset StartTime,
    decimal BasePrice);
public record CreateShowtimeResponse(Guid Id);
public class CreateShowtimeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/showtimes", async (CreateShowtimeRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateShowtimeCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<CreateShowtimeResponse>();

            return Results.Created($"/showtimes/{response.Id}", response);
        })
        .WithName("CreateShowtime")
        .Produces<CreateShowtimeResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Create Showtime")
        .WithDescription("Tạo suất chiếu mới (tự tính 15p dọn dẹp & chống trùng lịch)");
    }
}