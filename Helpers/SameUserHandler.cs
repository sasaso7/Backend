using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

public class SameUserRequirement : IAuthorizationRequirement { }
public class SameUserHandler : AuthorizationHandler<SameUserRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SameUserHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return Task.CompletedTask;
        }

        var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var routeUserId = httpContext.Request.RouteValues["id"]?.ToString();
        var isAdmin = httpContext.User.IsInRole("Admin");

        if (userId == routeUserId || isAdmin)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}