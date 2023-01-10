using System.ComponentModel.DataAnnotations;

namespace index_book_shop.ViewModels
{
    public class CreateProductVM
    {
        [MaxLength(30)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string Description { get; set; }
        public double CostPrice { get; set; }
        [Range(0.0, 99999)]
        public double SellPrice { get; set; }
        [Range(0.0, 99999)]
        public double? DiscountPrice { get; set; }
        public IFormFile PrimaryImage { get; set; }
        public IFormFile? SecondaryImage { get; set; }
        public ICollection<IFormFile>? OtherImages { get; set; }
        public List<int> TagIds { get; set; }
        public List<int> CategoryIds { get; set; }
    }
}
