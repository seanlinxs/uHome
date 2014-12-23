using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using uHome.Models;

namespace uHome.Services
{
    public class UserService
    {
        public static ISet<ApplicationUser> FindUsersByRoleName(string RoleName)
        {
            using (var Database = new ApplicationDbContext())
            {
                var role = Database.Roles.Where(r => r.Name == RoleName).Single();
                var users = new HashSet<ApplicationUser>();
                var user_ids = role.Users.Select(u => u.UserId);

                foreach (var user_id in user_ids)
                {
                    users.Add(Database.Users.Find(user_id));
                }

                return users;
            }
        }
    }
}