namespace Movies.API.Showtimes.GetShowtimes;
public record GetShowtimesQuery(
    Guid? MovieId = null,
    Guid? CinemaId = null,
    DateOnly? Date = null) : IQuery<GetShowtimesResult>;
public record ShowtimeDto(
    Guid Id,
    Guid MovieId,
    string MovieTitle,
    Guid CinemaId,
    string CinemaName,
    Guid AuditoriumId,
    string AuditoriumName,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    decimal BasePrice,
    ShowtimeStatus Status,
    string PosterUrl);
public record GetShowtimesResult(IEnumerable<ShowtimeDto> Showtimes);
internal class GetShowtimesQueryHandler
    (IQuerySession session)
    : IQueryHandler<GetShowtimesQuery, GetShowtimesResult>
{
    public async Task<GetShowtimesResult> Handle(GetShowtimesQuery query, CancellationToken cancellationToken)
    {
        var movies = new Dictionary<Guid, Movie>();
        var cinemas = new Dictionary<Guid, Cinema>();
        var auditoriums = new Dictionary<Guid, Auditorium>();

        // chạy xong câu này nó sẽ lưu lại data vào 3 biến Dictionary trên rồi gọi details ở dưới.
        var queryable = session.Query<Showtime>()
            .Include(s => s.MovieId, movies)
            .Include(s => s.CinemaId, cinemas)
            .Include(s => s.AuditoriumId, auditoriums)
            .Where(s => s.Status == ShowtimeStatus.Scheduled);

        if (query.MovieId.HasValue)
        {
            queryable = queryable.Where(s => s.MovieId == query.MovieId.Value);
        } 

        if (query.CinemaId.HasValue)
        {
            queryable = queryable.Where(s => s.CinemaId == query.CinemaId.Value);
        }

        if (query.Date.HasValue)
        {
            // Ép sang DateTimeOffset UTC (00:00 -> 24:00) cho Marten/Npsql nuốt mượt
            var startOfDay = new DateTimeOffset(query.Date.Value.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
            var endOfDay = startOfDay.AddDays(1);

            queryable = queryable.Where(s => s.StartTime >= startOfDay && s.StartTime < endOfDay);
        }

        var showtimes = await queryable
            .OrderBy(s => s.StartTime)
            .ToListAsync(cancellationToken);

        var result = showtimes.Select(s =>
        {
            var movieDetails = movies.GetValueOrDefault(s.MovieId);
            return new ShowtimeDto(
                s.Id,
                s.MovieId,
                movieDetails?.Title ?? "",
                s.CinemaId,
                cinemas.GetValueOrDefault(s.CinemaId)?.Name ?? "",
                s.AuditoriumId,
                auditoriums.GetValueOrDefault(s.AuditoriumId)?.Name ?? "",
                s.StartTime,
                s.EndTime,
                s.BasePrice,
                s.Status,
                movieDetails?.PosterUrl ?? ""
            );
        });

        return new GetShowtimesResult(result);
    }
}