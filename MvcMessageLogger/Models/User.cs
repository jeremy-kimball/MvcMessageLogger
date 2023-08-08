using Microsoft.EntityFrameworkCore;
using MvcMessageLogger.DataAccess;
using System.Linq;

namespace MvcMessageLogger.Models
{
    public class User
    {
        //remember to change name/username back to private set after testing
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public List<Message> Messages { get; } = new List<Message>();

        public User()
        {

        }

        public User(string name, string username)
        {
            Name = name;
            Username = username;
        }

        public static IEnumerable<KeyValuePair<string, int>> UsersOrderedByMessageCount(MvcMessageLoggerContext context)
        {
            var users = context.Users.Include(u => u.Messages);
            Dictionary<string, int> UserDict = new Dictionary<string, int>();
            foreach (var user in users)
            {
                UserDict.Add(user.Username, user.Messages.Count());
            }
            var sortedDict = UserDict.OrderByDescending(kv => kv.Value);
            return sortedDict;
        }
    }
}
