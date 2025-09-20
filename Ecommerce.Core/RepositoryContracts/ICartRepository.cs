using Ecommerce.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.RepositoryContracts
{
    public interface ICartRepository
    {
        public Task<CUstomerCart> GetCart(string key);

        public Task<CUstomerCart> UpdateOrCreateCart(CUstomerCart cart , TimeSpan? time = null);

        public Task<bool> DeleteCart(string id);


    }
}
