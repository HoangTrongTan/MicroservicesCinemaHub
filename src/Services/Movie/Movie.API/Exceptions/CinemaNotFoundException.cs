namespace Movies.API.Exceptions;

public class CinemaNotFoundException(Guid Id) : NotFoundException("Cinema", Id)
{

}