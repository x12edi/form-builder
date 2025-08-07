namespace form_builder.Models
{
    public class FormSubmission
    {
        public int Id { get; set; }
        public int FormId { get; set; }
        public string SubmissionData { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
