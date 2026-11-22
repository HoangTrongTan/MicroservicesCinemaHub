namespace Movies.API.Showtimes.GetShowtimes;
public record GetShowtimesRequest(
    Guid? MovieId = null,
    Guid? CinemaId = null,
    DateOnly? Date = null);
public record GetShowtimesResponse(IEnumerable<ShowtimeDto> Showtimes);
public class GetShowtimesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/showtimes", async ([AsParameters] GetShowtimesRequest request, ISender sender) =>
        {
            var query = request.Adapt<GetShowtimesQuery>();
            var result = await sender.Send(query);
            var response = result.Adapt<GetShowtimesResponse>();

            return Results.Ok(response);
        })
        .WithName("GetShowtimes")
        .Produces<GetShowtimesResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get Showtimes")
        .WithDescription("Lấy danh sách suất chiếu theo phim, rạp và ngày (YYYY-MM-DD)");
    }
}