using System.ComponentModel.DataAnnotations;

namespace VolunteerManagement.Model
{
    public class VolunteerDisponibilityPatchDTO
    {
        public int Id { get; set; }
        [Required]
        public required int VolunteerId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public required DateTime DateTime { get; set; }
        [Required]
        public required int LocationId { get; set; }

        //Parameterless constructor
        public VolunteerDisponibilityPatchDTO()
        { 
        }
    }
}