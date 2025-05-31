namespace CineFlix.Contracts.Request;

public class CreateMovieRequest
{
    public string MovieName { get; init; }
    public int YearOfRelease { get; init; }
    public IEnumerable<string> Genres { get; init; } = [];
}