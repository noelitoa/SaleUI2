using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SaleUI2.Models
{
    public class SaleEntry
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Product SKU")]
        public string ProductSKU { get; set; }

        [Required]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Display(Name = "Product Description")]
        public string ProductDescription { get; set; }

        [Required]
        [Display(Name = "Price")]
        public decimal ProductPrice { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public string Branch { get; set; }

        [Required]
        [Display(Name = "Sold By")]
        public string SoldBy { get; set; }

        [Required]
        [Display(Name = "Sale Date")]
        [DataType(DataType.DateTime)]
        public DateTime SaleDate { get; set; }

        public DateTime TimeStamp { get; set; }

        [Required]
        [Display(Name = "Encoded By")]
        public string EncodedBy { get; set; }

    }
}
