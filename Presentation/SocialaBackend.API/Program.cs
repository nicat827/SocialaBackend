using SocialaBackend.API.Middlewares;
using SocialaBackend.Persistence.ServiceRegistration;
using SocialaBackend.Infrastructure.ServiceRegistration;
using SocialaBackend.Application.ServiceRegistration;
using ProniaOnion.Persistence.DAL;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SocialaBackend.Persistence.Implementations.Hubs;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.Extensions.Logging.AzureAppServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(p => p.AddPolicy("corspolicy", build =>
{
    build.WithOrigins("https://app-socialite-eastus-dev-001.azurewebsites.net").AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    build.WithOrigins("https://signalr-socialite-eastus-dev-001.service.signalr.net").AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    build.WithOrigins("http://localhost:5173").AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    build.WithOrigins("https://socialite-827.netlify.app").AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}));


builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddSignalR();//.AddAzureSignalR();

builder.Logging.AddAzureWebAppDiagnostics();
//builder.Services.Configure<AzureFileLoggerOptions>(opt =>
//{
//    opt.FileName = "logs-";
//    opt.FileSizeLimit = 50 * 1024;
//    opt.RetainedFileCountLimit = 5;
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseRouting();

app.UseCors("corspolicy");

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<AppDbContextInitializer>();
    initializer.InitializeDbAsync().Wait();
    initializer.CreateRolesAsync().Wait();
    initializer.CreateAdminAsync().Wait();
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.AddPersistenceConfigure();

app.MapControllers();

app.UseEndpoints(cfg =>
{
    cfg.MapHub<NotificationHub>("/notificationHub");
    cfg.MapHub<MessagesHub>("/messagesHub");
});




app.Run();
