using Microsoft.AspNetCore.Mvc;
using MvcMessageLogger.DataAccess;

namespace MvcMessageLogger.Controllers
{
    public class UsersController : Controller
    {
        private readonly MvcMessageLoggerContext _context;

        public UsersController(MvcMessageLoggerContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            var users = _context.Users;
            return View(users);
        }
    }
}
