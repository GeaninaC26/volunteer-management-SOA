using System.ComponentModel.DataAnnotations;

namespace VolunteerManagement.Model
{
    public class PersonalInfoDTO
    {
        [Required]
        public string Address { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateOnly Birthdate { get; set; }
        [Required]
        public Gender Gender { get; set; }
        [Required]
        public StudyType StudyType { get; set; }
        [Required]
        public StudyLanguage StudyLanguage { get; set; }
        [Required, Range(1, 4)]
        public int Year { get; set; }
        [RegularExpression(@"https:\/\/www\.facebook\.com\/.*")]
        public string? FacebookProfile { get; set; }
        public string? InstagramProfile { get; set; }
        [Required]
        public Diet Diet { get; set; }
        public string? Allergies { get; set; }
        [Required]
        public ShirtSize ShirtSize { get; set; }

        //Parameterless constructor
        public PersonalInfoDTO()
        {
            Address = string.Empty;
        }
    }
}
