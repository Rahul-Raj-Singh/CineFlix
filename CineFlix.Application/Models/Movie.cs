using System.Text.RegularExpressions;

namespace CineFlix.Application.Models;

public class Movie
{
    public Guid MovieId { get; init; }
    public string Slug => GetMovieSlug();
    public string MovieName { get; init; }
    public int YearOfRelease { get; init; }
    public IEnumerable<string> Genres { get; init; } = [];

    private string GetMovieSlug()
    {
        var cleanMovie = Regex.Replace(MovieName, @"[^A-Za-z0-9\s-]", string.Empty);
        return cleanMovie.ToLowerInvariant().Replace(" ", "-") + "-" + YearOfRelease;
    }
}