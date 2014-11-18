﻿using System;
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
    public class CaseAuthorizationTest : AuthorizationTest
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
                State = CaseState.OPEN
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
        public void AnonymousCannotAccessCase()
        {
            var ctx1 = new ResourceAuthorizationContext(Anonymous,
                UhomeResources.Actions.View, UhomeResources.Case);
            Assert.IsFalse(subject.CheckAccessAsync(ctx1).Result);

            var ctx2 = new ResourceAuthorizationContext(Anonymous,
                UhomeResources.Actions.Edit, UhomeResources.Case);
            Assert.IsFalse(subject.CheckAccessAsync(ctx2).Result);

            var ctx3 = new ResourceAuthorizationContext(Anonymous,
                UhomeResources.Actions.List, UhomeResources.Case);
            Assert.IsFalse(subject.CheckAccessAsync(ctx3).Result);
        }

        [TestMethod]
        public void AuthenticatedCanListCase()
        {
            var user = User("John");
            var ctx = new ResourceAuthorizationContext(user,
                UhomeResources.Actions.List, UhomeResources.Case);
            Assert.IsTrue(subject.CheckAccessAsync(ctx).Result);

        }

        [TestMethod]
        public void AuthenticatedCanViewOwnedCase()
        {
            var user = User("John");
            var applicationUser = new ApplicationUser();
            applicationUser.UserName = user.Identity.Name;
            db.Users.Add(applicationUser);
            var @case = CreateCase();
            @case.CreatedBy = applicationUser;
            db.Cases.Add(@case);
            db.SaveChanges();

            var ctx = new ResourceAuthorizationContext(user,
                UhomeResources.Actions.View, UhomeResources.Case, @case.ID.ToString());
            Assert.IsTrue(subject.CheckAccessAsync(ctx).Result);
        }

        [TestMethod]
        public void AuthenticatedCannotViewothersCase()
        {
            var user = User("John");
            var applicationUser1 = new ApplicationUser();
            applicationUser1.UserName = user.Identity.Name;
            db.Users.Add(applicationUser1);
            var applicationUser2 = db.Users.Create();
            applicationUser2.UserName = "Other";
            db.Users.Add(applicationUser2);
            var @case = CreateCase();
            @case.CreatedBy = applicationUser2;
            db.Cases.Add(@case);
            db.SaveChanges();

            var ctx = new ResourceAuthorizationContext(user,
                UhomeResources.Actions.View, UhomeResources.Case, @case.ID.ToString());
            Assert.IsFalse(subject.CheckAccessAsync(ctx).Result);
        }

        [TestMethod]
        public void AuthenticatedCanEditOwnedCase()
        {
            var user = User("John");
            var applicationUser = new ApplicationUser();
            applicationUser.UserName = user.Identity.Name;
            db.Users.Add(applicationUser);
            var @case = CreateCase();
            db.Cases.Add(@case);
            @case.CreatedBy = applicationUser;
            db.SaveChanges();

            var ctx = new ResourceAuthorizationContext(user,
                UhomeResources.Actions.Edit, UhomeResources.Case, @case.ID.ToString());
            Assert.IsTrue(subject.CheckAccessAsync(ctx).Result);
        }

        [TestMethod]
        public void AuthenticatedCannotEditothersCase()
        {
            var user = User("John");
            var applicationUser1 = new ApplicationUser();
            applicationUser1.UserName = user.Identity.Name;
            db.Users.Add(applicationUser1);
            var applicationUser2 = db.Users.Create();
            applicationUser2.UserName = "Other";
            db.Users.Add(applicationUser2);
            var @case = CreateCase();
            @case.CreatedBy = applicationUser2;
            db.Cases.Add(@case);
            db.SaveChanges();

            var ctx = new ResourceAuthorizationContext(user,
                UhomeResources.Actions.Edit, UhomeResources.Case, @case.ID.ToString());
            Assert.IsFalse(subject.CheckAccessAsync(ctx).Result);
        }

        [TestMethod]
        public void AuthenticatedAdminCanViewOthersCase()
        {
            var admin = User("AdminUser", new string[] { "Admin" });
            var applicationUser1 = new ApplicationUser();
            applicationUser1.UserName = admin.Identity.Name;
            db.Users.Add(applicationUser1);
            var applicationUser2 = db.Users.Create();
            applicationUser2.UserName = "Other";
            db.Users.Add(applicationUser2);
            var @case = CreateCase();
            @case.CreatedBy = applicationUser2;
            db.Cases.Add(@case);
            db.SaveChanges();

            var ctx = new ResourceAuthorizationContext(admin,
                UhomeResources.Actions.View, UhomeResources.Case, @case.ID.ToString());
            Assert.IsTrue(subject.CheckAccessAsync(ctx).Result);
        }

        [TestMethod]
        public void AuthenticatedAdminCanEditOthersCase()
        {
            var admin = User("AdminUser", new string[] { "Admin" });
            var applicationUser1 = new ApplicationUser();
            applicationUser1.UserName = admin.Identity.Name;
            db.Users.Add(applicationUser1);
            var applicationUser2 = db.Users.Create();
            applicationUser2.UserName = "Other";
            db.Users.Add(applicationUser2);
            var @case = CreateCase();
            @case.CreatedBy = applicationUser2;
            db.Cases.Add(@case);
            db.SaveChanges();

            var ctx = new ResourceAuthorizationContext(admin,
                UhomeResources.Actions.Edit, UhomeResources.Case, @case.ID.ToString());
            Assert.IsTrue(subject.CheckAccessAsync(ctx).Result);
        }

        [TestMethod]
        public void AuthenticatedManagerCanViewOthersCase()
        {
            var manager = User("ManagerUser", new string[] { "Manager" });
            var applicationUser1 = new ApplicationUser();
            applicationUser1.UserName = manager.Identity.Name;
            db.Users.Add(applicationUser1);
            var applicationUser2 = db.Users.Create();
            applicationUser2.UserName = "Other";
            db.Users.Add(applicationUser2);
            var @case = CreateCase();
            @case.CreatedBy = applicationUser2;
            db.Cases.Add(@case);
            db.SaveChanges();

            var ctx = new ResourceAuthorizationContext(manager,
                UhomeResources.Actions.View, UhomeResources.Case, @case.ID.ToString());
            Assert.IsTrue(subject.CheckAccessAsync(ctx).Result);
        }

        [TestMethod]
        public void AuthenticatedManagerCanEditOthersCase()
        {
            var manager = User("ManagerUser", new string[] { "Manager" });
            var applicationUser1 = new ApplicationUser();
            applicationUser1.UserName = manager.Identity.Name;
            db.Users.Add(applicationUser1);
            var applicationUser2 = db.Users.Create();
            applicationUser2.UserName = "Other";
            db.Users.Add(applicationUser2);
            var @case = CreateCase();
            @case.CreatedBy = applicationUser2;
            db.Cases.Add(@case);
            db.SaveChanges();

            var ctx = new ResourceAuthorizationContext(manager,
                UhomeResources.Actions.Edit, UhomeResources.Case, @case.ID.ToString());
            Assert.IsTrue(subject.CheckAccessAsync(ctx).Result);
        }
    }
}