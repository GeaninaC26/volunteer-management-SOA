using System.ComponentModel.DataAnnotations;

namespace VolunteerManagement.Model
{
    public class RecruitmentFormTemplatePatchDTO
    {
        public string? Name { get; set; }
        [Length(0, 100)]
        public List<string>? Questions { get; set; }
        public RecruitmentFormTemplatePatchDTO()
        {
        }

    }
}