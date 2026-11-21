namespace Movies.API.Cinemas.GetCinemaById;
public record GetCinemaByIdResponse(Cinema Cinema);
public class GetCinemaByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/cinemas/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetCinemaByIdQuery(id));

            var response = result.Adapt<GetCinemaByIdResponse>();

            return Results.Ok(response);
        })
        .WithName("GetCinemaById")
        .Produces<GetCinemaByIdResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get Cinema By Id")
        .WithDescription("Xem chi tiết 1 rạp chiếu phim (có bú Redis)");
    }
}