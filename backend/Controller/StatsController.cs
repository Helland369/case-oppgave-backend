using Backend.Dto;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace Backend.Controller;

[ApiController]
[Route("/stats")]
public class StatsController : ControllerBase
{
    private readonly ISqliteConnectionFactory _connection;

    public StatsController(ISqliteConnectionFactory connection)
    {
        _connection = connection;
    }

    [HttpGet]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            using var conn = _connection.Create();

            string query = @"
                            WITH ranked AS (
                              SELECT
                                StudentId,
                                Course,
                                Year,
                                Semester,
                                Type,
                                ROW_NUMBER() OVER (
                                  PARTITION BY StudentId, Course, Year, Semester
                                  ORDER BY OccurredUtc DESC, RecordedUtc DESC, EventId DESC
                                ) AS rn
                            FROM Event
                              WHERE Course IS NOT NULL AND Course <> ''
                              AND Year <> 0
                              AND Semester <> 0
                            ),
                            latest AS (
                              SELECT StudentId, Course, Year, Semester, Type
                              FROM ranked
                              WHERE rn = 1
                            ),
                            totals AS (
                              SELECT Course, Year, Semester, COUNT(*) AS TotalStudents
                              FROM latest
                              GROUP BY Course, Year, Semester
                            )
                            SELECT
                              l.Course,
                              l.Year,
                              l.Semester,
                              t.TotalStudents,
                              l.Type AS Status,
                              COUNT(*) AS StatusCount
                            FROM latest l
                            JOIN totals t USING (Course, Year, Semester)
                            GROUP BY l.Course, l.Year, l.Semester, t.TotalStudents, l.Type
                            ORDER BY l.Course, l.Year, l.Semester, l.Type;
                            ";

            using var command = new SqliteCommand(query, conn);

            var map = new Dictionary<(string Course, int Year, int Semester), CourseRoundStatsDto>();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var course = reader.GetString(0);
                var year = reader.GetInt32(1);
                var semester = reader.GetInt32(2);
                var total = reader.GetInt32(3);
                var status = reader.GetString(4);
                var count = reader.GetInt32(5);

                var key = (course, year, semester);

                if (!map.TryGetValue(key, out var round))
                {
                    round = new CourseRoundStatsDto(course, year, semester, total, new List<StatusCountDto>());
                    map[key] = round;
                }

                round.StatucCounts.Add(new StatusCountDto(status, count));
            }

            var response = new StatsResponseDto(
                map.Values
                .OrderBy(x => x.Course)
                .ThenBy(x => x.Year)
                .ThenBy(x => x.Semester)
                .ToList()
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return BadRequest();
        }
    }
}
