using System.ComponentModel.DataAnnotations;

namespace VolunteerManagement.Model
{
    public class BlockedPeriodPatchDTO
    {
        [DataType(DataType.DateTime)]
        public required DateTime? Start { get; set; }
        [DataType(DataType.Duration)]
        public required TimeSpan? Duration { get; set; }
        public required int? LocationId { get; set; }

        public BlockedPeriodPatchDTO() { }
    }
}