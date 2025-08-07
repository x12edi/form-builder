using System.Text;
using form_builder.Models;

namespace form_builder.Services
{
    public class FormRenderer
    {
        public string RenderForm(FormMetadata metadata, int formId)
        {
            var sb = new StringBuilder();

            //sb.AppendLine($"<form id=\"{metadata.FormName.Replace(" ", "")}Form\" method=\"post\">");
            sb.AppendLine($"<form id=\"{metadata.FormName.Replace(" ", "")}Form\" method=\"post\" action=\"/Form/Submit/{formId}\">");


            foreach (var field in metadata.Fields)
            {
                sb.AppendLine(RenderField(field));
            }

            sb.AppendLine("<button type=\"submit\" class=\"btn btn-primary\">Submit</button>");
            sb.AppendLine("</form>");

            return sb.ToString();
        }

        private string RenderField(FieldMetadata field)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<div class=\"form-group mb-3\">");
            sb.AppendLine($"<label for=\"{field.Id}\">{field.Label}</label>");

            var requiredAttr = field.Required ? "required" : "";

            switch (field.Type.ToLower())
            {
                case "text":
                case "email":
                case "number":
                    sb.AppendLine($"<input type=\"{field.Type}\" class=\"form-control\" id=\"{field.Id}\" name=\"{field.Id}\" {requiredAttr} />");
                    break;

                case "textarea":
                    sb.AppendLine($"<textarea class=\"form-control\" id=\"{field.Id}\" name=\"{field.Id}\" {requiredAttr}></textarea>");
                    break;

                case "checkbox":
                    sb.AppendLine("<div class=\"form-check\">");
                    sb.AppendLine($"<input type=\"checkbox\" class=\"form-check-input\" id=\"{field.Id}\" name=\"{field.Id}\" />");
                    sb.AppendLine($"<label class=\"form-check-label\" for=\"{field.Id}\">{field.Label}</label>");
                    sb.AppendLine("</div>");
                    break;

                case "select":
                    sb.AppendLine($"<select class=\"form-control\" id=\"{field.Id}\" name=\"{field.Id}\" {requiredAttr}>");
                    if (field.Options != null)
                    {
                        foreach (var option in field.Options)
                        {
                            sb.AppendLine($"<option value=\"{option}\">{option}</option>");
                        }
                    }
                    sb.AppendLine("</select>");
                    break;

                default:
                    sb.AppendLine($"<input type=\"text\" class=\"form-control\" id=\"{field.Id}\" name=\"{field.Id}\" {requiredAttr} />");
                    break;
            }

            sb.AppendLine("</div>");

            return sb.ToString();
        }
    }
}
