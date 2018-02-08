using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleUI2.Models
{
    public class ProductGet : Product
    {
        public int Count { get; set; }
        public List<Product> Products { get; set; }

    }
}
