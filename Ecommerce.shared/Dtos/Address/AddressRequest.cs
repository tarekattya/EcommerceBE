using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.shared.Dtos.Address
{
    public record AddressRequest(int Id ,string FirstName , string LastName , string Country , string City , string Street);

    
    
}
