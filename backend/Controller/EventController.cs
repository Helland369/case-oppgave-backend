using Backend.Model;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controller;

[ApiController]
[Route("events")]
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
            string cmd = @"INSERT INTO Event";
            string type = "";
            string value = "";

            if (events.Type == "student_registrert")
            {
                events.StudentId = _generator.NewUuid();
            }

            if (events.Name != "") {
                type = "(EventId, OccurredUtc, RecordedUtc, StudentId, Course, Year, Semester, Type, Birthdate, City, Name)";
                value = "VALUES (@EventId, @OccurredUtc, @RecordedUtc, @StudentId, @Course, @Year, @Semester, @Type, @Birthdate, @City, @Name)";
            }
            else
            {
                type = "(EventId, OccurredUtc, RecordedUtc, StudentId, Course, Year, Semester, Type, Birthdate, City)";
                value = "VALUES (@EventId, @OccurredUtc, @RecordedUtc, @StudentId, @Course, @Year, @Semester, @Type, @Birthdate, @City)";
            }

            cmd = cmd + type + value;

            using var conn = _connection.Create();
            using var command = conn.CreateCommand();

            // command.CommandText = @"INSERT INTO Event
            //             (EventId, OccurredUtc, RecordedUtc, StudentId, Course, Year, Semester, Type, Birthdate, City, Name) VALUES
            //             (@EventId, @OccurredUtc, @RecordedUtc, @StudentId, @Course, @Year, @Semester, @Type, @Birthdate, @City, @Name)";

            command.CommandText = cmd;

            command.Parameters.AddWithValue("@EventId", events.EventId);
            command.Parameters.AddWithValue("@OccurredUtc", events.OccurredUtc);
            command.Parameters.AddWithValue("@RecordedUtc", events.RecordedUtc);
            command.Parameters.AddWithValue("@Course", events.Course);
            command.Parameters.AddWithValue("@Year", events.Year);
            command.Parameters.AddWithValue("@Semester", events.Semester);
            command.Parameters.AddWithValue("@Type", events.Type);
            command.Parameters.AddWithValue("@Birthdate", events.Birthdate);
            command.Parameters.AddWithValue("@City", events.City);
            command.Parameters.AddWithValue("@Name", events.Name);
            command.Parameters.AddWithValue("@StudentId", events.StudentId);
            
            await command.ExecuteNonQueryAsync();

            if (events.Type == "student_registrert")
                return Ok(new { studentId = events.StudentId });

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return BadRequest(ex.Message);
        }
    }
}
