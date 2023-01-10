using index_book_shop.DAL;
using index_book_shop.Models;
using index_book_shop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace index_book_shop.Controllers
{
    public class HomeController : Controller
    {
        readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            HomeVM home = new HomeVM { Products = _context.Products.Include(p =>p.ProductImages).Where(p=>p.IsDeleted==false).Take(3), LastestProducts= _context.Products.Include(p => p.ProductImages).Where(p => p.IsDeleted == false).OrderByDescending(p=>p.Id)};
            return View(home);
        }
        public IActionResult Privacy()
        {
            return View();
        }
    }
}