namespace Movies.API.Movies.DeleteMovie;
public record DeleteMovieCommand(Guid Id) : ICommand<DeleteMovieResult>, ICacheInvalidatorCommand
{
    string[] ICacheInvalidatorCommand.CacheKeys => [$"movie:{Id}"];
}

public record DeleteMovieResult(bool IsSuccess);
public class DeleteMovieCommandValidator : AbstractValidator<DeleteMovieCommand>
{
    public DeleteMovieCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Movie Id không được để trống nha sếp!");
    }
}
internal class DeleteMovieCommandHandler
    (IDocumentSession session)
    : ICommandHandler<DeleteMovieCommand, DeleteMovieResult>
{
    public async Task<DeleteMovieResult> Handle(DeleteMovieCommand command, CancellationToken cancellationToken)
    {
        var movie = await session.LoadAsync<Movie>(command.Id, cancellationToken);

        if (movie is null)
        {
            throw new MovieNotFoundException(command.Id);
        }

        session.Delete(movie);
        await session.SaveChangesAsync(cancellationToken);

        return new DeleteMovieResult(true);
    }
}