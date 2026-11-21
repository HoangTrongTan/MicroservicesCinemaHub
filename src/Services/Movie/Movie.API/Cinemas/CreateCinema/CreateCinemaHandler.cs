namespace Movies.API.Cinemas.CreateCinema;
public record CreateCinemaCommand(
    string Name,
    string Address,
    string City,
    string PhoneNumber) : ICommand<CreateCinemaResult>;
public record CreateCinemaResult(Guid Id);
public class CreateCinemaCommandValidator : AbstractValidator<CreateCinemaCommand>
{
    public CreateCinemaCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150).WithMessage("Tên rạp không được để trống!");
        RuleFor(x => x.Address).NotEmpty().WithMessage("Địa chỉ rạp đâu sếp ơi?");
        RuleFor(x => x.City).NotEmpty().WithMessage("Phải có thành phố mới biết đường đi xem chứ!");
        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^\d{9,11}$").WithMessage("Số hotline phải từ 9-11 số nha.");
    }
}
internal class CreateCinemaCommandHandler
    (IDocumentSession session)
    : ICommandHandler<CreateCinemaCommand, CreateCinemaResult>
{
    public async Task<CreateCinemaResult> Handle(CreateCinemaCommand command, CancellationToken cancellationToken)
    {
        var cinema = new Cinema
        {
            Name = command.Name,
            Address = command.Address,
            City = command.City,
            PhoneNumber = command.PhoneNumber,
        };

        session.Store(cinema);

        await session.SaveChangesAsync(cancellationToken);

        return new CreateCinemaResult(cinema.Id);
    }
}