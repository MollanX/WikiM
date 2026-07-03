using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

// YARP — загружает конфигурацию маршрутов из appsettings
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapReverseProxy(); // Все запросы идут через прокси

app.Run();