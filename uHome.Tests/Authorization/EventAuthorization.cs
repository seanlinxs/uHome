using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using uHome.Authorization;
using System.Security.Claims;
using Thinktecture.IdentityModel.Owin.ResourceAuthorization;

namespace uHome.Tests.Authorization
{
    [TestClass]
    public class EventAuthorizationTest : AuthorizationTest
    {
        [TestMethod]
        public void AnonymousCanViewEvent()
        {
            var ctx1 = new ResourceAuthorizationContext(Anonymous,
                UhomeResources.EventActions.View, UhomeResources.Event);
            Assert.IsTrue(subject.CheckAccessAsync(ctx1).Result);
        }

        public void AnonymousCannotEditEvent()
        {
            var ctx1 = new ResourceAuthorizationContext(Anonymous,
                UhomeResources.EventActions.Edit, UhomeResources.Event);
            Assert.IsFalse(subject.CheckAccessAsync(ctx1).Result);
        }

        [TestMethod]
        public void AuthenticatedCanViewEvent()
        {
            var user = User("John");
            var ctx = new ResourceAuthorizationContext(user,
                UhomeResources.EventActions.View, UhomeResources.Event);
            Assert.IsTrue(subject.CheckAccessAsync(ctx).Result);
        }

        [TestMethod]
        public void AuthenticatedNonAdminCannotEditEvent()
        {
            var roles = new string[] { "Manager", "Staff", "FreeAccount", "SilverAccount", "GoldAccount" };
            var user = User("John", roles);
            var ctx = new ResourceAuthorizationContext(user,
                UhomeResources.EventActions.Edit, UhomeResources.Event);
            Assert.IsFalse(subject.CheckAccessAsync(ctx).Result);
        }

        [TestMethod]
        public void AuthenticatedAdminCanEditEvent()
        {
            var roles = new string[] { "Admin" };
            var user = User("John", roles);
            var ctx = new ResourceAuthorizationContext(user,
                UhomeResources.EventActions.Edit, UhomeResources.Event);
            Assert.IsTrue(subject.CheckAccessAsync(ctx).Result);
        }
    }
}
