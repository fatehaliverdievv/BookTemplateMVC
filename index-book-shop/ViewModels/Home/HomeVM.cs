using index_book_shop.Models;
using index_book_shop.ViewModels;

namespace index_book_shop.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Product> LastestProducts { get; set; }
    }
}
