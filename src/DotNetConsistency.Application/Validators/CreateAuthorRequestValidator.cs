using DotNetConsistency.Application.DTOs;
using FluentValidation;

namespace DotNetConsistency.Application.Validators;

public class CreateAuthorRequestValidator : AbstractValidator<CreateAuthorRequest>
{
    public CreateAuthorRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Yazar adı boş bırakılamaz.")
            .MaximumLength(200).WithMessage("Yazar adı en fazla {MaxLength} karakter olabilir.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta adresi boş bırakılamaz.")
            .MaximumLength(300).WithMessage("E-posta adresi en fazla {MaxLength} karakter olabilir.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi girilmelidir.");
    }
}
