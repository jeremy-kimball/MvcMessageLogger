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
    [Collection("Async2")]
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
            Assert.Contains($"<form method=\"post\" action=\"/Users/{user1.Id}/Details\">", html);
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

            var response = await client.PostAsync($"/Users/{user1.Id}/Details", new FormUrlEncodedContent(addMessageFormData));
            var html = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Contains($"/Users/{user1.Id}/Details", response.RequestMessage.RequestUri.ToString());
            Assert.Contains("Hello!", html);
            Assert.DoesNotContain("Goodbye!", html);
        }


        
    }
}