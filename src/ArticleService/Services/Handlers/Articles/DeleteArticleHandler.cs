using ArticleService.Contracts;
using OpenMediator;

namespace ArticleService.Services.Handlers;

public class DeleteArticleHandler(IArticleRepository repository) : ICommandHandler<DeleteArticleCommand, ApiResponse>
{
    public async Task<ApiResponse> HandleAsync(DeleteArticleCommand command, CancellationToken cancellationToken)
    {
        bool deleted;

        if (command.Permanent)
        {
            deleted = await repository.PermanentDeleteAsync(command.Id);
            if (deleted)
            {
                await repository.InvalidateArticleCacheAsync(command.Id);
                await repository.InvalidateListCacheAsync();
                return ApiResponse.Ok("Статья полностью удалена");
            }
            return ApiResponse.Fail($"Статья с ID {command.Id} не найдена");
        }

        deleted = await repository.SoftDeleteAsync(command.Id);

        if (deleted)
        {
            await repository.InvalidateArticleCacheAsync(command.Id);
            await repository.InvalidateListCacheAsync();
            return ApiResponse.Ok("Статья помечена как удалённая");
        }
        return ApiResponse.Fail($"Статья с ID {command.Id} не найдена");
    }
}