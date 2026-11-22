namespace Movies.API.Auditoriums.GetAuditoriumById;
public record GetAuditoriumByIdResponse(Auditorium Auditorium);
public class GetAuditoriumByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/auditoriums/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetAuditoriumByIdQuery(id));

            var response = result.Adapt<GetAuditoriumByIdResponse>();

            return Results.Ok(response);
        })
        .WithName("Get Auditorium By Id")
        .Produces<GetAuditoriumByIdResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get Auditorium By Id")
        .WithDescription("Xem chi tiết phòng chiếu và sơ đồ ghế (bú Redis)");
    }
}