namespace Movies.API.Showtimes.CancelShowtime;
public record CancelShowtimeCommand(Guid Id) : ICommand<CancelShowtimeResult>, ICacheInvalidatorCommand
{
    string[] ICacheInvalidatorCommand.CacheKeys => [$"showtime:{Id}"];
}
public record CancelShowtimeResult(bool IsSuccess);
public class CancelShowtimeCommandValidator : AbstractValidator<CancelShowtimeCommand>
{
    public CancelShowtimeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Thiếu Id suất chiếu cần hủy rồi sếp ơi!");
    }
}
internal class CancelShowtimeCommandHandler
    (IDocumentSession session)
    : ICommandHandler<CancelShowtimeCommand, CancelShowtimeResult>
{
    public async Task<CancelShowtimeResult> Handle(CancelShowtimeCommand command, CancellationToken cancellationToken)
    {
        var showtime = await session.LoadAsync<Showtime>(command.Id, cancellationToken)
            ?? throw new ShowtimeNotFoundException(command.Id);

        if (showtime.Status == ShowtimeStatus.Cancelled)
        {
            throw new BadRequestException("Suất chiếu này đã bị hủy trước đó rồi, hủy gì nữa sếp! 😂");
        }

        if (showtime.StartTime <= DateTime.UtcNow)
        {
            throw new BadRequestException("Phim chiếu rồi (hoặc chiếu xong rồi) thì không thể hủy suất được nha!");
        }

        showtime.Status = ShowtimeStatus.Cancelled;

        session.Update(showtime);

        await session.SaveChangesAsync(cancellationToken);

        return new CancelShowtimeResult(true);
    }
}