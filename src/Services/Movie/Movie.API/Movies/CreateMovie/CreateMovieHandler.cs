namespace Movies.API.Movies.CreateMovie;
public record CreateMovieCommand
    (string Title, List<string> Genres, int DurationMinutes, AgeRating AgeRating, DateOnly ReleaseDate, string PosterUrl, MovieStatus Status) 
    : ICommand<CreateMovieResult>;
public record CreateMovieResult(Guid Id);

public class CreateMovieCommandValidator : AbstractValidator<CreateMovieCommand>
{
    public CreateMovieCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Tên phim không được để trống")
            .MaximumLength(200).WithMessage("Tên phim tối đa 200 ký tự thôi sếp ơi");

        RuleFor(x => x.Genres)
            .NotEmpty().WithMessage("Phim phải có ít nhất 1 thể loại");

        RuleForEach(x => x.Genres)
            .NotEmpty().WithMessage("Tên thể loại không được để trống");

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0).WithMessage("Thời lượng phim phải lớn hơn 0 phút")
            .LessThanOrEqualTo(600).WithMessage("Phim gì dài hơn 10 tiếng vậy trời");

        RuleFor(x => x.AgeRating)
            .IsInEnum().WithMessage("Độ tuổi không hợp lệ (chỉ nhận P, T13, T16, T18)");

        RuleFor(x => x.ReleaseDate)
            .NotEmpty().WithMessage("Ngày phát hành không được để trống");

        RuleFor(x => x.PosterUrl)
            .NotEmpty().WithMessage("Poster URL không được để trống")
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var uri)
                    && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            .WithMessage("Poster URL phải là link http/https hợp lệ");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Trạng thái phim không hợp lệ");
    }
}

public class CreateMovieCommandHandler
    (IDocumentSession session)
    : ICommandHandler<CreateMovieCommand, CreateMovieResult>
{
    public async Task<CreateMovieResult> Handle(CreateMovieCommand command, CancellationToken cancellationToken)
    {
        var isExist = await session
            .Query<Movie>()
            .AnyAsync(x => x.Title.Equals(command.Title, StringComparison.OrdinalIgnoreCase) && x.ReleaseDate == command.ReleaseDate, cancellationToken);

        if (isExist)
        {
            throw new Exception($"Phim '{command.Title}' đã tồn tại rồi sếp ơi!");
        }

        var movive = new Movie
        {
          Title = command.Title,
          Genres = command.Genres,
          DurationMinutes = command.DurationMinutes,
          AgeRating = command.AgeRating,
          ReleaseDate = command.ReleaseDate,
          PosterUrl = command.PosterUrl,
          Status = command.Status
        };

        session.Store(movive);
        await session.SaveChangesAsync(cancellationToken);

        return new CreateMovieResult(movive.Id);
    }
}