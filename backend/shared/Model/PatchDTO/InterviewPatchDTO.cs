namespace VolunteerManagement.Model
{
    public class InterviewPatchDTO
    {
        public List<Volunteer>? Interviewers { get; set; }
        public List<string>? Answers { get; set; }
        public int? LocationId { get; set; }
        public DateTime? DateTime { get; set; }
        public string? Notes { get; set; }

        public InterviewPatchDTO() { }
    }
}