using Microsoft.EntityFrameworkCore;
using CvWebApi.Models;
using CvWebApi.CoreLogic; // Ensure you include the namespace for your AutoMapper Profile

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// Register your DbContext with a database connection
builder.Services.AddDbContext<CvWebContext>(opt =>
    opt.UseInMemoryDatabase("CvDatabase"));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("[YOUR_REACT_PAGE_URL]")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add Swagger/OpenAPI for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Use CORS policy
app.UseCors("AllowReactApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
