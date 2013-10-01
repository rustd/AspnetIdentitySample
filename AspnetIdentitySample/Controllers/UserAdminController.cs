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

namespace AspnetIdentitySample.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersAdminController : Controller
    {
        public UsersAdminController()
        {
            IdentityManager = new AuthenticationIdentityManager(new IdentityStore(new MyDbContext()));
            context = new MyDbContext();

        }

        public UsersAdminController(AuthenticationIdentityManager manager)
        {
            IdentityManager = manager;
        }

        public AuthenticationIdentityManager IdentityManager { get; private set; }
        public MyDbContext context { get; private set; }

        //
        // GET: /Users/
        public async Task<ActionResult> Index()
        {
            return View(await context.Users.ToListAsync());
        }

        //
        // GET: /Users/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await context.Users.FindAsync(id);

            return View(user);
        }

        //
        // GET: /Users/Create
        public async Task<ActionResult> Create()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await context.Roles.ToListAsync(), "Id", "Name");
            return View();
        }

        //
        // POST: /Users/Create
        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, string RoleId)
        {
            if (ModelState.IsValid)
            {
                var user = new MyUser();
                user.UserName = userViewModel.UserName;
                user.HomeTown = userViewModel.HomeTown;
                var adminresult = await IdentityManager.Users.CreateLocalUserAsync(user, userViewModel.Password, CancellationToken.None);

                //Add User Admin to Role Admin
                if (adminresult.Success)
                {
                    if (!String.IsNullOrEmpty(RoleId))
                    {
                        var result = await IdentityManager.Roles.AddUserToRoleAsync(user.Id, RoleId, CancellationToken.None);
                        if (!result.Success)
                        {
                            ModelState.AddModelError("", result.Errors.First().ToString());
                            ViewBag.RoleId = new SelectList(await context.Roles.ToListAsync(), "Id", "Name");
                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First().ToString());
                    ViewBag.RoleId = new SelectList(await context.Roles.ToListAsync(), "Id", "Name");
                    return View();

                }
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.RoleId = new SelectList(await context.Roles.ToListAsync(), "Id", "Name");
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
            ViewBag.RoleId = new SelectList(await context.Roles.ToListAsync(), "Id", "Name");
            var user = await context.Users.FindAsync(id);
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
        public async Task<ActionResult> Edit([Bind(Include = "UserName,Id,HomeTown")] MyUser formuser, string id, string RoleId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.RoleId = new SelectList(await context.Roles.ToListAsync(), "Id", "Name");
            var user = await context.Users.FindAsync(id);
            user.UserName = formuser.UserName;
            user.HomeTown = formuser.HomeTown;
            if (ModelState.IsValid)
            {
                //Update the user details
                context.Entry(user).State = EntityState.Modified;
                await context.SaveChangesAsync();

                //If user has existing Role then remove the user from the role
                // This also accounts for the case when the Admin selected Empty from the drop-down and
                // this means that all roles for the user must be removed
                var rolesForUser = await IdentityManager.Roles.GetRolesForUserAsync(id, CancellationToken.None);
                if (rolesForUser.Count() > 0)
                {
                    
                    foreach (var item in rolesForUser)
                    {
                        var result = await IdentityManager.Roles.RemoveUserFromRoleAsync(user.Id, item.Id, CancellationToken.None);
                    }
                }

                if (!String.IsNullOrEmpty(RoleId))
                {
                    //Add user to new role
                    var result = await IdentityManager.Roles.AddUserToRoleAsync(user.Id, RoleId, CancellationToken.None);
                    if (!result.Success)
                    {
                        ModelState.AddModelError("", result.Errors.First().ToString());
                        ViewBag.RoleId = new SelectList(await context.Roles.ToListAsync(), "Id", "Name");
                        return View();
                    }
                }

                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.RoleId = new SelectList(await context.Roles.ToListAsync(), "Id", "Name");
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
