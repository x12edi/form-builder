using System.Collections.Generic;

namespace form_builder.Models
{
    public class FormMetadata
    {
        public string FormName { get; set; }
        public List<FieldMetadata> Fields { get; set; }
    }

    public class FieldMetadata
    {
        public string Id { get; set; }           // Unique field identifier
        public string Label { get; set; }        // Label text
        public string Type { get; set; }         // Field type (text, email, textarea, checkbox, select)
        public bool Required { get; set; }       // Is field required?
        public List<string> Options { get; set; } // For select/radio options (optional)
    }
}
