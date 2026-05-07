using DotNetConsistency.Application.DTOs;
using FluentValidation;

namespace DotNetConsistency.Application.Validators;

public class CreateAuthorRequestValidator : AbstractValidator<CreateAuthorRequest>
{
    public CreateAuthorRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(300)
            .EmailAddress();
    }
}
