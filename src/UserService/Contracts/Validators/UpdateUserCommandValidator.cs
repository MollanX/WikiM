using FluentValidation;

namespace UserService.Contracts.Validators;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Имя пользователя обязательно")
            .MinimumLength(3).WithMessage("Минимум 3 символа")
            .MaximumLength(50).WithMessage("Максимум 50 символов")
            .Matches("^[a-zA-Z0-9_-]+$").WithMessage("Только буквы, цифры, дефис и подчёркивание");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен")
            .EmailAddress().WithMessage("Некорректный email")
            .MaximumLength(256);

        RuleFor(x => x.Phone)
            .Matches(@"^\+?[0-9\s\-\(\)]{5,20}$")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone))
            .WithMessage("Некорректный номер телефона");

        RuleFor(x => x.Bio)
            .MaximumLength(500).WithMessage("Описание не может быть длиннее 500 символов");

        RuleFor(x => x.BirthDate)
            .Must(date => date == null || date <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Дата рождения не может быть в будущем");
    }
}