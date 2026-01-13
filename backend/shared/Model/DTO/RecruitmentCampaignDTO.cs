using System.ComponentModel.DataAnnotations;

namespace VolunteerManagement.Model
{
    public class RecruitmentCampaignDTO
    {
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateOnly StartDate { get; set; }
        [DataType(DataType.Date)]
        [Required]
        public DateOnly EndDate { get; set; }
        [Required]
        public required int InterviewTemplateId { get; set; }
        [Required]
        public required int RecruitmentFormTemplateId { get; set; }

        // Parameterless constructor 
        public RecruitmentCampaignDTO(){}
    }
}