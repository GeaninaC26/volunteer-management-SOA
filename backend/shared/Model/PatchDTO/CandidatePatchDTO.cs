namespace VolunteerManagement.Model
{
    public class CandidatePatchDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PersonalEmail { get; set; }
        public string? Phone { get; set; }
        public RecruitingStatus? RecruitingStatus { get; set; }
        public List<string>? AnswersToForm { get; set; }
        public int? SchedulerId { get; set; }
    }
}
