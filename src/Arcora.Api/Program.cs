using Arcora.Api.Extensions;
using Arcora.Api.TokenServices;
using Arcora.Api.Middleware;
using Arcora.Api;
using Arcora.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddApplicationServices(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Add Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Arcora Web API",
        Description = "ASP.NET Core Web API",
        TermsOfService = new Uri("https://arcora.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "",
            Email = string.Empty,
            Url = new Uri("https://arcora.com/csr/spboyer"),
        },
        License = new OpenApiLicense
        {
            Name = "Use under LICX",
            Url = new Uri("https://arcora.com/csr/license"),
        }
    });

    // Add JWT Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: Authorization: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Enable Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Arcora API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });

    // Seed development sample data when the listing tables are empty.
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ArcoraDbContext>();
    var seedLogger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");

    // Ensure the database exists and all migrations are applied before seeding.
    await context.Database.MigrateAsync();

    // When "SeedData:ForceReseed" is true, existing seeded rows are cleared first so the
    // latest SeedData graph is inserted. Otherwise seeding only runs when tables are empty.
    var forceReseed = builder.Configuration.GetValue<bool>("SeedData:ForceReseed");
    await DbSeeder.SeedAsync(context, seedLogger, forceReseed);
}




app.UseHttpsRedirection();



app.UseGlobalExceptionMiddleware();

app.UseCors("EnableCORS");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.UseMiddleware<TokenManagerMiddleware>();

app.MapControllers();
app.MapHub<Arcora.Api.Realtime.MessagingHub>("/hubs/messaging");

app.Run();
