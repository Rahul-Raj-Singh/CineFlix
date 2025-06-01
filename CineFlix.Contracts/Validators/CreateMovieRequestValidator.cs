using CineFlix.Contracts.Request;
using FluentValidation;

namespace CineFlix.Contracts.Validators;

public class CreateMovieRequestValidator : AbstractValidator<CreateMovieRequest>
{
    public CreateMovieRequestValidator()
    {
        RuleFor(x => x.MovieName).NotEmpty();
        RuleFor(x => x.YearOfRelease).LessThanOrEqualTo(DateTime.Now.Year);
        RuleFor(x => x.Genres).NotEmpty();

    }
}