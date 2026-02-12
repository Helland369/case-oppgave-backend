using Backend.Services;
using Backend.Dto;
using Backend.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

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
    public async Task<ActionResult<List<StatusDto>>> GetStatus()
    {
        try
        {
            List<StatusDto> results = new();
            using var conn = _connection.Create();

            const string query = @"SELECT StudentId, Course, Year, Semester, Type
                              FROM (
                                SELECT
                                  StudentId, Course, Year, Semester, Type,
                                  ROW_NUMBER() OVER (
                                    PARTITION BY StudentId, Course, Year, Semester
                                    ORDER BY OccurredUtc DESC, RecordedUtc DESC, EventId DESC
                                  ) AS rn
                                FROM Event
                                WHERE Course IS NOT NULL AND Course <> ''
                                  AND Year <> 0
                                  AND Semester <> 0 
                              )
                              WHERE rn = 1
                              ORDER BY StudentId, Course, Year, Semester;
                            ";

            using var command = new SqliteCommand(query, conn);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new StatusDto(
                    reader["StudentId"]?.ToString() ?? "",
                    reader["Course"]?.ToString() ?? "",
                    Convert.ToInt32(reader["Year"]),
                    reader["Semester"]?.ToString() ?? "",
                    reader["Type"]?.ToString() ?? ""
                ));
            }
            return Ok(results);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return BadRequest();
        }
    }

    [HttpGet("{studentId}")]
    public async Task<ActionResult<List<Event>>> FilterStatus(string studentId)
    {
        try
        {
            List<Event> result = new();
            using var conn = _connection.Create();
            string query = "SELECT * FROM Event WHERE StudentId = @studentId";

            using var command = new SqliteCommand(query, conn);
            command.Parameters.AddWithValue("@studentId", studentId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new Event
                {
                    StudentId = reader["StudentId"]?.ToString() ?? "",
                    EventId = reader["EventId"]?.ToString() ?? "",
                    Name = reader["Name"]?.ToString() ?? "",
                    OccurredUtc = Convert.ToDateTime(reader["OccurredUtc"]),
                    RecordedUtc = Convert.ToDateTime(reader["RecordedUtc"]),
                    Course = reader["Course"]?.ToString() ?? "",
                    Year = Convert.ToInt32(reader["Year"]),
                    Semester = Convert.ToInt32(reader["Semester"]),
                    Type = reader["Type"]?.ToString() ?? "",
                    Birthdate = reader["Birthdate"]?.ToString() ?? "",
                    City = reader["City"]?.ToString() ?? "",
                });
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return BadRequest();
        }
    }
}


