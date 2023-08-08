using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MvcMessageLogger.DataAccess;
using MvcMessageLogger.Models;

namespace MvcMessageLogger.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly MvcMessageLoggerContext _context;

        public StatisticsController(MvcMessageLoggerContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var users = _context.Users.Include(u => u.Messages);
            ViewData["UsersOrderedByMessageCount"] = UsersOrderedByMessageCount(_context);
            ViewData["MostCommonWord"] = MostCommonWord(_context, 0);
            ViewData["HourOfMostMessages"] = Hourlytwo(_context);
            return View(users);
        }


        public static string HourOfMostMessages(MvcMessageLoggerContext context)
        {
            //count messages that occur between range of time (1 hour)
            //move through each hour from first occurance to last occurence
            var finalHour = context.Messages.Max(m => m.CreatedAt.Hour) + 1;
            string builtString = null;
            for (var firstHour = context.Messages.Min(m => m.CreatedAt.Hour); firstHour < finalHour; firstHour++)
            {
                var currentHour = firstHour;
                var nextHour = firstHour + 1;
                var totalMessages = context.Messages.Where(m => m.CreatedAt.Hour < nextHour && m.CreatedAt.Hour >= currentHour).Count();
                builtString += $"{TwelveHourTime(currentHour)}: {totalMessages}\n";
            }
            return builtString;
        }

        public static string TwelveHourTime(int hour)
        {
            return
                (
                hour < 12 ? $"{hour} AM" :
                hour == 12 ? $"{hour} PM" :
                hour == 0 ? $"{hour + 12} AM" :
                $"{hour - 12} PM"
                );
        }

        public static string MostCommonWord(MvcMessageLoggerContext context, int minUsageNum)
        {
            List<string> allMessageStrings = context.Messages.Select(m => m.Content).ToList();
            List<string> singleWordList = new List<string>();
            foreach (string messageString in allMessageStrings)
            {
                var splitString = messageString.Split(" ");
                singleWordList.AddRange(splitString);
            }
            string builtString = null;
            foreach (string word in singleWordList.Distinct())
            {
                if (singleWordList.Where(w => w == word).Count() < minUsageNum)
                {
                    continue;
                }
                else
                {
                    builtString += $"'{word}' occurs {singleWordList.Where(w => w == word).Count()} times.\n";
                }
            }
            return builtString;
        }

        public static IEnumerable<KeyValuePair<string, int>> UsersOrderedByMessageCount(MvcMessageLoggerContext context)
        {
            var users = context.Users.Include(u => u.Messages);
            Dictionary<string, int> UserDict = new Dictionary<string, int>();
            foreach(var user in users)
            {
                UserDict.Add(user.Username, user.Messages.Count());
            }
            var sortedDict = UserDict.OrderByDescending(kv => kv.Value);
            return sortedDict;
        }

        public static Dictionary<int, string> Hourly(MvcMessageLoggerContext context)
        {
            Dictionary<int, string> timeMessageCount = new Dictionary<int, string>();
            var messageList = context.Messages;
            var sortedList = messageList
                .GroupBy
                (m => m.CreatedAt.ToLocalTime().Hour).ToDictionary(group => group.Key, group => group.First());
            foreach(var kv in sortedList)
            {
                timeMessageCount.Add(Convert.ToInt32(kv.Key), kv.Value.CreatedAt.ToLocalTime().ToString("h tt"));
            }
            return timeMessageCount;

        }
        public static Dictionary<string, int> Hourlytwo(MvcMessageLoggerContext context)
        {
            Dictionary<string, int> timeMessageCount = new Dictionary<string, int>();
            var messageList = context.Messages;
            //initialize key value pairs
            foreach (var message in messageList)
            {
                if(!timeMessageCount.ContainsKey(message.CreatedAt.ToLocalTime().ToString("h tt")))
                {
                    timeMessageCount.Add(message.CreatedAt.ToLocalTime().ToString("h tt"), 0);
                }
            }
            //currently working on messages not counting correctly, times are shown correctly but the value isnt being added up (ie 11am show only 1 message create when there is actually 3 messages at 11am) 
            foreach(var kvp in timeMessageCount)
            {
                foreach(var message in messageList)
                {
                    if(kvp.Key == message.CreatedAt.ToLocalTime().ToString("h tt"))
                    {
                        timeMessageCount[kvp.Key] += 1;
                    }
                }
            }
            return timeMessageCount;

        }

        }
}


//Stats page goals

//[]How many messages each user has written
// - Search bar? search bar above message highscores to filter for an exact user

//[X]Users ordered by number of messages created (most to least) <-- Message Highscores
// - build dictionary, key - user, value - count of messages

//[]Most commonly used word for messages (by user and overall)
// -

//[]The hour with the most messages
// - convert universal time to local, create ranges in hour increments, count messages for that range
