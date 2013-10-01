using AspnetIdentitySample.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace AspnetIdentitySample
{
    public static class IdentityConfig{
    public static async void Initialize(){
        var context = new MyDbContext();
        AuthenticationIdentityManager IdentityManager = new AuthenticationIdentityManager(new IdentityStore(context));
        string name = "Admin";
        string password = "123456";
        string test = "test";
        //Create Role Test and User Test
        await IdentityManager.Roles.CreateRoleAsync(new Role(test), CancellationToken.None);
        await IdentityManager.Users.CreateLocalUserAsync(new MyUser(){ UserName=test}, password, CancellationToken.None);
    
        //Create Role Admin if it does not exist
        var role = await IdentityManager.Roles.FindRoleByNameAsync(name, CancellationToken.None);
        if (role == null)
        {
            var roleresult = await IdentityManager.Roles.CreateRoleAsync(new Role(name), CancellationToken.None);
            if (roleresult.Success)
            {
                role = await IdentityManager.Roles.FindRoleByNameAsync(name, CancellationToken.None);

            }
        }

        //Create User=Admin with password=123456
        var user = new MyUser();
        user.UserName = name;
        //var adminresult = await IdentityManager.Users.CreateLocalUserAsync(user, password, CancellationToken.None);
        var adminresult = await IdentityManager.Users.CreateLocalUserAsync(user, password, CancellationToken.None);

        //Add User Admin to Role Admin
        if (adminresult.Success)
        {
            var result = await IdentityManager.Roles.AddUserToRoleAsync(user.Id, role.Id, CancellationToken.None);
        }
        else
        {
            // Add user admin to Role Admin if not already added
            //Find the user Admin
            var userId = await IdentityManager.Logins.GetUserIdForLocalLoginAsync(name, CancellationToken.None);

            var rolesForUser = await IdentityManager.Roles.GetRolesForUserAsync(userId, CancellationToken.None);
            if (!rolesForUser.Contains(role))
            {
                var result = await IdentityManager.Roles.AddUserToRoleAsync(userId, role.Id, CancellationToken.None);
            }
        }
    }
    }
    public class MyDbInitializer : DropCreateDatabaseAlways<MyDbContext>
    {
        protected override void Seed(MyDbContext context)
        {
            base.Seed(context);
        }
    }
}