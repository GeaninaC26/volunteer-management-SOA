namespace VolunteerManagement.Model
{
    public class BlockedPeriod
    {
        public int Id { get; set; }
        public required DateTime Start { get; set; }
        public required TimeSpan Duration { get; set; }
        public required int LocationId { get; set; }
        public required int RecruitmentCampaignId { get; set; }
        public BlockedPeriod() { }
    }
}