namespace CineFlix.Api;

public static class ApiEndpoints
{
    public const string Base = "api";

    public static class Movie
    {
        public const string GetAll = $"{Base}/movies";
        public const string Get = $"{Base}/movies/{{idOrSlug}}";
        public const string Create = $"{Base}/movies";
        public const string Update = $"{Base}/movies";
    }
}