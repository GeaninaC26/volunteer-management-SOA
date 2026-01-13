namespace VolunteerManagement.Model
{
    public class RecruitmentCampaign
    {
        public int Id { get; set; }

        public required string Name { get; set; }
        public required DateOnly StartDate { get; set; }
        public required DateOnly EndDate { get; set; }
        public required int InterviewTemplateId { get; set; }
        public required int RecruitmentFormTemplateId { get; set; }
        public List<Location> Locations { get; set; }
        public List<BlockedPeriod> BlockedPeriods { get; set; }
        public List<Candidate> Candidates { get; set; }
        public List<Volunteer> Volunteers { get; set; }


        // Parameterless constructor 
        public RecruitmentCampaign()
        {
            Locations = [];
            BlockedPeriods = [];
            Candidates = [];
            Volunteers = [];
        }
    }
}