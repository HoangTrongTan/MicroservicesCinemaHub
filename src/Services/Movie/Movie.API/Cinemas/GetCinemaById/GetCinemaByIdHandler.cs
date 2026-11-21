namespace Movies.API.Cinemas.GetCinemaById;
public record GetCinemaByIdQuery(Guid Id) : IQuery<GetCinemaByIdResult>, ICacheableQuery
{
    string ICacheableQuery.CacheKey => $"cinema:{Id}";
}
public record GetCinemaByIdResult(Cinema Cinema);
internal class GetCinemaByIdQueryHandler
    (IDocumentSession session)
    : IQueryHandler<GetCinemaByIdQuery, GetCinemaByIdResult>
{
    public async Task<GetCinemaByIdResult> Handle(GetCinemaByIdQuery query, CancellationToken cancellationToken)
    {
        var cinema = await session.LoadAsync<Cinema>(query.Id, cancellationToken);

        if (cinema is null)
        {
            throw new CinemaNotFoundException(query.Id);
        }

        return new GetCinemaByIdResult(cinema);
    }
}