namespace Movies.API.Movies.UpdateMovie;
public record UpdateMovieCommand(
    Guid Id,
    string Title,
    List<string> Genres,
    int DurationMinutes,
    AgeRating AgeRating,
    DateOnly ReleaseDate,
    string PosterUrl,
    MovieStatus Status) : ICommand<UpdateMovieResult>, ICacheInvalidatorCommand
{
    public string[] CacheKeys => [$"movie:{Id}"];
}

public record UpdateMovieResult(bool IsSuccess);
internal class UpdateMovieCommandHandler
    (IDocumentSession session)
    : ICommandHandler<UpdateMovieCommand, UpdateMovieResult>
{
    public async Task<UpdateMovieResult> Handle(UpdateMovieCommand command, CancellationToken cancellationToken)
    {
        var movie = await session.LoadAsync<Movie>(command.Id, cancellationToken);

        if (movie is null)
        {
            throw new MovieNotFoundException(command.Id);
        }

        var isDuplicate = await session.Query<Movie>()
            .AnyAsync(x => x.Id != command.Id
                        && x.Title.Equals(command.Title, StringComparison.OrdinalIgnoreCase)
                        && x.ReleaseDate == command.ReleaseDate, cancellationToken);

        if (isDuplicate)
        {
            throw new BadRequestException($"Phim '{command.Title}' đã tồn tại rồi sếp ơi!");
        }

        movie.Title = command.Title;
        movie.Genres = command.Genres;
        movie.DurationMinutes = command.DurationMinutes;
        movie.AgeRating = command.AgeRating;
        movie.ReleaseDate = command.ReleaseDate;
        movie.PosterUrl = command.PosterUrl;
        movie.Status = command.Status;

        session.Update(movie);
        await session.SaveChangesAsync(cancellationToken);

        return new UpdateMovieResult(true);
    }
}