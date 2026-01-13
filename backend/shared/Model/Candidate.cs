namespace VolunteerManagement.Model
{
    public class Candidate
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string PersonalEmail { get; set; }
        public required string Phone { get; set; }
        public PersonalInfo PersonalInfo { get; set; }
        public RecruitingStatus RecruitingStatus { get; set; }
        public int RecruitmentCampaignId { get; set; }

        public List<string> AnswersToForm { get; set; }
        public int SchedulerId { get; set; }

        //Parameterless constructor
        public Candidate()
        {
            AnswersToForm = [];
            PersonalInfo = new PersonalInfo();
            RecruitingStatus = RecruitingStatus.Open;
        }

    }
}
