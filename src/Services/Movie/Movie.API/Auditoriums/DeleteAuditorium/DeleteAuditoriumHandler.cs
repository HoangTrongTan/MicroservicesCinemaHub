namespace Movies.API.Auditoriums.DeleteAuditorium;
public record DeleteAuditoriumCommand(Guid Id) : ICommand<DeleteAuditoriumResult>, ICacheInvalidatorCommand
{
    string[] ICacheInvalidatorCommand.CacheKeys => [$"auditorium:{Id}"];
}
public record DeleteAuditoriumResult(bool IsSuccess);
public class DeleteAuditoriumCommandValidator : AbstractValidator<DeleteAuditoriumCommand>
{
    public DeleteAuditoriumCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Thiếu Id phòng chiếu cần xóa rồi sếp ơi!");
    }
}
internal class DeleteAuditoriumCommandHandler
    (IDocumentSession session)
    : ICommandHandler<DeleteAuditoriumCommand, DeleteAuditoriumResult>
{
    public async Task<DeleteAuditoriumResult> Handle(DeleteAuditoriumCommand command, CancellationToken cancellationToken)
    {
        var auditorium = await session.LoadAsync<Auditorium>(command.Id, cancellationToken);

        if (auditorium is null)
        {
            throw new AuditoriumNotFoundException(command.Id);
        }

        session.Delete(auditorium);
        await session.SaveChangesAsync(cancellationToken);

        return new DeleteAuditoriumResult(true);
    }
}