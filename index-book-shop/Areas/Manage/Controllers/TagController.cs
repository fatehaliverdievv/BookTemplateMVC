using index_book_shop.DAL;
using index_book_shop.Models;
using Microsoft.AspNetCore.Mvc;

namespace index_book_shop.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class TagController : Controller
    {
        readonly AppDbContext _context;
        public TagController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Tags.ToList());
        }
        public IActionResult Delete(int id)
        {
            Tag tag = _context.Tags.Find(id);
            if (tag is null) return NotFound();
            _context.Tags.Remove(tag);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(string name)
        {
            if (!ModelState.IsValid) return View();
            _context.Tags.Add(new Tag { Name = name });
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Update(int? id)
        {
            Tag tag = _context.Tags.Find(id);
            if (tag is null) return NotFound();
            if (id is null || id == 0) BadRequest();
            return View(tag);
        }
        [HttpPost]
        public IActionResult Update(int? id, Tag tag)
        {
            if (!ModelState.IsValid) return View();
            if (id is null || id != tag.Id) BadRequest();
            Tag existedcategory = _context.Tags.Find(id);
            if (existedcategory is null) return NotFound();
            existedcategory.Name = tag.Name;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
