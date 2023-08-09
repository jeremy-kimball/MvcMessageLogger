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

        [HttpPost]
        [Route("/Users/{userId:int}/Messages/Delete/{messageId:int}")]
        public IActionResult Delete(int userId, int messageId)
        {
            var message = _context.Messages.Find(messageId);
            _context.Messages.Remove(message);
            _context.SaveChanges();

            return Redirect($"/Users/{userId}/Details");
        }

        [Route("/Users/{userId:int}/Messages/{messageId}/Edit")]
        public IActionResult Edit(int userId, int messageId)
        {
            var user = _context.Users.Find(userId);
            var message = _context.Messages.Find(messageId);
            message.User = user;
            return View(message);
        }

        [HttpPost]
        [Route("/Users/{userId:int}/Messages/{messageId:int}")]
        public IActionResult Update(int userId, int messageId, Message message)
        {
            message.Id = messageId;
            message.CreatedAt = DateTime.Now.ToUniversalTime();
            _context.Messages.Update(message);
            _context.SaveChanges();
            return Redirect($"/Users/{userId}/Details");
        }

    }
}
