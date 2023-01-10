namespace index_book_shop.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public bool? IsPrimary { get; set; }
        public string ImgUrl { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
