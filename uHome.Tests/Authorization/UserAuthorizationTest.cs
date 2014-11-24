using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using uHome.Authorization;
using System.Security.Claims;
using Thinktecture.IdentityModel.Owin.ResourceAuthorization;
using uHome.Models;
using System.Collections.Generic;
using System.Transactions;

namespace uHome.Tests.Authorization
{
    [TestClass]
    public class UserAuthorizationTest : AuthorizationTest
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private TransactionScope scope;

        private Case CreateCase()
        {
            Case @case = new Case
            {
                Title = "Case Title",
                Description = "Case Description",
                CreatedAt = System.DateTime.Now,
                State = CaseState.NEW
            };

            return @case;
        }

        [TestInitialize]
        public void Init()
        {
            subject = new UhomeResourceAuthorizationManager();
            scope = new TransactionScope();
        }

        [TestCleanup]
        public void Cleanup()
        {
            scope.Dispose();
        }

        [TestMethod]
        public void AnonymousCannotListUser()
        {
            var ctx = new ResourceAuthorizationContext(Anonymous,
                UhomeResources.Actions.List, UhomeResources.User);
            Assert.IsFalse(subject.CheckAccessAsync(ctx).Result);
        }

        [TestMethod]
        public void AnonymousCannotEditUser()
        {
            var ctx = new ResourceAuthorizationContext(Anonymous,
                UhomeResources.Actions.Edit, UhomeResources.User);
            Assert.IsFalse(subject.CheckAccessAsync(ctx).Result);
        }

        [TestMethod]
        public void AuthenticatedNonAdminCannotListUser()
        {
            var user = User("John");
            var ctx = new ResourceAuthorizationContext(user,
                UhomeResources.Actions.List, UhomeResources.User);
            Assert.IsFalse(subject.CheckAccessAsync(ctx).Result);

        }

        [TestMethod]
        public void AuthenticatedNonAdminCannotEditUser()
        {
            var user = User("John");
            var ctx = new ResourceAuthorizationContext(user,
                UhomeResources.Actions.Edit, UhomeResources.User);
            Assert.IsFalse(subject.CheckAccessAsync(ctx).Result);

        }

        [TestMethod]
        public void AuthenticatedAdminCanListUser()
        {
            var admin = User("AdminUser", new string[] { "Admin" });
            var ctx = new ResourceAuthorizationContext(admin,
                UhomeResources.Actions.List, UhomeResources.User);
            Assert.IsTrue(subject.CheckAccessAsync(ctx).Result);
        }

        [TestMethod]
        public void AuthenticatedAdminCanEditUser()
        {
            var admin = User("AdminUser", new string[] { "Admin" });
            var ctx = new ResourceAuthorizationContext(admin,
                UhomeResources.Actions.Edit, UhomeResources.User);
            Assert.IsTrue(subject.CheckAccessAsync(ctx).Result);
        }
    }
}
