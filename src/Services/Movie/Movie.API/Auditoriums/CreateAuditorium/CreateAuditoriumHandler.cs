namespace Movies.API.Auditoriums.CreateAuditorium;
public record CreateAuditoriumCommand(
    Guid CinemaId,
    string Name,
    List<SeatDefinition> SeatLayout) : ICommand<CreateAuditoriumResult>;
public record CreateAuditoriumResult(Guid Id);
public class CreateAuditoriumCommandValidator : AbstractValidator<CreateAuditoriumCommand>
{
    public CreateAuditoriumCommandValidator()
    {
        RuleFor(x => x.CinemaId).NotEmpty().WithMessage("Phòng chiếu phải thuộc về 1 rạp nào đó chứ sếp!");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100).WithMessage("Tên phòng không được để trống!");
        RuleFor(x => x.SeatLayout).NotEmpty().WithMessage("Phòng chiếu gì mà không có cái ghế nào vậy? Bắt khách đứng xem à? 😂");
        RuleForEach(x => x.SeatLayout).ChildRules(seat =>
        {
            seat.RuleFor(s => s.Row).NotEmpty().WithMessage("Thiếu tên hàng ghế (A, B, C...)!");
            seat.RuleFor(s => s.Column).GreaterThan(0).WithMessage("Số cột phải lớn hơn 0!");
            seat.RuleFor(s => s.PairSeatCode)
                .NotEmpty()
                .When(s => s.Type == SeatType.Couple)
                .WithMessage("Ghế Couple thì phải có PairSeatCode để còn khóa cặp nha!");
        });
    }
}
internal class CreateAuditoriumCommandHandler
    (IDocumentSession session)
    : ICommandHandler<CreateAuditoriumCommand, CreateAuditoriumResult>
{
    public async Task<CreateAuditoriumResult> Handle(CreateAuditoriumCommand command, CancellationToken cancellationToken)
    {
        var cinemaExits = await session.Query<Cinema>()
            .AnyAsync(c => c.Id == command.CinemaId, cancellationToken);

        if (!cinemaExits)
        {
            throw new CinemaNotFoundException(command.CinemaId);
        }

        var auditorium = new Auditorium
        {
            CinemaId = command.CinemaId,
            Name = command.Name,
            SeatLayout = command.SeatLayout
        };

        session.Store(auditorium);

        await session.SaveChangesAsync(cancellationToken);

        return new CreateAuditoriumResult(auditorium.Id);
    }
}