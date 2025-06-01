using System.Text.RegularExpressions;

namespace CineFlix.Application.Models;

public class Movie
{
    public Guid MovieId { get; set; }
    public string Slug => GetMovieSlug();
    public string MovieName { get; set; }
    public int YearOfRelease { get; set; }
    public IEnumerable<string> Genres { get; set; } = [];

    private string GetMovieSlug()
    {
        var cleanMovie = Regex.Replace(MovieName, @"[^A-Za-z0-9\s-]", string.Empty);
        return cleanMovie.ToLowerInvariant().Replace(" ", "-") + "-" + YearOfRelease;
    }
}