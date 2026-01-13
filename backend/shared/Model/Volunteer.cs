namespace VolunteerManagement.Model
{
    public class Volunteer
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string PersonalEmail { get; set; }
        public required string Phone { get; set; }
        public PersonalInfo PersonalInfo { get; set; }
        public string? Email { get; set; }
        public VolunteerStatus VolunteerStatus { get; set; }
        public Department Department { get; set; }

        //Parameterless constructor
        public Volunteer()
        { 
            PersonalInfo = new PersonalInfo();
        }
    }
}