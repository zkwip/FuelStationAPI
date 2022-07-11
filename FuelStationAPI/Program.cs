using FuelStationAPI.DataProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IFuelStationDataProvider, TinqStationDataProvider>();
builder.Services.AddScoped<IFuelStationDataProvider, TangoStationDataProvider>();
builder.Services.AddScoped<IFuelStationDataProvider, CarbuStationDataProvider>();
builder.Services.AddScoped<IFuelStationDataProvider, ArgosStationDataProvider>();
builder.Services.AddScoped<IFuelStationDataProvider, CleverTankenStationDataProvider>();


builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

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
