using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using MvcMessageLogger.DataAccess;
using MvcMessageLogger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MvcMessageLogger.Testing
{
    public class StatisticsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        
        private readonly WebApplicationFactory<Program> _factory;

        public StatisticsControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

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
        public async Task Index_ShowsStatistics()
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


            var client = _factory.CreateClient();

            
            var response = await client.GetAsync("/Statistics");
            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains("<h1>Statistics</h1>", html);
            Assert.Contains("<h3>Users Ordered by Message Count</h3>", html);
        }
    }
}
