using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiFood.Models
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }
        public double Price { get; set; }
        public int Qty { get; set; }
        public double TotalAmount { get; set; }
        public int ProductId { get; set; }
        public int CustomerId { get; set; }

    }
}
