﻿namespace index_book_shop.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<ProductTag>? ProductTags { get; set; }
    }
}
