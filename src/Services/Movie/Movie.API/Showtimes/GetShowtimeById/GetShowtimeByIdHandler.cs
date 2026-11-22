namespace Movies.API.Showtimes.GetShowtimeById;
public record GetShowtimeByIdQuery(Guid Id) : IQuery<GetShowtimeByIdResult>, ICacheableQuery
{
    string ICacheableQuery.CacheKey => $"showtime:{Id}";
    TimeSpan? ICacheableQuery.Expiration => TimeSpan.FromDays(1);
}
public record ShowtimeDetailDto(
    Guid Id,
    Guid MovieId,
    string MovieTitle,
    string? PosterUrl,
    Guid CinemaId,
    string CinemaName,
    Guid AuditoriumId,
    string AuditoriumName,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    decimal BasePrice,
    ShowtimeStatus Status);
public record GetShowtimeByIdResult(ShowtimeDetailDto Showtime);
internal class GetShowtimeByIdHandler
    (IQuerySession session)
    : IQueryHandler<GetShowtimeByIdQuery, GetShowtimeByIdResult>
{
    public async Task<GetShowtimeByIdResult> Handle(GetShowtimeByIdQuery query, CancellationToken cancellationToken)
    {
        Movie? movie = null;
        Cinema? cinema = null;
        Auditorium? auditorium = null;

        var showtime = await session.Query<Showtime>()
            .Include<Movie>(s => s.MovieId, m => movie = m)
            .Include<Cinema>(s => s.CinemaId, c => cinema = c)
            .Include<Auditorium>(s => s.AuditoriumId, a => auditorium = a)
            .FirstOrDefaultAsync(s => s.Id == query.Id, cancellationToken)
            ?? throw new ShowtimeNotFoundException(query.Id);

        var dto = new ShowtimeDetailDto(
            showtime.Id,
            showtime.MovieId,
            movie?.Title ?? "Unknown",
            movie?.PosterUrl,
            showtime.CinemaId,
            cinema?.Name ?? "Unknown",
            showtime.AuditoriumId,
            auditorium?.Name ?? "Unknown",
            showtime.StartTime,
            showtime.EndTime,
            showtime.BasePrice,
            showtime.Status);

        return new GetShowtimeByIdResult(dto);
    }
}
