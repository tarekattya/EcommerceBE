using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Abstraction.Errors
{
    public class ApiExceptionError : Error
    {
        public string ErrorId { get; }

        public ApiExceptionError(string message, string details, int statusCode , string errorId) : base(message, details, statusCode)
        {
            ErrorId = errorId;
        }
    }
}
