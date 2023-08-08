using Microsoft.EntityFrameworkCore;
using MvcMessageLogger.DataAccess;
using System.Text.RegularExpressions;

namespace MvcMessageLogger.Helpers
{
    public static class HelperMethods
    {
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

        public static Dictionary<string, int> HourOfMostMessages(MvcMessageLoggerContext context)
        {
            Dictionary<string, int> timeMessageCount = new Dictionary<string, int>();
            var messageList = context.Messages;
            //initialize key value pairs
            foreach (var message in messageList)
            {
                if (!timeMessageCount.ContainsKey(message.CreatedAt.ToLocalTime().ToString("h tt")))
                {
                    timeMessageCount.Add(message.CreatedAt.ToLocalTime().ToString("h tt"), 0);
                }
            }
            //Where key matches add to count of value for that hour
            foreach (var kvp in timeMessageCount)
            {
                foreach (var message in messageList)
                {
                    if (kvp.Key == message.CreatedAt.ToLocalTime().ToString("h tt"))
                    {
                        timeMessageCount[kvp.Key] += 1;
                    }
                }
            }
            return timeMessageCount;
        }

        public static Dictionary<string, int> MostCommonWordOverall(MvcMessageLoggerContext context)
        {
            var users = context.Users.Include(u => u.Messages);
            var wordCount = new Dictionary<string, int>();
            foreach (var user in users)
            {
                foreach (var message in user.Messages)
                {
                    foreach (var word in SplitAndClean(message.Content))
                    {
                        if (!wordCount.ContainsKey(word))
                        {
                            wordCount.Add(word, 1);
                        }
                        else if (wordCount.ContainsKey(word))
                        {
                            wordCount[word] += 1;
                        }
                    }
                }
            }
            return wordCount;
        }

        public static Dictionary<string, int> MostCommonWordByUser(MvcMessageLoggerContext context, int id)
        {
            var user = context.Users.Include(u => u.Messages).Where(u => u.Id == id).First();
            var wordCount = new Dictionary<string, int>();
            foreach (var message in user.Messages)
            {
                foreach (var word in SplitAndClean(message.Content))
                {
                    if (!wordCount.ContainsKey(word))
                    {
                        wordCount.Add(word, 1);
                    }
                    else if (wordCount.ContainsKey(word))
                    {
                        wordCount[word] += 1;
                    }
                }
            }
            return wordCount;
        }

        public static List<string> SplitAndClean(string words)
        {
            var wordsSplit = words.Split();
            var cleanList = new List<string>();
            foreach (var word in wordsSplit)
            {
                cleanList.Add(Regex.Replace(word, "[!\"#$%&'()*+,-./:;<=>?@\\[\\]^_`{|}~]", string.Empty));
            }
            return cleanList;
        }
    }
}


//Stats page goals

//[]How many messages each user has written
// - Search bar? search bar above message highscores to filter for an exact user
// - 
//[X]Users ordered by number of messages created (most to least) <-- Message Highscores
// - build dictionary, key - user, value - count of messages

//[X]Most commonly used word for messages (by user and overall)
// -

//[X]The hour with the most messages
// - convert universal time to local, create ranges in hour increments, count messages for that range










//Previous attempt at counting messages by hour
//public static Dictionary<int, string> Hourly(MvcMessageLoggerContext context)
//{
//    Dictionary<int, string> timeMessageCount = new Dictionary<int, string>();
//    var messageList = context.Messages;
//    var sortedList = messageList
//        .GroupBy
//        (m => m.CreatedAt.ToLocalTime().Hour).ToDictionary(group => group.Key, group => group.First());
//    foreach(var kv in sortedList)
//    {
//        timeMessageCount.Add(Convert.ToInt32(kv.Key), kv.Value.CreatedAt.ToLocalTime().ToString("h tt"));
//    }
//    return timeMessageCount;

//}

//Most common word method from old project
//public static string MostCommonWord(MvcMessageLoggerContext context, int minUsageNum)
//{
//    List<string> allMessageStrings = context.Messages.Select(m => m.Content).ToList();
//    List<string> singleWordList = new List<string>();
//    foreach (string messageString in allMessageStrings)
//    {
//        var splitString = messageString.Split(" ");
//        singleWordList.AddRange(splitString);
//    }
//    string builtString = null;
//    foreach (string word in singleWordList.Distinct())
//    {
//        if (singleWordList.Where(w => w == word).Count() < minUsageNum)
//        {
//            continue;
//        }
//        else
//        {
//            builtString += $"'{word}' occurs {singleWordList.Where(w => w == word).Count()} times.\n";
//        }
//    }
//    return builtString;
//}