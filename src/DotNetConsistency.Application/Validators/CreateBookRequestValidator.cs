using DotNetConsistency.Application.DTOs;
using FluentValidation;

namespace DotNetConsistency.Application.Validators;

public class CreateBookRequestValidator : AbstractValidator<CreateBookRequest>
{
    public CreateBookRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.ISBN)
            .NotEmpty()
            .MaximumLength(20)
            .Matches(@"^[0-9\-]{10,20}$")
            .WithMessage("ISBN must be 10-20 characters containing only digits and hyphens.");

        RuleFor(x => x.Price)
            .GreaterThan(0);

        RuleFor(x => x.AuthorId)
            .GreaterThan(0);
    }
}
