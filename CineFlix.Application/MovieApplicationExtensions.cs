using CineFlix.Application.Database;
using CineFlix.Application.Repositories;
using CineFlix.Application.Services;
using CineFlix.Contracts;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CineFlix.Application;

public static class MovieApplicationExtensions
{
    public static IServiceCollection AddMovieApplication(this IServiceCollection services)
    {
        services.AddScoped<IMovieService, MovieService>();
        services.AddScoped<IMovieRepository, MovieRepository>();
        
        services.AddValidatorsFromAssemblyContaining<ICineFlixContractsAssemblyMarker>();
        
        return services;
    }
    
    public static IServiceCollection AddMovieDatabase(this IServiceCollection services, 
        string dbConnectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new DbConnectionFactory(dbConnectionString));
        services.AddSingleton<DbInitializer>();
        return services;
    }
}