using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

// CORS — разрешаем всё для разработки
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseCors("AllowAll");
app.MapReverseProxy();

app.Run();