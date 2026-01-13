using System.ComponentModel.DataAnnotations;

namespace VolunteerManagement.Model
{
    public class BlockedPeriodDTO
    {
        public int Id { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public required DateTime Start { get; set; }
        [Required]
        [DataType(DataType.Duration)]
        public required TimeSpan Duration { get; set; }
        [Required]
        public required int LocationId { get; set; }

        public BlockedPeriodDTO() { }
    }
}