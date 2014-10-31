using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using uHome.Authorization;
using System.Security.Claims;
using Thinktecture.IdentityModel.Owin.ResourceAuthorization;

namespace uHome.Tests.Authorization
{
    [TestClass]
    public class VideoClipTest
    {
        static readonly ClaimsPrincipal Anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        IResourceAuthorizationManager subject;

        ClaimsPrincipal User(string username, string[] roles)
        {
            ClaimsIdentity ci = new ClaimsIdentity("Password");
            ci.AddClaim(new Claim(ClaimTypes.Name, username));

            foreach (string role in roles)
            {
                ci.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            return new ClaimsPrincipal(ci);
        }

        [TestInitialize]
        public void Init()
        {
            subject = new UhomeResourceAuthorizationManager();
        }

        [TestMethod]
        public void AnonymousCannotViewVideoClip()
        {
            ResourceAuthorizationContext ctx = new ResourceAuthorizationContext(
                Anonymous,
                UhomeResources.VideoClipActions.View,
                UhomeResources.VideoClip
            );

            Assert.IsFalse(subject.CheckAccessAsync(ctx).Result);
        }
    }
}
