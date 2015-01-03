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

            if (resource == UhomeResources.User)
            {
                return CheckAccessUserAsync(ctx);
            }

            if (resource == UhomeResources.DownloadItem)
            {
                return CheckAccessDownloadItemAsync(ctx);
            }

            if (resource == UhomeResources.Event)
            {
                return CheckAccessEventAsync(ctx);
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
                var applicationUser = db.Users.Where(u => u.UserName == ctx.Principal.Identity.Name).Single();

                if (ctx.Action.First().Value == UhomeResources.Actions.Edit)
                {
                    if (@case.ApplicationUserId == applicationUser.Id)
                    {
                        return Ok();
                    }
                    else
                    {
                        return Nok();
                    }
                }
                else if (ctx.Action.First().Value == UhomeResources.Actions.StaffEdit)
                {
                    if (@case.CaseAssignment != null && @case.CaseAssignment.ApplicationUserId == applicationUser.Id)
                    {
                        return Ok();
                    }
                    else
                    {
                        return Nok();
                    }
                }
                else if (ctx.Action.First().Value == UhomeResources.Actions.View)
                {
                    if (@case.CaseAssignment != null && @case.CaseAssignment.ApplicationUserId == applicationUser.Id)
                    {
                        return Ok();
                    }
                    else if (@case.ApplicationUserId == applicationUser.Id)
                    {
                        return Ok();
                    }
                    {
                        return Nok();
                    }
                }
            }

            return Nok();
       }

        public Task<bool> CheckAccessUserAsync(ResourceAuthorizationContext ctx)
        {
            var user = ctx.Principal.Identity;

            if (ctx.Principal.IsInRole("Admin"))
            {
                return Ok();
            }

            return Nok();
        }

        public Task<bool> CheckAccessDownloadItemAsync(ResourceAuthorizationContext ctx)
        {
            var user = ctx.Principal.Identity;

            if (!user.IsAuthenticated)
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

        public Task<bool> CheckAccessEventAsync(ResourceAuthorizationContext ctx)
        {
            var user = ctx.Principal.Identity;
            var action = ctx.Action.First().Value;

            // Anyone can view event, even public user
            if (action == UhomeResources.EventActions.View)
            {
                return Ok();
            }

            if (action == UhomeResources.EventActions.Edit)
            {
                if (ctx.Principal.IsInRole("Admin"))
                {
                    return Ok();
                }
            }

            return Nok();
        }

    }
}