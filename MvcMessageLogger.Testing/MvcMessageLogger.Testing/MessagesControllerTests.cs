using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using MvcMessageLogger.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcMessageLogger.Models;

namespace MvcMessageLogger.Testing
{
    [Collection("Async")]
    public class MessagesControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public MessagesControllerTests(WebApplicationFactory<Program> factory)
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
        public async Task New_ReturnsViewWithForm()
        {
            //Arrange
            var client = _factory.CreateClient();
            var context = GetDbContext();
            var user1 = new User("User1", "Username1");
            context.Users.Add(user1);
            context.SaveChanges();
            //Act
            var response = await client.GetAsync($"/Users/{user1.Id}/Messages/New");
            var html = await response.Content.ReadAsStringAsync();

            //Assert
            Assert.Contains("label", html);
            Assert.Contains("input", html);
            Assert.Contains("Content", html);
            Assert.Contains($"<form method=\"post\" action=\"/Users/{user1.Id}/Messages\">", html);
        }

        [Fact]
        public async Task Create_PostsFormData()
        {
            var client = _factory.CreateClient();
            var context = GetDbContext();
            var user1 = new User("User1", "Username1");
            context.Users.Add(user1);
            context.SaveChanges();

            var addMessageFormData = new Dictionary<string, string>
            {
                {"Content", "Hello!" },
            };

            var response = await client.PostAsync($"/Users/{user1.Id}/Messages", new FormUrlEncodedContent(addMessageFormData));
            var html = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Contains($"/Users/{user1.Id}/Details", response.RequestMessage.RequestUri.ToString());
            Assert.Contains("Hello!", html);
            Assert.DoesNotContain("Goodbye!", html);
        }

        [Fact]
        public async Task Edit_DisplaysFormPrePopulated()
        {
            //Arrange
            var context = GetDbContext();
            var client = _factory.CreateClient();
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

            //Act
            var response = await client.GetAsync($"/Users/{user1.Id}/Messages/{message1.Id}/Edit");
            var html = await response.Content.ReadAsStringAsync();

            //Assert
            Assert.Contains("Message1", html);
        }

        [Fact]
        public async Task Update_SavesChangesToMessage()
        {
            // Arrange
            var context = GetDbContext();
            var client = _factory.CreateClient();

            User user1 = new User { Name = "user1", Username = "username1" };
            context.Users.Add(user1);
            context.SaveChanges();

            var formData = new Dictionary<string, string>
                {
                    { "Name", "user100" },
                    { "Username", "username100" }
                };

            // Act
            var response = await client.PostAsync($"/Users/{user1.Id}", new FormUrlEncodedContent(formData));
            var html = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("user100", html);
            Assert.Contains("username100", html);
        }

    }
}