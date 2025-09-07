using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Abstraction
{
    public class ApiValidationResponse : Error
    {
        public IEnumerable<string> Errors { get; set; }
        public ApiValidationResponse() : base("Not Valid", "This action not valid to execute", 400)
        {
            Errors = new List<string>();
        }
    }
}
