using form_builder.Data;
using form_builder.Models;
using form_builder.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Text;
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

        [Route("Form/Submissions/{id}")]
        public IActionResult Submissions(int id)
        {
            string formName = "";
            var submissions = new List<FormSubmission>();

            using var conn = new SqliteConnection(DatabaseHelper.ConnectionString);
            conn.Open();

            // Get form name
            var nameCmd = conn.CreateCommand();
            nameCmd.CommandText = "SELECT Name FROM Forms WHERE Id = $id";
            nameCmd.Parameters.AddWithValue("$id", id);
            var result = nameCmd.ExecuteScalar();
            if (result == null)
                return NotFound("Form not found");
            formName = result.ToString();

            // Get submissions
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, SubmissionData, SubmittedAt FROM Submissions WHERE FormId = $id ORDER BY SubmittedAt DESC";
            cmd.Parameters.AddWithValue("$id", id);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                submissions.Add(new FormSubmission
                {
                    Id = reader.GetInt32(0),
                    SubmissionData = reader.GetString(1),
                    SubmittedAt = DateTime.Parse(reader.GetString(2))
                });
            }

            ViewBag.FormName = formName;
            ViewBag.FormId = id;
            return View(submissions);
        }

        [HttpGet]
        [Route("Form/ExportCsv/{id}")]
        public IActionResult ExportCsv(int id)
        {
            var submissions = new List<FormSubmission>();
            string formName = "";
            var fieldNames = new HashSet<string>();

            using var conn = new SqliteConnection(DatabaseHelper.ConnectionString);
            conn.Open();

            // Get form name
            var nameCmd = conn.CreateCommand();
            nameCmd.CommandText = "SELECT Name FROM Forms WHERE Id = $id";
            nameCmd.Parameters.AddWithValue("$id", id);
            formName = nameCmd.ExecuteScalar()?.ToString() ?? "Form";

            // Get submissions
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT SubmissionData, SubmittedAt FROM Submissions WHERE FormId = $id ORDER BY SubmittedAt DESC";
            cmd.Parameters.AddWithValue("$id", id);

            using var reader = cmd.ExecuteReader();
            var parsedSubmissions = new List<Dictionary<string, string>>();

            while (reader.Read())
            {
                var dataJson = reader.GetString(0);
                var submittedAt = DateTime.Parse(reader.GetString(1)).ToString("yyyy-MM-dd HH:mm");

                var data = JsonSerializer.Deserialize<Dictionary<string, string>>(dataJson);
                data["SubmittedAt"] = submittedAt;
                parsedSubmissions.Add(data);

                foreach (var key in data.Keys)
                    fieldNames.Add(key);
            }

            // CSV generation
            var csv = new StringBuilder();
            var columns = fieldNames.OrderBy(k => k).ToList();
            csv.AppendLine(string.Join(",", columns));

            foreach (var sub in parsedSubmissions)
            {
                var row = columns.Select(col => EscapeCsv(sub.ContainsKey(col) ? sub[col] : ""));
                csv.AppendLine(string.Join(",", row));
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"{formName}_Submissions.csv");
        }

        // Escape values to handle commas, quotes, etc.
        private static string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }
            return value;
        }

    }
}
