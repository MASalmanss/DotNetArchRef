using DotNetConsistency.Application.DTOs;
using FluentValidation;

namespace DotNetConsistency.Api.Validators;

public class UpdateBookRequestValidator : AbstractValidator<UpdateBookRequest>
{
    public UpdateBookRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Kitap başlığı boş bırakılamaz.")
            .MaximumLength(500).WithMessage("Kitap başlığı en fazla {MaxLength} karakter olabilir.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır.");
    }
}
