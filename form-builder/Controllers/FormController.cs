using Microsoft.AspNetCore.Mvc;
using form_builder.Data;
using Microsoft.Data.Sqlite;
using form_builder.Models;
using form_builder.Services;
using System.Text.Json;

namespace form_builder.Controllers
{
    public class FormController : Controller
    {
        private readonly FormRenderer _renderer = new FormRenderer();

        public IActionResult Render(int id=1)
        {
            string metadataJson = null;
            string formName = ""; int formId=0;

            using var conn = new SqliteConnection(DatabaseHelper.ConnectionString);
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, Metadata FROM Forms WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                formId = reader.GetInt32(0);
                formName = reader.GetString(1);
                metadataJson = reader.GetString(2);
            }
            else
            {
                return NotFound();
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var metadata = JsonSerializer.Deserialize<FormMetadata>(metadataJson, options);
            metadata.FormName = formName; // Ensure form name sync

            var formHtml = _renderer.RenderForm(metadata, formId);

            ViewBag.FormHtml = formHtml;
            ViewBag.FormName = formName;

            return View();
        }

        [HttpPost]
        [Route("Form/Submit/{id}")]
        public IActionResult Submit(int id)
        {
            // Collect all posted values
            var submissionData = new Dictionary<string, string>();

            foreach (var key in Request.Form.Keys)
            {
                submissionData[key] = Request.Form[key];
            }

            // Serialize submission to JSON
            var json = JsonSerializer.Serialize(submissionData);

            // Save to database
            using var conn = new SqliteConnection(DatabaseHelper.ConnectionString);
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
        INSERT INTO Submissions (FormId, SubmissionData, SubmittedAt)
        VALUES ($formId, $data, $submittedAt)";
            cmd.Parameters.AddWithValue("$formId", id);
            cmd.Parameters.AddWithValue("$data", json);
            cmd.Parameters.AddWithValue("$submittedAt", DateTime.UtcNow.ToString("s"));

            cmd.ExecuteNonQuery();

            return RedirectToAction("Thanks");
        }

        public IActionResult Thanks()
        {
            return View();
        }

    }
}
