using form_builder.Data;
using form_builder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Text.Json;

namespace form_builder.Controllers
{
    public class FormBuilderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Save([FromBody] FormMetadata metadata)
        {
            var metadataJson = JsonSerializer.Serialize(metadata);

            using var conn = new SqliteConnection(DatabaseHelper.ConnectionString);
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Forms (Name, Metadata, CreatedAt)
                VALUES ($name, $metadata, $createdAt)";
            cmd.Parameters.AddWithValue("$name", metadata.FormName);
            cmd.Parameters.AddWithValue("$metadata", metadataJson);
            cmd.Parameters.AddWithValue("$createdAt", DateTime.UtcNow.ToString("s"));

            cmd.ExecuteNonQuery();

            return Ok();
        }
    }
}
