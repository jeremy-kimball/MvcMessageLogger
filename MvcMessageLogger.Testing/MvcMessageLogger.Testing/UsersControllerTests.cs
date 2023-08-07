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
        public void Index_ReturnsViewWithUsers()
        {
            var context = GetDbContext();
            context.Users.Add(new User ("User1", "Username2"));
            context.Users.Add(new User ("User2","Username2"));
            context.SaveChanges();

            var client = _factory.CreateClient();

            var response = await client.GetAsync("/Users");
            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains("Spaceballs", html);
            Assert.Contains("Young Frankenstein", html);

            // Make sure it does not hit actual database
            Assert.DoesNotContain("Elf", html);
        }
    }
}