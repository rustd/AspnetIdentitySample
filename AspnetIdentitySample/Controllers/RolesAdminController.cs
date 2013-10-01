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
    [Authorize(Roles="Admin")]
    public class RolesAdminController : Controller
    {
        public RolesAdminController()
        {
            IdentityManager = new AuthenticationIdentityManager(new IdentityStore(new MyDbContext()));
            context = new MyDbContext();

        }

        public RolesAdminController(AuthenticationIdentityManager manager)
        {
            IdentityManager = manager;
        }

        public AuthenticationIdentityManager IdentityManager { get; private set; }
        public MyDbContext context { get; private set; }
        //
        // GET: /Roles/
        public async Task<ActionResult> Index()
        {
            return View(await context.Roles.ToListAsync());
        }

        //
        // GET: /Roles/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Role role = await IdentityManager.Roles.FindRoleAsync(id, CancellationToken.None) as Role;
            return View(role);
        }

        //
        // GET: /Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Roles/Create
        [HttpPost]
        public async Task<ActionResult> Create(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var role = new Role(roleViewModel.Name);
                // TODO: Add insert logic here
                var roleresult = await IdentityManager.Roles.CreateRoleAsync(role, CancellationToken.None);
                if(!roleresult.Success)
                {
                    ModelState.AddModelError("",roleresult.Errors.First().ToString());
                    return View();
                }
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        //
        // GET: /Roles/Edit/Admin
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Role role = await IdentityManager.Roles.FindRoleAsync(id,CancellationToken.None) as Role;
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        //
        // POST: /Roles/Edit/5
        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Name,Id")] Role role)
        {
            if (ModelState.IsValid)
            {
                context.Entry(role).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        //
        // GET: /Roles/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Role role = await IdentityManager.Roles.FindRoleAsync(id, CancellationToken.None) as Role;
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        //
        // POST: /Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var  result = await IdentityManager.Roles.DeleteRoleAsync(id, false, CancellationToken.None);
                if (!result.Success)
                {
                    ModelState.AddModelError("",result.Errors.First().ToString());
                    return View();
                }
                return RedirectToAction("Index");
            } 
            else
            {
                return View();
            }
        }
    }
}
