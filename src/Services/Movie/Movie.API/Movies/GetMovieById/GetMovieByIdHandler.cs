namespace Movies.API.Movies.GetMovieById;
public record GetMovieByIdQuery(Guid Id) : IQuery<GetMovieByIdResult>;
public record GetMovieByIdResult(Movie Movie);
public class GetMovieByIdQueryHandler
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