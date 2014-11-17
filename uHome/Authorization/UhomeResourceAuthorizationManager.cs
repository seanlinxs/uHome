using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Thinktecture.IdentityModel.Owin.ResourceAuthorization;
using uHome.Models;

namespace uHome.Authorization
{
    public class UhomeResourceAuthorizationManager : ResourceAuthorizationManager
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public override Task<bool> CheckAccessAsync(ResourceAuthorizationContext ctx)
        {
            var resource = ctx.Resource.First().Value;

            if (resource == UhomeResources.VideoClip)
            {
                return CheckAccessVideoClipAsync(ctx);
            }

            if (resource == UhomeResources.Case)
            {
                return CheckAccessCaseAsync(ctx);
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

            var action = ctx.Action.First().Value;

            if (action == UhomeResources.VideoClipActions.View)
            {
                return Ok();
            }

            if (action == UhomeResources.VideoClipActions.Edit)
            {
                if (ctx.Principal.IsInRole("Admin"))
                {
                    return Ok();
                }
            }

            return Nok();
        }

        public Task<bool> CheckAccessCaseAsync(ResourceAuthorizationContext ctx)
        {
            var user = ctx.Principal.Identity;

            if (! user.IsAuthenticated)
            {
                return Nok();
            }

            if (ctx.Principal.IsInRole("Admin") || ctx.Principal.IsInRole("Manager"))
            {
                return Ok();
            }

            if (ctx.Action.First().Value == UhomeResources.Actions.List)
            {
                return Ok();
            }

            if (ctx.Resource.Count() == 2)
            {
                var caseId = int.Parse(ctx.Resource.Skip(1).Take(1).First().Value);
                var @case = db.Cases.Find(caseId);
                var users = db.Users;
                var applicationUser = (from u in db.Users
                                      where u.UserName == ctx.Principal.Identity.Name
                                      select u).First();

                if (applicationUser.Cases.Contains(@case))
                {
                    return Ok();
                }
                else
                {
                    return Nok();
                }
            }

            return Nok();
       }
    }
}