using System.ComponentModel.DataAnnotations;

namespace VolunteerManagement.Model
{
    public class CandidateDTO
    {
        public int Id { get; set; }
        [Required]
        public required string FirstName { get; set; }
        [Required]
        public required string LastName { get; set; }
        [Required, EmailAddress]
        public required string PersonalEmail { get; set; }
        [Required]
        [RegularExpression(@"^\d{10}$")]
        public required string Phone { get; set; }
        public RecruitingStatus RecruitingStatus { get; set; }
        public List<string> AnswersToForm { get; set; }

        public int SchedulerId { get; set; }

        //Parameterless constructor
        public CandidateDTO()
        {
            RecruitingStatus = RecruitingStatus.Open;
            AnswersToForm = [];
        }

    }
}
