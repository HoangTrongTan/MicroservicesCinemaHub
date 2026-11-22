namespace Movies.API.Showtimes.CancelShowtime;
public record CancelShowtimeResponse(bool IsSuccess);
public class CancelShowtimeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/showtimes/{id:guid}/cancel", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new CancelShowtimeCommand(id));
            var response = result.Adapt<CancelShowtimeResponse>();

            return Results.Ok(response);
        })
        .WithName("CancelShowtime")
        .Produces<CancelShowtimeResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Cancel Showtime")
        .WithDescription("Hủy suất chiếu (chuyển trạng thái sang Cancelled và xóa cache Redis)");
    }
}