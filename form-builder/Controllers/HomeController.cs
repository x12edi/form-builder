using form_builder.Data;
using form_builder.Models;
using form_builder.Data;
using form_builder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace form_builder.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var forms = new List<FormModel>();

            using var conn = new SqliteConnection(DatabaseHelper.ConnectionString);
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, Metadata, CreatedAt FROM Forms";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                forms.Add(new FormModel
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Metadata = reader.GetString(2),
                    CreatedAt = DateTime.Parse(reader.GetString(3))
                });
            }

            return View(forms);
        }
    }
}
