using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using MvcMessageLogger.DataAccess;
using MvcMessageLogger.Models;

namespace MvcMessageLogger.Testing
{
    [Collection("Async")]
    public class UsersControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public UsersControllerTests(WebApplicationFactory<Program> factory)
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
        public async Task Index_ReturnsViewWithUsers()
        {
            var context = GetDbContext();
            context.Users.Add(new User ("User1","Username1"));
            context.Users.Add(new User ("User2","Username2"));
            context.SaveChanges();

            var client = _factory.CreateClient();

            var response = await client.GetAsync("/Users");
            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains("User1", html);
            Assert.Contains("User2", html);
            Assert.DoesNotContain("User3", html);
        }

        [Fact]
        public async Task Index_ShowsLinkToNew()
        {
            //Arrange
            var client = _factory.CreateClient();

            //Act
            var response = await client.GetAsync($"/Users");
            var html = await response.Content.ReadAsStringAsync();

            //Assert
            Assert.Contains($"<a href='/Users/new'>", html);
        }

        [Fact]
        public async Task New_ReturnsForm()
        {
            //Arrange
            var client = _factory.CreateClient();
            var context = GetDbContext();

            //Act
            var response = await client.GetAsync($"/Users/new");
            var html = await response.Content.ReadAsStringAsync();

            //Assert
            Assert.Contains("label", html);
            Assert.Contains("input", html);
            Assert.Contains("User", html);
            Assert.Contains("Username", html);
            Assert.Contains($"<form method=\"post\" action=\"/Users\">", html);
        }
        [Fact]
        public async Task Create_CreatesUsersRedirectsToIndex()
        {
            var client = _factory.CreateClient();
            var context = GetDbContext();
            var addUserFormData = new Dictionary<string, string>
            {
                {"Name", "User1" },
                {"Username", "Username1" }
            };

            var response = await client.PostAsync($"/Users", new FormUrlEncodedContent(addUserFormData));
            var html = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Contains($"/Users", response.RequestMessage.RequestUri.ToString());
            Assert.Contains("User1", html);
            Assert.Contains("Username1", html);
            Assert.DoesNotContain("User2", html);
        }

        [Fact]
        public async Task Edit_DisplaysFormPrePopulated()
        {
            //Arrange
            var context = GetDbContext();
            var client = _factory.CreateClient();
            User user1 = new User { Name = "user1", Username = "username1" };
            context.Users.Add(user1);
            context.SaveChanges();

            //Act
            var response = await client.GetAsync($"/Users/{user1.Id}/edit");
            var html = await response.Content.ReadAsStringAsync();

            //Assert
            Assert.Contains(user1.Name, html);
            Assert.Contains(user1.Username, html);
            Assert.Contains("form method=\"post\"", html);
            Assert.Contains($"action=\"/Users/{user1.Id}\"", html);
        }

        [Fact]
        public async Task Update_SavesChangesToUser()
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