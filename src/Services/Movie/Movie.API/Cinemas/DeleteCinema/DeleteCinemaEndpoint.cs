namespace Movies.API.Cinemas.DeleteCinema;
public record DeleteCinemaResponse(bool IsSuccess);
public class DeleteCinemaEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/cinemas/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteCinemaCommand(id));

            var response = result.Adapt<DeleteCinemaResponse>();

            return Results.Ok(response);
        })
        .WithName("DeleteCinema")
        .Produces<DeleteCinemaResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Delete Cinema")
        .WithDescription("Xóa rạp chiếu phim và tiễn luôn cache Redis bay màu");
    }
}