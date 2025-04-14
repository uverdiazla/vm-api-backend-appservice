using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using vm_api_backend_appservice.Attributes;

namespace vm_api_backend_appservice.Utils
{
    public class SwaggerAuthorizationOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Get the method info for the API method
            var methodInfo = context.MethodInfo;
            
            // Get the attributes on the method
            var allowAnonymousAttribute = methodInfo.GetCustomAttribute<vm_api_backend_appservice.Attributes.AllowAnonymousAttribute>();
            var adminOnlyAttribute = methodInfo.GetCustomAttribute<AdminOnlyAttribute>();
            
            // Check if the action has an [AllowAnonymous] attribute
            if (allowAnonymousAttribute != null)
            {
                operation.Description = operation.Description ?? "";
                operation.Description += "\n\n**Authorization: Anonymous Access**\nThis endpoint can be accessed without authentication.";
                
                // If anonymous access is allowed, we don't need to add security requirements
                return;
            }
            
            // Add security requirement (the padlock icon) if [AllowAnonymous] is not present
            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                }
            };

            // Add description about admin role requirement if the [AdminOnly] attribute is present
            if (adminOnlyAttribute != null)
            {
                operation.Description = operation.Description ?? "";
                operation.Description += "\n\n**Authorization: Admin Only**\nThis endpoint can only be accessed by users with the Admin role.";
            }
            else
            {
                operation.Description = operation.Description ?? "";
                operation.Description += "\n\n**Authorization: Authentication Required**\nThis endpoint requires authentication.";
            }
        }
    }
} 