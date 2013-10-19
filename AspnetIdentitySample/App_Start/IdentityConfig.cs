using AspnetIdentitySample.Models;
using Microsoft.AspNet.Identity;
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
        var UserManager = new UserManager<MyUser>(new UserStore<MyUser>(context));
        var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
        
        string name = "Admin";
        string password = "123456";
        string test = "test";
        
        //Create Role Test and User Test
        await RoleManager.CreateAsync(new IdentityRole(test));
        await UserManager.CreateAsync(new MyUser(){ UserName=test});
    
        //Create Role Admin if it does not exist
        if (!await RoleManager.RoleExistsAsync(name))
        {
            var roleresult = await RoleManager.CreateAsync(new IdentityRole(name));
        }

        //Create User=Admin with password=123456
        var user = new MyUser();
        user.UserName = name;
        user.HomeTown = "Seattle";
        var adminresult = await UserManager.CreateAsync(user, password);

        //Add User Admin to Role Admin
        if (adminresult.Succeeded)
        {
            var result = await UserManager.AddToRoleAsync(user.Id, name);
        }
        else
        {
        //    // Add user admin to Role Admin if not already added
        //    //Find the user Admin
        //    var userId = await IdentityManager.Logins.GetUserIdForLocalLoginAsync(name, CancellationToken.None);

        //    var rolesForUser = await IdentityManager.Roles.GetRolesForUserAsync(userId, CancellationToken.None);
        //    if (!rolesForUser.Contains(role))
        //    {
        //        var result = await IdentityManager.Roles.AddUserToRoleAsync(userId, role.Id, CancellationToken.None);
        //    }
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