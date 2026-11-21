namespace Movies.API.Movies.GetMovies;
public record GetMoviesQuery(int? PageNumber = 1, int? PageSize = 10) : IQuery<GetMoviesResult>;
public record GetMoviesResult(IEnumerable<Movie> Movies, long TotalCount, long TotalPages, bool HasNextPage, bool HasPreviousPage);
public class GetMoviesValidator : AbstractValidator<GetMoviesQuery>
{
    public GetMoviesValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("Số trang phải lớn hơn hoặc bằng 1");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Số lượng phim mỗi trang phải từ 1 đến 100");
    }
}
internal class GetMoviesQueryHandler
    (IQuerySession session)
    : IQueryHandler<GetMoviesQuery, GetMoviesResult>
{
    public async Task<GetMoviesResult> Handle(GetMoviesQuery query, CancellationToken cancellationToken)
    {
        var movies = await session.Query<Movie>()
            .OrderByDescending(m => m.ReleaseDate)
            .ToPagedListAsync(query.PageNumber ?? 1, query.PageSize ?? 10, cancellationToken);

        return new GetMoviesResult(movies, movies.TotalItemCount, movies.PageCount, movies.HasNextPage, movies.HasPreviousPage);
    }
}