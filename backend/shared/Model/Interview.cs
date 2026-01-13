namespace VolunteerManagement.Model
{
    public class Interview
    {
        public int Id { get; set; }
        public List<Volunteer> Interviewers { get; set; }
        public required int CandidateId { get; set; }
        public List<string> Answers { get; set; }
        public int? LocationId { get; set; }
        public DateTime? DateTime { get; set; }
        public string? Notes { get; set; }
        public Interview()
        {
            Interviewers = [];
            Answers = [];
        }
    }
}