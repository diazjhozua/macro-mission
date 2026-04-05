using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace MacroMission.Api.OpenApi;

/// <summary>
/// Adds JWT Bearer security scheme to the OpenAPI document so Scalar UI shows the Authorize button.
/// </summary>
internal sealed class BearerSecuritySchemeTransformer(
    IAuthenticationSchemeProvider authenticationSchemeProvider)
    : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        IEnumerable<AuthenticationScheme> authSchemes =
            await authenticationSchemeProvider.GetAllSchemesAsync();

        if (!authSchemes.Any(s => s.Name == JwtBearerDefaults.AuthenticationScheme))
            return;

        Dictionary<string, IOpenApiSecurityScheme> securitySchemes = new()
        {
            [JwtBearerDefaults.AuthenticationScheme] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                In = ParameterLocation.Header,
                BearerFormat = "JSON Web Token"
            }
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = securitySchemes;

        // Apply the security requirement to every operation in the document.
        foreach (OpenApiPathItem path in document.Paths.Values)
        {
            foreach (KeyValuePair<HttpMethod, OpenApiOperation> operation in path.Operations ?? [])
            {
                operation.Value.Security ??= [];
                operation.Value.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(
                        JwtBearerDefaults.AuthenticationScheme, document)] = []
                });
            }
        }
    }
}
