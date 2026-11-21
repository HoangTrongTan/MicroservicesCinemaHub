namespace Movies.API.Exceptions;

public class MovieNotFoundException : NotFoundException
{
    public MovieNotFoundException(Guid id) : base("Movie", id) { }
}