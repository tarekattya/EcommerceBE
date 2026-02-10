using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ecommerce.API.Filters;

/// <summary>
/// Ensures Bearer security is applied to every operation so Swagger UI sends the token.
/// </summary>
public class SwaggerSecurityDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument document, DocumentFilterContext context)
    {
        var schemeRef = new OpenApiSecuritySchemeReference("Bearer");
        var requirement = new OpenApiSecurityRequirement { [schemeRef] = [] };

        if (document.Paths == null) return;
        foreach (var pathItem in document.Paths.Values)
        {
            if (pathItem.Operations == null) continue;
            foreach (var operation in pathItem.Operations.Values)
            {
                operation.Security ??= new List<OpenApiSecurityRequirement>();
                operation.Security.Add(requirement);
            }
        }
    }
}
