namespace Movies.API.Auditoriums.DeleteAuditorium;
public record DeleteAuditoriumResponse(bool IsSuccess);
public class DeleteAuditoriumEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/auditoriums/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteAuditoriumCommand(id));
            var response = result.Adapt<DeleteAuditoriumResponse>();

            return Results.Ok(response);
        })
        .WithName("DeleteAuditorium")
        .Produces<DeleteAuditoriumResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Delete Auditorium")
        .WithDescription("Xóa phòng chiếu và tiễn luôn cache Redis bay màu");
    }
}