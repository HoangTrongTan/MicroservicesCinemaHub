namespace Movies.API.Showtimes.CreateShowtime;
public record CreateShowtimeCommand(
    Guid MovieId,
    Guid AuditoriumId,
    DateTimeOffset StartTime,
    decimal BasePrice) : ICommand<CreateShowtimeResult>;
public record CreateShowtimeResult(Guid Id);
public class CreateShowtimeCommandValidator : AbstractValidator<CreateShowtimeCommand>
{
    public CreateShowtimeCommandValidator()
    {
        RuleFor(x => x.MovieId).NotEmpty().WithMessage("Thiếu MovieId rồi sếp ơi!");
        RuleFor(x => x.AuditoriumId).NotEmpty().WithMessage("Chiếu ngoài trời hay sao mà thiếu AuditoriumId? 😂");
        RuleFor(x => x.StartTime)
            .GreaterThan(DateTimeOffset.UtcNow)
            .WithMessage("Không thể tạo suất chiếu trong quá khứ (nhớ truyền giờ UTC nha)!");
        RuleFor(x => x.BasePrice)
            .GreaterThan(0)
            .WithMessage("Giá vé phải lớn hơn 0, chiếu miễn phí lấy gì trả tiền điện!");
    }
}
internal class CreateShowtimeCommandHandler
    (IDocumentSession session)
    : ICommandHandler<CreateShowtimeCommand, CreateShowtimeResult>
{
    private const int CleaningBufferMinutes = 15;
    public async Task<CreateShowtimeResult> Handle(CreateShowtimeCommand command, CancellationToken cancellationToken)
    {
        // 1.Check Movie tồn tại chưa & chưa Ended
        var movie = await session.LoadAsync<Movie>(command.MovieId, cancellationToken)
            ?? throw new MovieNotFoundException(command.MovieId);

        if (movie.Status == MovieStatus.Ended)
        {
            throw new BadRequestException($"Phim '{movie.Title}' đã ngừng chiếu (Ended), không thể xếp lịch!");
        }

        // 2.Check Auditorium tồn tại (tiện tay hốt luôn CinemaId)
        var auditorium = await session.LoadAsync<Auditorium>(command.AuditoriumId, cancellationToken)
            ?? throw new AuditoriumNotFoundException(command.AuditoriumId);

        // 3. Tính giờ kết thúc = StartTime + Thời lượng phim + 15ph rọn rác 🧹
        var startTimeUtc = command.StartTime.ToUniversalTime();

        var endTimeUtc = startTimeUtc.AddMinutes(movie.DurationMinutes + CleaningBufferMinutes);

        // 4. Check trùng giờ trong cùng phòng chiếu (chỉ tính suất đang Scheduled)
        var hasOverlap = await session.Query<Showtime>()
                .AnyAsync(s =>
                    s.AuditoriumId == command.AuditoriumId &&
                    s.Status == ShowtimeStatus.Scheduled &&
                    s.StartTime < endTimeUtc &&
                    s.EndTime > startTimeUtc,
                    cancellationToken);

        if (hasOverlap)
        {
            throw new BadRequestException(
                $"Phòng '{auditorium.Name}' bị trùng lịch trong khung giờ {startTimeUtc:HH:mm} - {endTimeUtc:HH:mm} UTC rồi sếp ơi!");
        }

        var showtime = new Showtime
        {
            AuditoriumId = auditorium.Id,
            MovieId = movie.Id,
            CinemaId = auditorium.CinemaId,
            StartTime = startTimeUtc,
            EndTime = endTimeUtc,
            BasePrice = command.BasePrice,
            Status = ShowtimeStatus.Scheduled
        };

        session.Store(showtime);

        await session.SaveChangesAsync(cancellationToken);

        return new CreateShowtimeResult(showtime.Id);
    }
}