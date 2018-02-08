using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SaleUI2.Models
{
    public class Product
    {
        public string Id { get; set; }
        [Required]
        public string ProductSKU { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public decimal Price { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime TimeStamp { get; set; }
        public string EncodedBy { get; set; }
        public string LazadaSKU { get; set; }
    }
}
