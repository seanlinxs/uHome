using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using uHome.Authorization;
using System.Security.Claims;
using Thinktecture.IdentityModel.Owin.ResourceAuthorization;

namespace uHome.Tests.Authorization
{
    [TestClass]
    public class DownloadItemAuthorizationTest : AuthorizationTest
    {
        [TestMethod]
        public void AnonymousCannotAccessDownloadItem()
        {
            var ctx1 = new ResourceAuthorizationContext(Anonymous,
                UhomeResources.DownloadItemActions.View, UhomeResources.DownloadItem);
            Assert.IsFalse(subject.CheckAccessAsync(ctx1).Result);

            var ctx2 = new ResourceAuthorizationContext(Anonymous,
                UhomeResources.DownloadItemActions.Edit, UhomeResources.DownloadItem);
            Assert.IsFalse(subject.CheckAccessAsync(ctx2).Result);
        }

        [TestMethod]
        public void AuthenticatedCanViewDownloadItem()
        {
            var user = User("John");
            var ctx = new ResourceAuthorizationContext(user,
                UhomeResources.DownloadItemActions.View, UhomeResources.DownloadItem);
            Assert.IsTrue(subject.CheckAccessAsync(ctx).Result);
        }

        [TestMethod]
        public void AuthenticatedNonAdminCannotEditDownloadItem()
        {
            var roles = new string[] { "Manager", "Staff", "FreeAccount", "SilverAccount", "GoldAccount" };
            var user = User("John", roles);
            var ctx = new ResourceAuthorizationContext(user,
                UhomeResources.DownloadItemActions.Edit, UhomeResources.DownloadItem);
            Assert.IsFalse(subject.CheckAccessAsync(ctx).Result);
        }

        [TestMethod]
        public void AuthenticatedAdminCanEditDownloadItem()
        {
            var roles = new string[] { "Admin" };
            var user = User("John", roles);
            var ctx = new ResourceAuthorizationContext(user,
                UhomeResources.DownloadItemActions.Edit, UhomeResources.DownloadItem);
            Assert.IsTrue(subject.CheckAccessAsync(ctx).Result);
        }
    }
}
