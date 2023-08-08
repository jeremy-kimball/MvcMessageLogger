using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            ViewData["HourOfMostMessages"] = HourOfMostMessages(_context);
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

        public static string UsersOrderedByMessageCount(MvcMessageLoggerContext context)
        {
            var userAll = context.Users.Include(u => u.Messages).OrderByDescending(u => u.Messages.Count);
            string builtString = null;
            foreach (var u in userAll)
            {
                builtString += $"{u.Username}: {u.Messages.Count}\n";
            }
            return builtString;
        }

    }
}
