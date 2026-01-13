using System.ComponentModel.DataAnnotations;

namespace VolunteerManagement.Model
{
    public class LocationDTO
    {
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string Address { get; set; }
        public LocationDTO(){}
    }
}

