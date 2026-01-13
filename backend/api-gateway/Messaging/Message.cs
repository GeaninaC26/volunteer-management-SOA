namespace VolunteerManagement.Messaging
{

public class Message(string command, object payload)
    {
        public string Command { get; set; } = command;
        public object Payload { get; set; } = payload;
    }
}