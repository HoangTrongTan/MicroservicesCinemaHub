namespace Movies.API.Auditoriums.UpdateAuditorium;
public record UpdateAuditoriumRequest(
    Guid Id,
    string Name,
    List<SeatDefinition> SeatLayout);
public record UpdateAuditoriumResponse(bool IsSuccess);
public class UpdateAuditoriumEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/auditoriums", async (UpdateAuditoriumRequest request, ISender sender) =>
        {
            var command = request.Adapt<UpdateAuditoriumCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<UpdateAuditoriumResponse>();

            return Results.Ok(response);
        })
        .WithName("UpdateAuditorium")
        .Produces<UpdateAuditoriumResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Update Auditorium")
        .WithDescription("Cập nhật tên phòng hoặc sơ đồ ghế và tự động xóa cache Redis");
    }
}