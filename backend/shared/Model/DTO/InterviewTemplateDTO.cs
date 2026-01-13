using System.ComponentModel.DataAnnotations;

namespace VolunteerManagement.Model
{
    public class InterviewTemplateDTO
    {
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [Length(0, 100)]
        public required List<string> Questions { get; set; }
        [Range(5, 60)]
        public required int Duration { get; set; }
        public InterviewTemplateDTO(){}
    }
}