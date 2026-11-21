namespace Movies.API.Cinemas.GetCinemas;
public record GetCinemasQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? City = null) : IQuery<GetCinemasResult>;
public record GetCinemasResult(
    IEnumerable<Cinema> Cinemas,
    long TotalCount,
    int PageNumber,
    int PageSize);
internal class GetCinemasQueryHandler
    (IDocumentSession session)
    : IQueryHandler<GetCinemasQuery, GetCinemasResult>
{
    public async Task<GetCinemasResult> Handle(GetCinemasQuery query, CancellationToken cancellationToken)
    {
        var queryable = session.Query<Cinema>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.City))
        {
            queryable = queryable.Where(c => c.City.Contains(query.City, StringComparison.OrdinalIgnoreCase));
        }

        var cinemas = await queryable
            .OrderBy(c => c.Name)
            .ToPagedListAsync(query.PageNumber, query.PageSize, cancellationToken);

        return new GetCinemasResult(
            cinemas,
            cinemas.TotalItemCount,
            query.PageNumber,
            query.PageSize);
    }
}