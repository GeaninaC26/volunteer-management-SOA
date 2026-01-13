namespace VolunteerManagement.Model
{
    public class PersonalInfo
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public DateOnly Birthdate { get; set; }
        public Gender Gender { get; set; }
        public StudyType StudyType { get; set; }
        public StudyLanguage StudyLanguage { get; set; }
        public int Year { get; set; }
        public string? FacebookProfile { get; set; }
        public string? InstagramProfile { get; set; }
        public Diet Diet { get; set; }
        public string? Allergies { get; set; }
        public ShirtSize ShirtSize { get; set; }

        //Parameterless constructor
        public PersonalInfo()
        {
            Address = "";
            FacebookProfile = "";
            InstagramProfile = "";
            Allergies = "";
        }
    }
}
