namespace Movies.API.Cinemas.UpdateCinema;
public record UpdateCinemaCommand(
    Guid Id,
    string Name,
    string Address,
    string City,
    string PhoneNumber,
    bool IsActive) : ICommand<UpdateCinemaResult>, ICacheInvalidatorCommand
{
    string[] ICacheInvalidatorCommand.CacheKeys => [$"cinema:{Id}"];
}
public record UpdateCinemaResult(bool IsSuccess);
public class UpdateCinemaCommandValidator : AbstractValidator<UpdateCinemaCommand>
{
    public UpdateCinemaCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Thiếu Id rạp rồi sếp ơi!");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150).WithMessage("Tên rạp không được để trống!");
        RuleFor(x => x.Address).NotEmpty().WithMessage("Địa chỉ rạp không được bỏ trống!");
        RuleFor(x => x.City).NotEmpty().WithMessage("Thành phố không được để trống!");
        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^\d{9,11}$").WithMessage("Số hotline phải từ 9-11 số nha.");
    }
}
internal class UpdateCinemaCommandHandler
    (IDocumentSession session)
    : ICommandHandler<UpdateCinemaCommand, UpdateCinemaResult>
{
    public async Task<UpdateCinemaResult> Handle(UpdateCinemaCommand command, CancellationToken cancellationToken)
    {
        var cinema = await session.LoadAsync<Cinema>(command.Id, cancellationToken);

        if (cinema is null)
        {
            throw new CinemaNotFoundException(command.Id);
        }

        cinema.Name = command.Name;
        cinema.Address = command.Address;
        cinema.City = command.City;
        cinema.PhoneNumber = command.PhoneNumber;
        cinema.IsActive = command.IsActive;

        session.Update(cinema);

        await session.SaveChangesAsync();

        return new UpdateCinemaResult(true);
    }
}