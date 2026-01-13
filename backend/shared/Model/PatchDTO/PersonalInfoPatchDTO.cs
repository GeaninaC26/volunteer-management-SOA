using System.ComponentModel.DataAnnotations;

namespace VolunteerManagement.Model
{
    public class PersonalInfoPatchDTO
    {
        public string? Address { get; set; }
        [DataType(DataType.Date)]
        public DateOnly? Birthdate { get; set; }
        public Gender? Gender { get; set; }
        public StudyType? StudyType { get; set; }
        public StudyLanguage? StudyLanguage { get; set; }
        [Range(1, 4)]
        public int? Year { get; set; }
        [RegularExpression(@"https:\/\/www\.facebook\.com\/.*")]
        public string? FacebookProfile { get; set; }
        public string? InstagramProfile { get; set; }
        [EmailAddress]
        public string? PersonalEmail { get; set; }
        public Diet? Diet { get; set; }
        public string? Allergies { get; set; }
        public ShirtSize? ShirtSize { get; set; }

        //Parameterless constructor
        public PersonalInfoPatchDTO()
        {
        }

    }
}