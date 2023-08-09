using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MvcMessageLogger.DataAccess;
using MvcMessageLogger.Helpers;
using MvcMessageLogger.Models;

namespace MvcMessageLogger.Testing
{
    public class HelperMethodsTests
    {
        private MvcMessageLoggerContext GetDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<MvcMessageLoggerContext>();
            optionsBuilder.UseInMemoryDatabase("TestDatabase");

            var context = new MvcMessageLoggerContext(optionsBuilder.Options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            return context;
        }

        [Fact]
        public async Task HelperMethods_UsersOrderedByMessageCount_ReturnsDictionary()
        {
            var context = GetDbContext();
            var user1 = new User("User1", "Username1");
            var user2 = new User("User2", "Username2");
            var message1 = new Message("Message1");
            var message2 = new Message("Message2");
            var message3 = new Message("Message3");
            context.Users.Add(user1);
            context.Users.Add(user2);
            context.Messages.Add(message1);
            context.Messages.Add(message2);
            context.Messages.Add(message3);
            user1.Messages.Add(message1);
            user2.Messages.Add(message2);
            user2.Messages.Add(message3);
            context.SaveChanges();
            
            var testDictionary = HelperMethods.UsersOrderedByMessageCount(context).ToDictionary(group => group.Key, group => group.Value);

            Assert.Equal(1, testDictionary[user1.Username]);
            Assert.Equal(2, testDictionary[user2.Username]);
            Assert.Equal("Username2", testDictionary.Keys.First());
        }

        [Fact]
        public void HelperMethods_HourOfMostMessages_ReturnsDictionary()
        {

        }

        [Fact]
        public void HelperMethods_MostCommonWordOverall_ReturnsDictionary()
        {
            var context = GetDbContext();
            var user1 = new User("User1", "Username1");
            var user2 = new User("User2", "Username2");
            var message1 = new Message("Message1");
            var message2 = new Message("Message1");
            var message3 = new Message("Message3");
            context.Users.Add(user1);
            context.Users.Add(user2);
            context.Messages.Add(message1);
            context.Messages.Add(message2);
            context.Messages.Add(message3);
            user1.Messages.Add(message1);
            user2.Messages.Add(message2);
            user2.Messages.Add(message3);
            context.SaveChanges();

            var testDictionary = HelperMethods.MostCommonWordOverall(context).ToDictionary(group => group.Key, group => group.Value); ;

            Assert.Equal(2, testDictionary["Message1"]);
            Assert.Equal(1, testDictionary["Message3"]);
        }

        [Fact]
        public void HelperMethods_MostCommonWordByUser_ReturnsDictionary()
        {
            var context = GetDbContext();
            var user1 = new User("User1", "Username1");
            var message1 = new Message("Message1");
            var message2 = new Message("Message1");
            var message3 = new Message("Message3");
            context.Users.Add(user1);
            context.Messages.Add(message1);
            context.Messages.Add(message2);
            context.Messages.Add(message3);
            user1.Messages.Add(message1);
            user1.Messages.Add(message2);
            user1.Messages.Add(message3);
            context.SaveChanges();

            var testDictionary = HelperMethods.MostCommonWordByUser(context, user1.Id).ToDictionary(group => group.Key, group => group.Value);

            Assert.Equal(2, testDictionary["Message1"]);
            Assert.Equal(1, testDictionary["Message3"]);
        }

        [Fact]
        public void HelperMethods_SplitAndClean_RemovesSpecialChars()
        {
            string test1 = "He!llo!";
            string test2 = "He!llo! h%ow %^you";
            
            var cleaned1 = HelperMethods.SplitAndClean(test1);
            var cleaned2 = HelperMethods.SplitAndClean(test2);

            Assert.Equal("Hello", cleaned1[0]);
            Assert.Equal("Hello", cleaned2[0]);
            Assert.Equal("how", cleaned2[1]);
            Assert.Equal("you", cleaned2[2]);
        }
    }
}
