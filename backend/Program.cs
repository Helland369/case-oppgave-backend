using Backend.Services;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();

var cs = builder.Configuration.GetConnectionString("sqlite")
         ?? "Data Source=case.db";

builder.Services.AddSingleton<ISqliteConnectionFactory>(
    new SqliteConnectionFactory(cs)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseHttpsRedirection();

app.Run();
