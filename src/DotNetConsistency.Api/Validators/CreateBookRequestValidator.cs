using DotNetConsistency.Application.DTOs;
using FluentValidation;

namespace DotNetConsistency.Api.Validators;

public class CreateBookRequestValidator : AbstractValidator<CreateBookRequest>
{
    public CreateBookRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Kitap başlığı boş bırakılamaz.")
            .MaximumLength(500).WithMessage("Kitap başlığı en fazla {MaxLength} karakter olabilir.");

        RuleFor(x => x.ISBN)
            .NotEmpty().WithMessage("ISBN boş bırakılamaz.")
            .MaximumLength(20).WithMessage("ISBN en fazla {MaxLength} karakter olabilir.")
            .Matches(@"^[0-9\-]{10,20}$").WithMessage("ISBN yalnızca rakam ve tire içermeli, 10-20 karakter uzunluğunda olmalıdır.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır.");

        RuleFor(x => x.AuthorId)
            .GreaterThan(0).WithMessage("Geçerli bir yazar ID'si girilmelidir.");
    }
}
