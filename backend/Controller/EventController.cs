using Backend.Model;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controller;

[ApiController]
[Route("/events")]
public class EventController : ControllerBase
{
    private readonly ISqliteConnectionFactory _connection;

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
            command.Parameters.AddWithValue("@OccurredUt", events.OccurredUtc);
            command.Parameters.AddWithValue("@RecordedUtc", events.RecordedUtc);
            command.Parameters.AddWithValue("@StudentI", events.StudentId);
            command.Parameters.AddWithValue("@Course", events.Course);
            command.Parameters.AddWithValue("@Year", events.Year);
            command.Parameters.AddWithValue("@Semester", events.Semester);
            command.Parameters.AddWithValue("@Type", events.Type);
            command.Parameters.AddWithValue("@Birthdat", events.Birthdate);
            command.Parameters.AddWithValue("@City", events.City);

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return BadRequest();
        }
    }
}
