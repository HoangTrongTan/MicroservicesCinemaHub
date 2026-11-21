namespace Movies.API.Cinemas.GetCinemas;
public record GetCinemasRequest(
    int PageNumber = 1,
    int PageSize = 10,
    string? City = null);
public record GetCinemasResponse(
    IEnumerable<Cinema> Cinemas,
    long TotalCount,
    int PageNumber,
    int PageSize);
public class GetCinemasEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/cinemas", async ([AsParameters] GetCinemasRequest request, ISender sender) =>
        {
            var query = request.Adapt<GetCinemasQuery>();

            var result = await sender.Send(query);

            var response = result.Adapt<GetCinemasResponse>();

            return Results.Ok(response);
        })
        .WithName("GetCinemas")
        .Produces<GetCinemasResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get Cinemas")
        .WithDescription("Lấy danh sách rạp chiếu phim (có phân trang & lọc theo thành phố)");
    }
}
