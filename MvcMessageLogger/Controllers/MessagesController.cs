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

        [Route("/Users/{id:int}/Messages/new")]
        public IActionResult New(int id)
        {
            var user = _context.Users.Find(id);
            return View(user);
        }

        [HttpPost]
        [Route("/Users/{id:int}/Details")]
        public IActionResult Create(int id, Message message)
        {
            var user = _context.Users.Find(id);
            message.CreatedAt = DateTime.Now.ToUniversalTime();
            _context.Messages.Add(message);
            user.Messages.Add(message);
            _context.SaveChanges();

            return Redirect($"/Users/{id}/Details");
        }
    }
}
