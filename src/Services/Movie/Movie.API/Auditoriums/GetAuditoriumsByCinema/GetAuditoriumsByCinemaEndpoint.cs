namespace Movies.API.Auditoriums.GetAuditoriumsByCinema;
public record GetAuditoriumsByCinemaResponse(IEnumerable<Auditorium> Auditoriums);
public class GetAuditoriumsByCinemaEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/cinemas/{cinemaId:guid}/auditoriums", async (Guid cinemaId, ISender sender) =>
        {
            var result = await sender.Send(new GetAuditoriumsByCinemaQuery(cinemaId));

            var response = result.Adapt<GetAuditoriumsByCinemaResponse>();

            return Results.Ok(response);
        })
        .WithName("Get Auditoriums By Cinema")
        .Produces<GetAuditoriumsByCinemaResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get Auditoriums By Cinema")
        .WithDescription("Lấy toàn bộ danh sách phòng chiếu của một rạp");
    }
}