using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MvcMessageLogger.DataAccess;
using MvcMessageLogger.Models;
using System.Diagnostics.Eventing.Reader;
using System.Text.RegularExpressions;
using MvcMessageLogger.Helpers;

namespace MvcMessageLogger.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly MvcMessageLoggerContext _context;

        public StatisticsController(MvcMessageLoggerContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? username)
        {
            var users = _context.Users.Include(u => u.Messages).AsEnumerable();

            if(username != null)
            {
                var user = users.Where(u => u.Username == username).First();
                ViewData["MostCommonWord"] = HelperMethods.MostCommonWordByUser(_context, user.Id);
                ViewData["SearchUsername"] = user;
            }

            ViewData["UsersOrderedByMessageCount"] = HelperMethods.UsersOrderedByMessageCount(_context);
            
            ViewData["MostCommonWordOverall"] = HelperMethods.MostCommonWordOverall(_context);
            ViewData["HourOfMostMessages"] = HelperMethods.HourOfMostMessages(_context);

            ViewData["AllUsernames"] = _context.Users.Select(u => u.Username).Distinct().ToList();

            return View(users);
        }
    }
}


