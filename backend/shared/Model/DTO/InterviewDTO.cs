using System.ComponentModel.DataAnnotations;

namespace VolunteerManagement.Model
{
    public class InterviewDTO
    {
        public int Id { get; set; }
        public List<Volunteer> Interviewers { get; set; }
        [Required]
        public required int CandidateId { get; set; }
        public List<string>? Answers { get; set; }
        public int? LocationId { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateTime { get; set; }
        public string? Notes { get; set; }

        public InterviewDTO()
        {
            Interviewers = [];
            Answers = [];
        }
    }
}