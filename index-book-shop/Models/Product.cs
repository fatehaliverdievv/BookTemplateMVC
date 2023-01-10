using System.ComponentModel.DataAnnotations;

namespace index_book_shop.Models
{
    public class Product
    {
        public int Id { get; set; }
        [MaxLength(30)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string Description { get; set; }
        [Range(0,99999)]
        public double CostPrice { get; set; }
        [Range(0.0, 99999)]
        public double SellPrice { get; set; }
        [Range(0.0, 99999)]
        public double? DiscountPrice { get; set; }
        public string SKU { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<ProductCategory>? ProductCategories { get; set; }
        public ICollection<ProductTag>? ProductTag { get; set; }
        public ICollection<ProductImage>? ProductImages { get; set; }

    }
}
