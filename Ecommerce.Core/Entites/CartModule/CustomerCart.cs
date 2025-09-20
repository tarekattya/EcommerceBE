using Ecommerce.Core.Entites.CartModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Entites
{
    public class CUstomerCart
    {
        public string Id { get; set; } = string.Empty;

        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
