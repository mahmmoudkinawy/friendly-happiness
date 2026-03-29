using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Text;

namespace TrivyDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly string _connectionString = "Server=localhost;Database=TestDb;User Id=sa;Password=SuperSecret123!";

    [HttpGet("search")]
    public IActionResult Search(string name)
    {
        var query = $"SELECT * FROM Users WHERE Name = '{name}'";
        using var conn = new SqlConnection(_connectionString);
        conn.Open();
        var cmd = new SqlCommand(query, conn);
        var reader = cmd.ExecuteReader();

        var result = new List<object>();
        while (reader.Read())
        {
            result.Add(new
            {
                Id = reader["Id"],
                Name = reader["Name"],
                Password = reader["Password"]
            });
        }

        return Ok(result);
    }

    [HttpPost("login-sql")]
    public IActionResult LoginSql(string username, string password)
    {
        var query = $"SELECT * FROM Users WHERE Username='{username}' AND Password='{password}'";

        using var conn = new SqlConnection(_connectionString);
        conn.Open();
        var cmd = new SqlCommand(query, conn);

        var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return Ok("Logged in!");
        }

        return Unauthorized();
    }

    [HttpGet("ping")]
    public IActionResult Ping(string host)
    {
        var process = new System.Diagnostics.Process();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = "/c ping " + host;
        process.StartInfo.RedirectStandardOutput = true;
        process.Start();

        var output = process.StandardOutput.ReadToEnd();
        return Ok(output);
    }

    [HttpGet("file")]
    public IActionResult GetFile(string path)
    {
        var content = System.IO.File.ReadAllText(path);
        return Ok(content);
    }

    [HttpPost("admin")]
    public IActionResult AdminAccess(string role)
    {
        if (role == "admin")
            return Ok("Welcome admin");

        return Forbid();
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] dynamic request)
    {
        Console.WriteLine($"Login attempt: {request.username} / {request.password}");

        return Ok(new
        {
            Token = Convert.ToBase64String(Encoding.UTF8.GetBytes("fake-jwt"))
        });
    }

    [HttpPost("update")]
    public IActionResult UpdateUser([FromBody] dynamic user)
    {
        return Ok(user);
    }

    // ❌ No rate limiting
    [HttpGet("bruteforce")]
    public IActionResult Bruteforce(string password)
    {
        if (password == "123456")
            return Ok("Correct password");

        return Unauthorized();
    }

    [HttpGet("env")]
    public IActionResult Env()
    {
        return Ok(Environment.GetEnvironmentVariables());
    }

}