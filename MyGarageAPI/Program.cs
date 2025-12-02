using Manager.Abstractions;
using Manager.Manager;
using Microsoft.OpenApi;
using MyGarageAPI.BusinessLogic;
using Repository.Abstractions;
using Repository.Repository;

var builder = WebApplication.CreateBuilder(args);

// Connection string SQLite
string sqliteConn = builder.Configuration.GetConnectionString("SQLiteConnection");

// Repository → injecte la connection string
builder.Services.AddScoped<IMyGarageRepository>(provider => new MyGarageRepository(sqliteConn));

// Manager
builder.Services.AddScoped<IMyGarageManager, MyGarageManager>();

// Business
builder.Services.AddScoped<MyGarageBusiness>();

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyGarageAPI v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();