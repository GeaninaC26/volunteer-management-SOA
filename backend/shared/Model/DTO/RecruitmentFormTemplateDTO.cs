using System.ComponentModel.DataAnnotations;

namespace VolunteerManagement.Model
{
    public class RecruitmentFormTemplateDTO
    {
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        [Length(0, 100)]
        public List<string> Questions { get; set; }
        public RecruitmentFormTemplateDTO()
        {
            Questions = [];
        }
    }
}