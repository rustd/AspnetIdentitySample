using AspnetIdentitySample.IdentityExtensions;
using AspnetIdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;

namespace AspnetIdentitySample.Controllers
{
    public class ValidationController : Controller
    {
        // GET: Validation
        public ActionResult Index()
        {
            // List of things you can do with Validation
            // Tweak the default settings
            return View();
        }

        // POST: Validation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Default(RegisterViewModel model)
        {
            var context = new MyDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            
            // The default Validators that the UserManager uses are UserValidator and MinimumLengthValidator
            // You can tweak some of the settings as follows
            // This example sets the Password length to be 3 characters
            UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager)
            {
                AllowOnlyAlphanumericUserNames = false
            };
             UserManager.PasswordValidator = new MinimumLengthValidator(3);


            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.UserName };
                user.HomeTown = model.HomeTown;
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var authManager = HttpContext.GetOwinContext().Authentication;
                    var claimsIdentity = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                    authManager.SignIn(claimsIdentity);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Customize(RegisterViewModel model)
        {
            var context = new MyDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // The default Validators that the UserManager uses are UserValidator and MinimumLengthValidator
            // If you want to have complete control over validation then you can write your own validators.
            UserManager.UserValidator = new MyUserValidation();
            UserManager.PasswordValidator = new MyPasswordValidation();
            UserManager.PasswordHasher = new PasswordHasher();


            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.UserName };
                user.HomeTown = model.HomeTown;
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var authManager = HttpContext.GetOwinContext().Authentication;
                    var claimsIdentity = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                    authManager.SignIn(claimsIdentity);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View("Index",model);
        }
    }
}
