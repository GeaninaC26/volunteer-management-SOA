using System.ComponentModel.DataAnnotations;

namespace VolunteerManagement.Model
{
    public class VolunteerDTO
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
        public string? Email { get; set; }
        public VolunteerStatus VolunteerStatus { get; set; }
        public required Department Department { get; set; }
        //Parameterless constructor
        public VolunteerDTO()
        {
            VolunteerStatus = VolunteerStatus.Inactive;
        }
    }
}