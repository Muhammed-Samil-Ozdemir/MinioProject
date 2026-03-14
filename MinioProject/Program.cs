using Minio;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<MinioClient>(sp =>
{
    return (MinioClient)new MinioClient()
        .WithEndpoint("10.0.0.22:9100")
        .WithCredentials("minioadmin", "minioadmin123")
        .WithSSL(false)
        .Build();
});

var app = builder.Build();

app.MapControllers();

app.Run();