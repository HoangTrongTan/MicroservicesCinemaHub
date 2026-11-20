namespace Movie.API.Models;

public class Movie
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public List<string> Genres { get; set; } = [];
    public int DurationMinutes { get; set; }
    public AgeRating AgeRating { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public string PosterUrl { get; set; } = default!;
    public MovieStatus Status { get; set; } = MovieStatus.Upcoming;
}

public enum AgeRating
{
    P,
    T13,
    T16,
    T18
}

public enum MovieStatus
{
    Upcoming,
    NowShowing,
    Ended
}