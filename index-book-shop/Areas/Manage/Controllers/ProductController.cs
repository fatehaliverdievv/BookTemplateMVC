using index_book_shop.DAL;
using index_book_shop.Models;
using index_book_shop.Utilies.Extension;
using index_book_shop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;

namespace index_book_shop.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View(_context.Products.Include(p => p.ProductCategories).ThenInclude(pc => pc.Category).Include(p => p.ProductTag).ThenInclude(p => p.Tag).Include(p => p.ProductImages).ToList().Where(p => p.IsDeleted == false));
        }
        public IActionResult DeletedProduct()
        {
            return View(_context.Products.Include(p => p.ProductCategories).ThenInclude(pc => pc.Category).Include(p => p.ProductTag).ThenInclude(pt => pt.Tag).Include(p => p.ProductImages).ToList().Where(p => p.IsDeleted == true));
        }
        public IActionResult DeletePermanently(int? id)
        {
            if (id is null || id == 0) return NotFound();
            Product product = _context.Products.Find(id);
            if (product is null) return NotFound();
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(DeletedProduct));
        }
        public IActionResult Delete(int? id)
        {
            if (id is null || id == 0) return NotFound();
            Product existed = _context.Products.Include(p => p.ProductImages).Include(p => p.ProductTag).FirstOrDefault(p => p.Id == id);
            if (existed == null) return NotFound();
            foreach (ProductImage image in existed.ProductImages)
            {
                image.ImgUrl.DeleteFile(_webHostEnvironment.WebRootPath, "assets/img/product");
            }
            _context.ProductImages.RemoveRange(existed.ProductImages);
            existed.IsDeleted = true;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
            ViewBag.Tags = new SelectList(_context.Tags, nameof(Tag.Id), nameof(Tag.Name));
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateProductVM productVM)
        {
            var primaryImg = productVM.PrimaryImage;
            var secondaryImg = productVM.SecondaryImage;
            var otherImg = productVM.OtherImages ?? new List<IFormFile>();
            string result = primaryImg?.CheckValidate("image/", 300);
            if (result?.Length > 0)
            {
                ModelState.AddModelError("PrimaryImage", result);
            }
            result = secondaryImg?.CheckValidate("image/", 300);
            if (result?.Length > 0)
            {
                ModelState.AddModelError("SecondaryImage", result);
            }
            foreach (IFormFile image in otherImg)
            {
                result = image?.CheckValidate("image/", 300);
                if (result?.Length > 0)
                {
                    ModelState.AddModelError("OtherImages", result);
                }
            }
            if (productVM.TagIds != null)
            {
                foreach (int tagid in productVM.TagIds)
                {
                    if (!_context.Tags.Any(t => t.Id == tagid))
                    {
                        ModelState.AddModelError("TagIds", "Bele bir tag yoxdu :F");
                        break;
                    }
                }
            }
            if (productVM.CategoryIds != null)
            {
                foreach (int categoryid in productVM.CategoryIds)
                {
                    if (!_context.Categories.Any(c => c.Id == categoryid))
                    {
                        ModelState.AddModelError("CategoryIds", "Bele bir reng yoxdu :F");
                        break;
                    }
                }
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
                ViewBag.Tags = new SelectList(_context.Tags, nameof(Tag.Id), nameof(Tag.Name));
                return View();
            }
            var tags = _context.Tags.Where(t => productVM.TagIds.Contains(t.Id));
            var categories = _context.Categories.Where(ca => productVM.CategoryIds.Contains(ca.Id));
            Product newProduct = new Product
            {
                Name = productVM.Name,
                CostPrice = productVM.CostPrice,
                SellPrice = productVM.SellPrice,
                DiscountPrice = productVM.DiscountPrice,
                Description = productVM.Description,
                IsDeleted = false,
                SKU = "XXX"
            };
            List<ProductImage> images = new List<ProductImage>();
            if (primaryImg != null)
            {
                images.Add(new ProductImage { ImgUrl = primaryImg.SaveFile(Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "product")), IsPrimary = true, Product = newProduct });
            }

            if (secondaryImg != null)
            {
                images.Add(new ProductImage { ImgUrl = secondaryImg.SaveFile(Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "product")), IsPrimary = false, Product = newProduct });
            }
            foreach (var item in otherImg)
            {
                images.Add(new ProductImage { ImgUrl = item.SaveFile(Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "product")), IsPrimary = null, Product = newProduct });
            }
            newProduct.ProductImages = images;
            foreach (var item in tags)
            {
                _context.ProductTags.Add(new ProductTag { Product = newProduct, TagId = item.Id });
            }
            foreach (var item in categories)
            {
                _context.ProductCategories.Add(new ProductCategory { Product = newProduct, CategoryId = item.Id });
            }
            _context.Products.Add(newProduct);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Update(int? id)
        {
            //ViewBag.Categories = _context.Categories;
            //ViewBag.Colors = _context.Tags;
            Product product = _context.Products.Find(id);
            UpdateProductVM productVM=new UpdateProductVM();
            productVM.Name = product.Name;
            productVM.Description = product.Description;
            productVM.CostPrice = product.CostPrice;
            productVM.DiscountPrice = product.DiscountPrice;
            //foreach (var item in product.ProductCategories.Include(produ).Where(product.Id==).ToList())
            //{
            //    productVM.CategoryIds.Where(productVM.Id == item.Id).Add(item.CategoryId);
            //}
            //productVM.CategoryIds = product.ProductCategories.ToList();
            if (product is null) return NotFound();
            if (id is null || id == 0) BadRequest();
            return View(productVM);
        }
        [HttpPost]
        public IActionResult Update(int? id , UpdateProductVM productVm)
        {
            if (!ModelState.IsValid) return View();
            if (id is null || id != productVm.Id) BadRequest();
            Product existedproduct = _context.Products.Find(id);
            if (existedproduct is null) return NotFound();
            var primaryImg = productVm.PrimaryImage;
            var secondaryImg = productVm.SecondaryImage;
            var otherImg = productVm.OtherImages ?? new List<IFormFile>();
            string result = primaryImg?.CheckValidate("image/", 300);
            if (result?.Length > 0)
            {
                ModelState.AddModelError("PrimaryImage", result);
            }
            result = secondaryImg?.CheckValidate("image/", 300);
            if (result?.Length > 0)
            {
                ModelState.AddModelError("SecondaryImage", result);
            }
            foreach (IFormFile image in otherImg)
            {
                result = image?.CheckValidate("image/", 300);
                if (result?.Length > 0)
                {
                    ModelState.AddModelError("OtherImages", result);
                }
            }
            if (productVm.TagIds != null)
            {
                foreach (int tagid in productVm.TagIds)
                {
                    if (!_context.Tags.Any(t => t.Id == tagid))
                    {
                        ModelState.AddModelError("TagIds", "Bele bir tag yoxdu :F");
                        break;
                    }
                }
            }
            if (productVm.CategoryIds != null)
            {
                foreach (int categoryid in productVm.CategoryIds)
                {
                    if (!_context.Categories.Any(c => c.Id == categoryid))
                    {
                        ModelState.AddModelError("CategoryIds", "Bele bir reng yoxdu :F");
                        break;
                    }
                }
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
                ViewBag.Tags = new SelectList(_context.Tags, nameof(Tag.Id), nameof(Tag.Name));
                return View();
            }
            var tags = _context.Tags.Where(t => productVm.TagIds.Contains(t.Id));
            var categories = _context.Categories.Where(ca => productVm.CategoryIds.Contains(ca.Id));
            Product product = new Product
            {
                Name = productVm.Name,
                CostPrice = productVm.CostPrice,
                SellPrice = productVm.SellPrice,
                DiscountPrice = productVm.DiscountPrice,
                Description = productVm.Description,
                IsDeleted = false,
                SKU = "XXX"
            };
            List<ProductImage> images = new List<ProductImage>();
            if (primaryImg != null)
            {
                images.Add(new ProductImage { ImgUrl = primaryImg.SaveFile(Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "product")), IsPrimary = true, Product = product });
            }

            if (secondaryImg != null)
            {
                images.Add(new ProductImage { ImgUrl = secondaryImg.SaveFile(Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "product")), IsPrimary = false, Product = product });
            }
            foreach (var item in otherImg)
            {
                images.Add(new ProductImage { ImgUrl = item.SaveFile(Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "product")), IsPrimary = null, Product = product });
            }
            product.ProductImages = images;
            foreach (var item in tags)
            {
                _context.ProductTags.Add(new ProductTag { Product = product, TagId = item.Id });
            }
            foreach (var item in categories)
            {
                _context.ProductCategories.Add(new ProductCategory { Product = product, CategoryId = item.Id });
            }
            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
