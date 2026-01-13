namespace VolunteerManagement.Model
{
    public class InterviewTemplate
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required List<string> Questions { get; set; }
        public required int Duration { get; set; }
        public InterviewTemplate(){}
    }
}