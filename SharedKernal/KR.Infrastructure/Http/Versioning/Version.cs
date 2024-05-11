using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace KR.Offering.Services.Versioning
{
    [AttributeUsage(AttributeTargets.Method)]
    public class VersionAttribute:Attribute, IActionConstraint
    {
        private readonly string version;

        public VersionAttribute(string version)
        {
            this.version = version;
        }

        public int Order => 0;

        public bool Accept(ActionConstraintContext context)
        {
            return context.RouteContext.HttpContext.Request.Headers["Accept"].Contains(version);
        }
    }
}

