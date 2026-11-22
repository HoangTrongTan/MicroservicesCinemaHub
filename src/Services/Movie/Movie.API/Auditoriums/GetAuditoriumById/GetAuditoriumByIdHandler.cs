namespace Movies.API.Auditoriums.GetAuditoriumById;
public record GetAuditoriumByIdQuery(Guid Id) : IQuery<GetAuditoriumByIdResult>, ICacheableQuery
{
    string ICacheableQuery.CacheKey => $"auditorium:{Id}";
    TimeSpan? ICacheableQuery.Expiration => TimeSpan.FromDays(7);
}

public record GetAuditoriumByIdResult(Auditorium Auditorium);
internal class GetAuditoriumByIdQueryHandler
    (IQuerySession session)
    : IQueryHandler<GetAuditoriumByIdQuery, GetAuditoriumByIdResult>
{
    public async Task<GetAuditoriumByIdResult> Handle(GetAuditoriumByIdQuery query, CancellationToken cancellationToken)
    {
        var auditorium = await session.LoadAsync<Auditorium>(query.Id, cancellationToken);

        if (auditorium is null)
        {
            throw new AuditoriumNotFoundException(query.Id);
        }

        return new GetAuditoriumByIdResult(auditorium);
    }
}