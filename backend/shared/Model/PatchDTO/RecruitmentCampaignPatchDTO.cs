using System.ComponentModel.DataAnnotations;

namespace VolunteerManagement.Model
{
    public class RecruitmentCampaignPatchDTO
    {
        [DataType(DataType.Date)]
        public DateOnly? StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateOnly? EndDate { get; set; }
        // Parameterless constructor 
        public RecruitmentCampaignPatchDTO()
        {
        }
    }
}