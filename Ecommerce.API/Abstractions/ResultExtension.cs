using Ecommerce.Shared.Abstraction;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Ecommerce.API.Abstractions
{
    public static class ResultExtension
    {

        public static ObjectResult ToProblem(this Result result)
        {

            if (result.IsSuccess)
                throw new InvalidOperationException("cannot convert succes result to problem");


            var problem = Results.Problem(statusCode:result.Error.statusCode);
            var problemDetails = problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(problem) as ProblemDetails;

            problemDetails!.Extensions = new Dictionary<string, object?>()
               {{
                       "errors",
                         new[] {  result.Error.Code , result.Error.Message}
                   }
               };
            return new ObjectResult(problemDetails);



        }
    }
}
