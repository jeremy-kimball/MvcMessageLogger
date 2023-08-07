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
            context.Users.Add(new User ("User1", "Username2"));
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
            Assert.Contains($"<form method=\"post\" action=\"/Users/index\">", html);
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

            var response = await client.PostAsync($"/Users/index", new FormUrlEncodedContent(addUserFormData));
            var html = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Contains($"/Users/index", response.RequestMessage.RequestUri.ToString());
            Assert.Contains("User1", html);
            Assert.Contains("Username1", html);
            Assert.DoesNotContain("User2", html);
        }
    }
}