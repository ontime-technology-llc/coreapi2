using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using WebApi.Services;
using WebApi.Helpers;
using WebApi.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Net;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

// add services to DI container
{
    var services = builder.Services;
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();

    services.AddDbContext<DataBaseContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlServerOptionsAction: sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure();
                sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                sqlOptions.CommandTimeout((int)TimeSpan.FromMinutes(5).TotalSeconds);
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(10), errorNumbersToAdd: null);
            });
    });

    services.AddCors();
    services.AddControllers();

    // configure strongly typed settings object
    services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

    // configure DI for application services
    services.AddScoped<IUserService, UserService>();
}

var app = builder.Build();

// configure HTTP request pipeline
{
    // global cors policy
    app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

    // custom jwt auth middleware
    app.UseMiddleware<JwtMiddleware>();

    app.MapControllers();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = string.Empty;
            options.DocumentTitle = "My Swagger Boyz";
        });
    }
    else if (app.Environment.IsStaging()) { }
    else if (app.Environment.IsProduction()) { }
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        // Retrieve the instance of your database context
        var dbContext = services.GetRequiredService<DataBaseContext>();

        // Apply any pending database migrations
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Handle any potential migration exception
        // Log the error or perform other error handling tasks
        Console.WriteLine($"Error occurred while applying migrations: {ex.Message}");
    }
}

//app.Run("http://localhost:4000");
app.Run();