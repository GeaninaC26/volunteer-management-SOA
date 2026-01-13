namespace VolunteerManagement.Model
{
    public class VolunteerDisponibility
    {
        public int Id { get; set; }
        public required int VolunteerId { get; set; }
        public required DateTime DateTime { get; set; }
        public required int LocationId { get; set; }

        //Parameterless constructor
        public VolunteerDisponibility()
        { 
        }
    }
}