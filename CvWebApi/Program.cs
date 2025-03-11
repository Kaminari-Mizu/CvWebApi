using Microsoft.EntityFrameworkCore;
using CvWebApi.CoreLogic;
using AutoMapper;
using Microsoft.Extensions.Options;
using Integration;
using Services;
using Domain;

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// This is the main entry point for the application.
/// It configures the services and middleware that the application will use.
/// It also sets up dependency injection, database confiuration, CORS policies, and Swagger.
/// </summary>
//-----------------------------------
//Configure Services
//-----------------------------------

// Add controllers with Newtonsoft.Json support for better JSON serialization
builder.Services.AddControllers().AddNewtonsoftJson();

///<summary>
///Register the services and repositories for dependency injection.
///This ensures that the services and repositories are available for use throughout the application.
///</summary>

builder.Services.AddScoped<ICardRepository, CardRepository>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<ICarouselRepository, CarouselRepository>();
builder.Services.AddScoped<ICarouselService, CarouselService>();


// Register AutoMapper for object mapping
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

/// <summary>
/// Configures the database context for EF Core with SQL Server.
/// Connection string is retrieved from the appsettings.json file configuration file.
/// </summary>
var environment = builder.Environment.EnvironmentName;

if (environment == "Test")  // For testing environment, use InMemory
{
    builder.Services.AddDbContext<CvWebContext>(opt =>
        opt.UseInMemoryDatabase("TestDatabase"));
}
else  // For other environments (e.g., Development, Production), use SQL Server
{
    builder.Services.AddDbContext<CvWebContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

//-----------------------------------
// Configure CORS (Cross-Origin Resource Sharing)
//-----------------------------------

///<summary>
///Here the CORS policy is set up to allow requests from the React application (Frontend).
///This is done to ensure that the API can be accessed from the React application/other clients without
///being blocked by browser security policies.
///</summary>
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("[YOUR_REACT_PAGE_URL]")
            .AllowAnyHeader()
            .AllowAnyOrigin()
            .AllowAnyMethod();
    });
});
//-----------------------------------
// Configure Swagger for API Documentation
//-----------------------------------

///<summary>
///With this can Add Swagger to the application to provide API documentation.
///Furthermore, the Swagger UI is added to allow users to interact with the API endpoints
///for testing purposes.
///</summary>
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGenNewtonsoftSupport();

var app = builder.Build();

//-----------------------------------
// Configure Middleware
//-----------------------------------

///<summary>
///Here CORS policy enabled to allow cross-origin requests
///This must be called before handling any incoming requests
///</summary>
app.UseCors("AllowReactApp");
///<summary>
/// Configure the HTTP request pipeline
///If in development environment, Swagger is enabled
///</summary>
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Enables HTTPS redirection for security
app.UseHttpsRedirection();

//Enables Authorisation middleware for secure endpoints
app.UseAuthorization();

//Maps controller endpoints
app.MapControllers();

//Runs the application
app.Run();
