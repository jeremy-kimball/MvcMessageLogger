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

        public IActionResult Index()
        {
            var users = _context.Users.Include(u => u.Messages);
            ViewData["UsersOrderedByMessageCount"] = HelperMethods.UsersOrderedByMessageCount(_context);
            ViewData["MostCommonWord"] = HelperMethods.MostCommonWordByUser(_context, 1);
            ViewData["MostCommonWordOverall"] = HelperMethods.MostCommonWordOverall(_context);
            ViewData["HourOfMostMessages"] = HelperMethods.HourOfMostMessages(_context);
            return View(users);
        }
    }
}


