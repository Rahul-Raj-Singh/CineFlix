using CineFlix.Application.Models;
using CineFlix.Application.Repositories;
using CineFlix.Contracts.Request;

namespace CineFlix.Application.Services;

public interface IMovieService
{
    Task<IEnumerable<Movie>> GetAll();
    Task<Movie> GetById(Guid movieId);
    Task<Movie> GetBySlug(string slug);
    Task<Movie> Create(CreateMovieRequest request);
    Task<bool> Update(UpdateMovieRequest request);
}

public class MovieService(IMovieRepository movieRepository) : IMovieService
{
    public async Task<IEnumerable<Movie>> GetAll()
    {
        return await movieRepository.GetAll();
    }

    public async Task<Movie> GetById(Guid movieId)
    {
        return await movieRepository.GetById(movieId);
    }

    public async Task<Movie> GetBySlug(string slug)
    {
        return await movieRepository.GetBySlug(slug);
    }

    public async Task<Movie> Create(CreateMovieRequest request)
    {
        return await movieRepository.Create(request);
    }

    public async Task<bool> Update(UpdateMovieRequest request)
    {
        var alreadyExists = await movieRepository.Exists(request.MovieId);
        
        if (!alreadyExists) return false;
        
        return await movieRepository.Update(request);
    }
}