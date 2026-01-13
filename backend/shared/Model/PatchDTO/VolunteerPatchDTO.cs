using System.ComponentModel.DataAnnotations;

namespace VolunteerManagement.Model
{
    public class VolunteerPatchDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [EmailAddress]
        public string? PersonalEmail { get; set; }
           [RegularExpression(@"^\d{10}$")]
        public string? Phone { get; set; }
        public VolunteerStatus? VolunteerStatus { get; set; }
        public Department? Department { get; set; }

        //Parameterless constructor
        public VolunteerPatchDTO()
        {
        }
    }
}