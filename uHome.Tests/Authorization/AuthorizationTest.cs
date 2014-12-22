using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using uHome.Authorization;
using System.Security.Claims;
using Thinktecture.IdentityModel.Owin.ResourceAuthorization;
using uHome.Models;
using System.Transactions;

namespace uHome.Tests.Authorization
{
    [TestClass]
    public class AuthorizationTest
    {
        public static readonly ClaimsPrincipal Anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public IResourceAuthorizationManager subject;

        public ClaimsPrincipal User(string username, params string[] roles)
        {
            ClaimsIdentity ci = new ClaimsIdentity("Password");
            ci.AddClaim(new Claim(ClaimTypes.Name, username));

            foreach (string role in roles)
            {
                ci.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            return new ClaimsPrincipal(ci);
        }

        public ApplicationDbContext db = new ApplicationDbContext();
        public TransactionScope scope;

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

    }
}
