using System.ComponentModel.DataAnnotations;

namespace TechJockeys.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:c}")] // this applies to the property directly below
        [Range(0.01, 1000000, ErrorMessage = "Dude, can you count??")]
        public decimal Price { get; set; }

        [Range(0, 1000000)]
        public int Stock { get; set; }
        public string Description { get; set; }
        public string? Image { get; set; }

        // FK for parent CategoryId (required)
        public int CategoryId { get; set; }

        // Parent object ref.  We mark as nullable using ? for reasons we'll see later.
        public Category? Category { get; set; }
    }
}
