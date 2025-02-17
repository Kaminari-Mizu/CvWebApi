using Microsoft.EntityFrameworkCore;
using CvWebApi.CoreLogic;
using AutoMapper;
using Microsoft.Extensions.Options;
using Integration;
using Services;
using Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

builder.Services.AddScoped<ICardRepository, CardRepository>();
builder.Services.AddScoped<ICardService, CardService>();

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// Register DbContext
builder.Services.AddDbContext<CvWebContext>(opt =>
       opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);



// Register Services & Repositories for Dependency Injection
builder.Services.AddScoped<ICardRepository, CardRepository>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<ICarouselRepository, CarouselRepository>();
builder.Services.AddScoped<ICarouselService, CarouselService>();

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

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Use CORS policy
app.UseCors("AllowReactApp");

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
