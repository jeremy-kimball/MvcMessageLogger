using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMessageLogger.DataAccess;
using MvcMessageLogger.Models;

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

        [Route("/Users/{id:int}/Details")]
        public IActionResult Show(int id)
        {
            var user = _context.Users.Include(u => u.Messages).Where(u => u.Id == id).FirstOrDefault();
            return View(user);
        }

        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        [Route("/Users")]
        public IActionResult Index(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return Redirect("/Users");
        }

        [Route("/Users/{id:int}/edit")]
        public IActionResult Edit(int id)
        {
            var user = _context.Users.Find(id);
            return View(user);
        }

        [HttpPost]
        [Route("/Users/{id:int}")]
        public IActionResult Update(int id, User user)
        {
            user.Id = id;
            _context.Users.Update(user);
            _context.SaveChanges();

            return RedirectToAction("show", new { id = user.Id });
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Where(u => u.Id == id).Include(u => u.Messages).First();
            _context.Users.Remove(user);
            _context.SaveChanges();

            return RedirectToAction("index");
        }


    }
}
