using ArticleService.Contracts;
using FluentValidation;

namespace ArticleService.Contracts.Validators;

public class CreateArticleCommandValidator : AbstractValidator<CreateArticleCommand>
{
    public CreateArticleCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Название статьи обязательно")
            .MinimumLength(3).WithMessage("Название должно быть не менее 3 символов")
            .MaximumLength(200).WithMessage("Название не может быть длиннее 200 символов");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Содержание статьи обязательно");

        RuleFor(x => x.ContentFormat)
            .Must(f => f is "Markdown" or "Html" or "Json")
            .WithMessage("Формат должен быть Markdown, Html или Json");

        RuleFor(x => x.AuthorId)
            .NotEqual(Guid.Empty).WithMessage("ID автора обязателен");

        RuleFor(x => x.Tags)
            .NotNull().WithMessage("Список тегов не может быть null");

        RuleForEach(x => x.Tags)
            .NotEmpty().WithMessage("Тег не может быть пустым")
            .MaximumLength(50).WithMessage("Тег не может быть длиннее 50 символов");
    }
}