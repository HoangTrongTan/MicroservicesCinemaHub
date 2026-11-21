namespace Movies.API.Cinemas.UpdateCinema;
public record UpdateCinemaRequest(
    Guid Id,
    string Name,
    string Address,
    string City,
    string PhoneNumber,
    bool IsActive);
public record UpdateCinemaResponse(bool IsSuccess);
public class UpdateCinemaEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/cinemas", async (UpdateCinemaRequest request, ISender sender) => {
            
            var command = request.Adapt<UpdateCinemaCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<UpdateCinemaResponse>();

            return Results.Ok(response);
        })
        .WithName("UpdateCinema")
        .Produces<UpdateCinemaResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Update Cinema")
        .WithDescription("Cập nhật thông tin rạp chiếu phim và tự động xóa cache Redis");
    }
}