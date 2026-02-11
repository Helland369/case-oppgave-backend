using Backend.Services;
using Backend.Dto;
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

            string query = @"SELECT StudentId, Course, Year, Semester, Type
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
}


