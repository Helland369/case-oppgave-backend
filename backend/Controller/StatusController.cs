using Backend.Model;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controller;

[ApiController]
[Route("/status")]

public class StatusController : ControllerBase
{
    private readonly ISqliteConnectionFactory _connection;

    public StatusController(ISqliteConnectionFactory connection)
    {
        _connection = connection;
    }

    [HttpGet]
    public async Task<IActionResult> GetStatus([FromQuery] string? studentId = null)
    {
        try
        {
            using var conn = _connection.Create();
            using var command = conn.CreateCommand();

            if (string.IsNullOrEmpty(studentId))
            {
                //query for  All students
                command.CommandText = @"
                    SELECT 
                        StudentId,
                        Course,
                        Year,
                        Semester,
                        Type as Status,
                        OccurredUtc,
                        Birthdate,
                        City
                    FROM Event
                    WHERE (StudentId, Course, Year, Semester, OccurredUtc) IN (
                        SELECT StudentId, Course, Year, Semester, MAX(OccurredUtc)
                        FROM Event
                        GROUP BY StudentId, Course, Year, Semester
                    )
                    ORDER BY StudentId, Course, Year, Semester;
                ";
            }
            else
            {
                //query for Filtered by studentId
                command.CommandText = @"
                    SELECT 
                        StudentId,
                        Course,
                        Year,
                        Semester,
                        Type as Status,
                        OccurredUtc,
                        Birthdate,
                        City
                    FROM Event
                    WHERE StudentId = @StudentId
                      AND (StudentId, Course, Year, Semester, OccurredUtc) IN (
                        SELECT StudentId, Course, Year, Semester, MAX(OccurredUtc)
                        FROM Event
                        WHERE StudentId = @StudentId
                        GROUP BY StudentId, Course, Year, Semester
                    )
                    ORDER BY Course, Year, Semester;
                ";
                command.Parameters.AddWithValue("@StudentId", studentId);
            }

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return BadRequest();
        }
    }
}


