using Backend.Services;

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

using (var scope = app.Services.CreateScope())
{
    var connectionFactory = scope.ServiceProvider.GetRequiredService<ISqliteConnectionFactory>();
    using var connection = connectionFactory.Create();
    using var command = connection.CreateCommand();

    command.CommandText = @"
        CREATE TABLE IF NOT EXISTS Event (
           EventId TEXT PRIMARY KEY,
           OccurredUtc TEXT,
           RecordedUtc TEXT,
           StudentId TEXT,
           Course TEXT,
           Year INTEGER,
           Semester INTEGER,
           Type TEXT,
           Birthdate TEXT,
           City TEXT,
           Name TEXT
        )";
    command.ExecuteNonQuery();
    Console.WriteLine("DB schema verified/created!");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
//app.UseHttpsRedirection();

app.Run();
