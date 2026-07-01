using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using ArticleService.Data;
using ArticleService.Services;
using FluentValidation;
using OpenMediator;

var builder = WebApplication.CreateBuilder(args);

// === OpenMediator ===
builder.Services.AddOpenMediator(cfg =>
    cfg.RegisterCommandsFromAssembly(typeof(Program).Assembly));

// === FluentValidation ===
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// === Контроллеры и Swagger ===
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// === PostgreSQL ===
builder.Services.AddDbContext<ArticleDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// === Redis ===
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = ConfigurationOptions.Parse(
        builder.Configuration.GetConnectionString("Redis")!);
    config.AbortOnConnectFail = false;
    return ConnectionMultiplexer.Connect(config);
});

// === Репозиторий ===
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

// === Health Checks ===
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ArticleDbContext>("database")
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!, "redis");

// === Фоновые сервисы ===
builder.Services.AddHostedService<ArticleCleanupService>();

// === Сборка приложения ===
var app = builder.Build();

// === Авто-создание БД (только для разработки) ===
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ArticleDbContext>();
    await db.Database.EnsureCreatedAsync();
}

// === Middleware ===
app.UseSwagger();
app.UseSwaggerUI();
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();