namespace MvcMessageLogger.Models
{
    public class User
    {
        //remember to change name/username back to private set after testing
        public int Id { get; set; }
        public string Name { get; private set; }
        public string Username { get; private set; }
        public List<Message> Messages { get; } = new List<Message>();

        public User(string name, string username)
        {
            Name = name;
            Username = username;
        }
    }
}
