namespace Movies.API.Cinemas.DeleteCinema;
public record DeleteCinemaCommand(Guid Id) : ICommand<DeleteCinemaResult>, ICacheInvalidatorCommand
{
    string[] ICacheInvalidatorCommand.CacheKeys => [$"cinema:{Id}"];
}
public record DeleteCinemaResult(bool IsSuccess);
public class DeleteCinemaCommandValidator : AbstractValidator<DeleteCinemaCommand>
{
    public DeleteCinemaCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Thiếu Id rạp cần xóa rồi sếp ơi!");
    }
}
public class DeleteCinemaCommandHandler
    (IDocumentSession session)
    : ICommandHandler<DeleteCinemaCommand, DeleteCinemaResult>
{
    public async Task<DeleteCinemaResult> Handle(DeleteCinemaCommand command, CancellationToken cancellationToken)
    {
        var cinema = await session.LoadAsync<Cinema>(command.Id, cancellationToken);

        if (cinema is null)
        {
            throw new CinemaNotFoundException(command.Id);
        }

        session.Delete(cinema);

        await session.SaveChangesAsync(cancellationToken);

        return new DeleteCinemaResult(true);
    }
}