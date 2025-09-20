﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Entites.CartModule
{
    public class CartItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string PictureUrl { get; set; } = string.Empty;

    }
}
