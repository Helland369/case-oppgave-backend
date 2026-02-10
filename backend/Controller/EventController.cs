using Backend.Model;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controller;

[ApiController]
[Route("/events")]
public class EventController : ControllerBase
{
    private readonly ISqliteConnectionFactory _connection;
    private readonly UuidGeneratorService _generator = new();

    public EventController(ISqliteConnectionFactory connection)
    {
        _connection = connection;
    }

    [HttpPost]
    public async Task<IActionResult> PostEvents([FromBody]Event events)
    {
        try
        {
            using var conn = _connection.Create();
            using var command = conn.CreateCommand();

            command.CommandText = @"INSERT INTO Event
                        (EventId, OccurredUtc, RecordedUtc, StudentId, Course, Year, Semester, Type, Birthdate, City) VALUES
                        (@EventId, @OccurredUtc, @RecordedUtc, @StudentId, @Course, @Year, @Semester, @Type, @Birthdate, @City)";

            command.Parameters.AddWithValue("@EventId", events.EventId);
            command.Parameters.AddWithValue("@OccurredUtc", events.OccurredUtc);
            command.Parameters.AddWithValue("@RecordedUtc", events.RecordedUtc);
            command.Parameters.AddWithValue("@Course", events.Course);
            command.Parameters.AddWithValue("@Year", events.Year);
            command.Parameters.AddWithValue("@Semester", events.Semester);
            command.Parameters.AddWithValue("@Type", events.Type);
            command.Parameters.AddWithValue("@Birthdate", events.Birthdate);
            command.Parameters.AddWithValue("@City", events.City);

            if (events.Type == "student_registrert")
            {
                events.StudentId = _generator.NewUuid();
                command.Parameters.AddWithValue("@StudentId", events.StudentId);
            }

            await command.ExecuteNonQueryAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return BadRequest();
        }
    }
}
