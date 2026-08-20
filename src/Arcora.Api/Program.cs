using Arcora.Api.Extensions;
using Arcora.Api.TokenServices;
using Arcora.Api.Middleware;
using Microsoft.OpenApi.Models;;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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

app.Run();
