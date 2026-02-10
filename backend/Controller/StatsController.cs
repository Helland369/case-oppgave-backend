using Backend.Model;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controller;

[ApiController]
[Route("/status")]

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
            using var command = conn.CreateCommand();

            //query not finished

            command.CommandText = @"SELECT * FROM Events WHERE ";

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return BadRequest();
        }
    }
}


