using ArticleService.Contracts;
using FluentValidation;

namespace ArticleService.Contracts.Validators;

public class UpdateArticleCommandValidator : AbstractValidator<UpdateArticleCommand>
{
    public UpdateArticleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage("ID статьи обязателен");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Название статьи обязательно")
            .MinimumLength(3).WithMessage("Название должно быть не менее 3 символов")
            .MaximumLength(200).WithMessage("Название не может быть длиннее 200 символов");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Содержание статьи обязательно");

        RuleFor(x => x.ContentFormat)
            .Must(f => f is "Markdown" or "Html" or "Json")
            .WithMessage("Формат должен быть Markdown, Html или Json");

        RuleForEach(x => x.Tags)
            .NotEmpty().WithMessage("Тег не может быть пустым")
            .MaximumLength(50).WithMessage("Тег не может быть длиннее 50 символов");
    }
}