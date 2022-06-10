using FuelStationAPI.Scraper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<HttpClient>();
builder.Services.AddSingleton<IFuelSiteScraper>(x => new TinqSiteScraper(x.GetService<HttpClient>()!, x.GetService<ILogger>()!));
builder.Services.AddSingleton<IFuelSiteScraper>(x => new TangoSiteScraper(x.GetService<HttpClient>()!, x.GetService<ILogger>()!));
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
