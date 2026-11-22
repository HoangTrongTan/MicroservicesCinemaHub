namespace Movies.API.Auditoriums.GetAuditoriumsByCinema;
public record GetAuditoriumsByCinemaQuery(Guid CinemaId) : IQuery<GetAuditoriumsByCinemaResult>;
public record GetAuditoriumsByCinemaResult(IEnumerable<Auditorium> Auditoriums);
public class GetAuditoriumsByCinemaQueryHandler
    (IQuerySession session)
    : IQueryHandler<GetAuditoriumsByCinemaQuery, GetAuditoriumsByCinemaResult>
{
    public async Task<GetAuditoriumsByCinemaResult> Handle(GetAuditoriumsByCinemaQuery query, CancellationToken cancellationToken)
    {
        var cinemaExists = await session.Query<Cinema>()
            .AnyAsync(c => c.Id == query.CinemaId, cancellationToken);

        if (!cinemaExists)
        {
            throw new CinemaNotFoundException(query.CinemaId);
        }

        var auditoriums = await session.Query<Auditorium>()
            .Where(a => a.CinemaId == query.CinemaId)
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);

        return new GetAuditoriumsByCinemaResult(auditoriums);
    }
}