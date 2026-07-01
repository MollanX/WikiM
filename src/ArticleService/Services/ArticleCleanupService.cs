namespace ArticleService.Services;

public class ArticleCleanupService(IServiceScopeFactory scopeFactory, ILogger<ArticleCleanupService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var repository = scope.ServiceProvider.GetRequiredService<IArticleRepository>();

                var threshold = DateTime.UtcNow;
                var expired = await repository.GetExpiredForDeletionAsync(threshold);

                if (expired.Any())
                {
                    await repository.PermanentDeleteRangeAsync(expired);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Error cleaning up expired articles");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}