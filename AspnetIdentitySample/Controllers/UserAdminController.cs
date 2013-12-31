using AspnetIdentitySample.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace AspnetIdentitySample.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersAdminController : Controller
    {
        public UsersAdminController()
        {
            context = new MyDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
        }

        public UsersAdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }
        public RoleManager<IdentityRole> RoleManager { get; private set; }
        public MyDbContext context { get; private set; }

        //
        // GET: /Users/
        public async Task<ActionResult> Index()
        {
            return View(await UserManager.Users.ToListAsync());
        }

        //
        // GET: /Users/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            return View(user);
        }

        //
        // GET: /Users/Create
        public async Task<ActionResult> Create()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Id", "Name");
            return View();
        }

        //
        // POST: /Users/Create
        [HttpPost]
        public async Task<ActionResult>  Create(RegisterViewModel userViewModel, string RoleId)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser();
                user.UserName = userViewModel.UserName;
                user.HomeTown = userViewModel.HomeTown;
                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                //Add User Admin to Role Admin
                if (adminresult.Succeeded)
                {
                    if (!String.IsNullOrEmpty(RoleId))
                    {
                        //Find Role Admin
                        var role = await RoleManager.FindByIdAsync(RoleId);
                        var result = await UserManager.AddToRoleAsync(user.Id, role.Name);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First().ToString());
                            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Id", "Name");
                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First().ToString());
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Id", "Name");
                    return View();

                }
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.RoleId = new SelectList(RoleManager.Roles, "Id", "Name");
                return View();
            }
        }

        //
        // GET: /Users/Edit/1
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Id", "Name");

            var user = await UserManager.FindByIdAsync(id); 
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "UserName,Id,HomeTown")] ApplicationUser formuser, string id, string RoleId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Id", "Name");
            var user = await UserManager.FindByIdAsync(id);
            user.UserName = formuser.UserName;
            user.HomeTown = formuser.HomeTown;
            if (ModelState.IsValid)
            {
                //Update the user details
                await UserManager.UpdateAsync(user);
                
                //If user has existing Role then remove the user from the role
                // This also accounts for the case when the Admin selected Empty from the drop-down and
                // this means that all roles for the user must be removed
                var rolesForUser = await UserManager.GetRolesAsync(id);
                if (rolesForUser.Count() > 0)
                {   
                    foreach (var item in rolesForUser)
                    {
                        var result = await UserManager.RemoveFromRoleAsync(id,item);
                    }
                }

                if (!String.IsNullOrEmpty(RoleId))
                {
                    //Find Role
                    var role = await RoleManager.FindByIdAsync(RoleId);
                    //Add user to new role
                    var result = await UserManager.AddToRoleAsync(id,role.Name);
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", result.Errors.First().ToString());
                        ViewBag.RoleId = new SelectList(RoleManager.Roles, "Id", "Name");
                        return View();
                    }
                }
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.RoleId = new SelectList(RoleManager.Roles, "Id", "Name");
                return View();
            }
        }

        ////
        //// GET: /Users/Delete/5
        //public async Task<ActionResult> Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    var user = await context.Users.FindAsync(id);
        //    if (user == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(user);
        //}

        ////
        //// POST: /Users/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(string id)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }

        //        var user = await context.Users.FindAsync(id);
        //        var logins = user.Logins;
        //        foreach (var login in logins)
        //        {
        //            context.UserLogins.Remove(login);
        //        }
        //        var rolesForUser = await IdentityManager.Roles.GetRolesForUserAsync(id, CancellationToken.None);
        //        if (rolesForUser.Count() > 0)
        //        {

        //            foreach (var item in rolesForUser)
        //            {
        //                var result = await IdentityManager.Roles.RemoveUserFromRoleAsync(user.Id, item.Id, CancellationToken.None);
        //            }
        //        }
        //        context.Users.Remove(user);
        //        await context.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        return View();
        //    }
        //}
    }
}
