namespace Movies.API.Movies.GetMovieById;
public record GetMovieByIdQuery(Guid Id) : IQuery<GetMovieByIdResult>, ICacheableQuery
{
    string ICacheableQuery.CacheKey => $"movie:{Id}";
    TimeSpan? ICacheableQuery.Expiration => TimeSpan.FromDays(7);
}

public record GetMovieByIdResult(Movie Movie);
internal class GetMovieByIdQueryHandler
    (IQuerySession session)
    : IQueryHandler<GetMovieByIdQuery, GetMovieByIdResult>
{
    public async Task<GetMovieByIdResult> Handle(GetMovieByIdQuery query, CancellationToken cancellationToken)
    {
        var movie = await session.LoadAsync<Movie>(query.Id, cancellationToken);

        if (movie is null)
        {
            throw new MovieNotFoundException(query.Id);
        }

        return new GetMovieByIdResult(movie);
    }
}