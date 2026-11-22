namespace Movies.API.Auditoriums.UpdateAuditorium;
public record UpdateAuditoriumCommand(
    Guid Id,
    string Name,
    List<SeatDefinition> SeatLayout) : ICommand<UpdateAuditoriumResult>, ICacheInvalidatorCommand
{
    string[] ICacheInvalidatorCommand.CacheKeys => [$"auditorium:{Id}"];
}
public record UpdateAuditoriumResult(bool IsSuccess);
public class UpdateAuditoriumCommandValidator : AbstractValidator<UpdateAuditoriumCommand>
{
    public UpdateAuditoriumCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Thiếu Id phòng chiếu cần sửa rồi sếp ơi!");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100).WithMessage("Tên phòng không được để trống!");
        RuleFor(x => x.SeatLayout).NotEmpty().WithMessage("Sửa kiểu gì mà dọn sạch ghế luôn vậy sếp? 😂");

        RuleForEach(x => x.SeatLayout).ChildRules(seat =>
        {
            seat.RuleFor(s => s.Row).NotEmpty().WithMessage("Thiếu tên hàng ghế (A, B, C...)!");
            seat.RuleFor(s => s.Column).GreaterThan(0).WithMessage("Số cột phải lớn hơn 0!");
            seat.RuleFor(s => s.PairSeatCode)
                .NotEmpty()
                .When(s => s.Type == SeatType.Couple)
                .WithMessage("Ghế Couple bắt buộc phải có PairSeatCode nha!");
        });
    }
}
internal class UpdateAuditoriumCommandHandler
    (IDocumentSession session)
    : ICommandHandler<UpdateAuditoriumCommand, UpdateAuditoriumResult>
{
    public async Task<UpdateAuditoriumResult> Handle(UpdateAuditoriumCommand command, CancellationToken cancellationToken)
    {
        var auditorium = await session.LoadAsync<Auditorium>(command.Id, cancellationToken);

        if (auditorium is null)
        {
            throw new AuditoriumNotFoundException(command.Id);
        }

        auditorium.Name = command.Name;
        auditorium.SeatLayout = command.SeatLayout;

        session.Update(auditorium);
        await session.SaveChangesAsync(cancellationToken);

        return new UpdateAuditoriumResult(true);
    }
}