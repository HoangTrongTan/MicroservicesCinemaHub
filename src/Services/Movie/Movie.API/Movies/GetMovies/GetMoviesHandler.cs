namespace Movies.API.Movies.GetMovies;
public record GetMoviesQuery(
    string? Title, 
    string? Genre = null, 
    MovieStatus? Status = null, 
    int? PageNumber = 1, 
    int? PageSize = 10,
    DateOnly? ReleaseDateFrom = null,
    DateOnly? ReleaseDateTo = null,
    string? SortBy = null, 
    string? SortOrder = "desc") 
    : IQuery<GetMoviesResult>;
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
        IQueryable<Movie> moviesQuery = session.Query<Movie>();

        if (!string.IsNullOrWhiteSpace(query.Title))
        {
            var title = query.Title.Trim();
            moviesQuery = moviesQuery.Where(m => m.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query.Genre))
        {
            moviesQuery = moviesQuery.Where(m => m.Genres.Contains(query.Genre.Trim()));
        }

        if (query.Status.HasValue)
        {
            moviesQuery = moviesQuery.Where(m => m.Status == query.Status.Value);
        }

        if (query.ReleaseDateFrom.HasValue)
        {
            moviesQuery = moviesQuery.Where(m => m.ReleaseDate >= query.ReleaseDateFrom.Value);
        }

        if (query.ReleaseDateTo.HasValue)
        {
            moviesQuery = moviesQuery.Where(m => m.ReleaseDate <= query.ReleaseDateTo.Value);
        }

        var isDesc = string.Equals(query.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);

        moviesQuery = (query.SortBy?.Trim(), isDesc) switch
        {
            ("title", false) => moviesQuery.OrderBy(m => m.Title),
            ("title", true) => moviesQuery.OrderByDescending(m => m.Title),
            ("duration", false) => moviesQuery.OrderBy(m => m.DurationMinutes),
            ("duration", true) => moviesQuery.OrderByDescending(m => m.DurationMinutes),
            ("releasedate", false) => moviesQuery.OrderBy(m => m.ReleaseDate),
            _ => moviesQuery.OrderByDescending(m => m.ReleaseDate)
        };

        var movies = await moviesQuery
        .ToPagedListAsync(query.PageNumber ?? 1, query.PageSize ?? 10, cancellationToken);

        return new GetMoviesResult(movies, movies.TotalItemCount, movies.PageCount, movies.HasNextPage, movies.HasPreviousPage);
    }
}