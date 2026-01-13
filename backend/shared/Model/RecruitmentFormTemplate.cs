namespace VolunteerManagement.Model
{
    public class RecruitmentFormTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<string> Questions { get; set; }
        public RecruitmentFormTemplate()
        {
            Name = string.Empty;
            Questions = new List<string>();
        }

    }
}