using DotNetConsistency.Application.DTOs;
using FluentValidation;

namespace DotNetConsistency.Api.Validators;

public class CreateAuthorRequestValidator : AbstractValidator<CreateAuthorRequest>
{
    public CreateAuthorRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Yazar adı boş bırakılamaz.")
            .MaximumLength(200).WithMessage("Yazar adı en fazla {MaxLength} karakter olabilir.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta boş bırakılamaz.")
            .MaximumLength(300).WithMessage("E-posta en fazla {MaxLength} karakter olabilir.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi girilmelidir.");
    }
}
