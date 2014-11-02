using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using uHome.Authorization;
using System.Security.Claims;
using Thinktecture.IdentityModel.Owin.ResourceAuthorization;

namespace uHome.Tests.Authorization
{
    [TestClass]
    public class VideoClipAuthorizationTest : AuthorizationTest
    {
        [TestInitialize]
        public void Init()
        {
            subject = new UhomeResourceAuthorizationManager();
        }

        [TestMethod]
        public void AnonymousCannotAccessVideoClip()
        {
            var ctx1 = new ResourceAuthorizationContext(Anonymous,
                UhomeResources.VideoClipActions.View, UhomeResources.VideoClip);
            Assert.IsFalse(subject.CheckAccessAsync(ctx1).Result);

            var ctx2 = new ResourceAuthorizationContext(Anonymous,
                UhomeResources.VideoClipActions.Edit, UhomeResources.VideoClip);
            Assert.IsFalse(subject.CheckAccessAsync(ctx2).Result);
        }

        [TestMethod]
        public void AuthenticatedCanViewVideoClip()
        {
            var user = User("John");
            var ctx = new ResourceAuthorizationContext(user,
                UhomeResources.VideoClipActions.View, UhomeResources.VideoClip);
            Assert.IsTrue(subject.CheckAccessAsync(ctx).Result);
        }

        [TestMethod]
        public void AuthenticatedNonAdminCannotEditVideoClip()
        {
            var roles = new string[] { "Manager", "Staff", "FreeAccount", "SilverAccount", "GoldAccount" };
            var user = User("John", roles);
            var ctx = new ResourceAuthorizationContext(user,
                UhomeResources.VideoClipActions.Edit, UhomeResources.VideoClip);
            Assert.IsFalse(subject.CheckAccessAsync(ctx).Result);
        }

        [TestMethod]
        public void AuthenticatedAdminCanEditVideoClip()
        {
            var roles = new string[] { "Admin" };
            var user = User("John", roles);
            var ctx = new ResourceAuthorizationContext(user,
                UhomeResources.VideoClipActions.Edit, UhomeResources.VideoClip);
            Assert.IsTrue(subject.CheckAccessAsync(ctx).Result);
        }
    }
}
