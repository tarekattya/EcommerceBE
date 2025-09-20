using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Entites.ProductModule
{
    public class ProductCategory : AuditLogging
    {
        public string Name { get; set; } = default!;
    }
}
