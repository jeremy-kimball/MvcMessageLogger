using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMessageLogger.DataAccess;
using MvcMessageLogger.Models;

namespace MvcMessageLogger.Controllers
{
    public class MessagesController : Controller
    {
        private readonly MvcMessageLoggerContext _context;

        public MessagesController(MvcMessageLoggerContext context)
        {
            _context = context;
        }

        [Route("Users/{id:int}/Messages")]
        public IActionResult Index(int id)
        {
            var user = _context.Users.Where(u => u.Id == id).Include(u => u.Messages).First();
            return View();
        }

        [Route("/Users/{userId:int}/Messages/new")]
        public IActionResult New(int userId)
        {
            var user = _context.Users.Where(u => u.Id == userId).Include(u => u.Messages).First();
            return View(user);
        }

        [HttpPost]
        [Route("/Users/{userId:int}/Messages")]
        public IActionResult Create(int userId, Message message)
        {
            var user = _context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Messages)
                .First();
            message.CreatedAt = DateTime.Now.ToUniversalTime();
            user.Messages.Add(message);
            _context.SaveChanges();
            return Redirect($"/Users/{userId}/Details");
        }
    }
}
