using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Thinktecture.IdentityModel.Owin.ResourceAuthorization;

namespace uHome.Authorization
{
    public class UhomeResourceAuthorizationManager : ResourceAuthorizationManager
    {
        public override Task<bool> CheckAccessAsync(ResourceAuthorizationContext ctx)
        {
            var resource = ctx.Resource.First().Value;

            if (resource == UhomeResources.VideoClip)
            {
                return CheckAccessVideoClipAsync(ctx);
            }

            return Nok();
        }

        public Task<bool> CheckAccessVideoClipAsync(ResourceAuthorizationContext ctx)
        {
            var user = ctx.Principal.Identity;

            if (! user.IsAuthenticated)
            {
                return Nok();
            }

            return Nok();
        }
    }
}