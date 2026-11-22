
namespace Movies.API.Showtimes.GetShowtimeById;
public record GetShowtimeByIdResponse(ShowtimeDetailDto Showtime);
public class GetShowtimeByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/showtimes/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetShowtimeByIdQuery(id));
            var response = result.Adapt<GetShowtimeByIdResponse>();

            return Results.Ok(response);
        })
        .WithName("GetShowtimeById")
        .Produces<GetShowtimeByIdResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get Showtime By Id")
        .WithDescription("Xem chi tiết suất chiếu kèm thông tin phim, rạp, phòng (bú Redis)");
    }
}