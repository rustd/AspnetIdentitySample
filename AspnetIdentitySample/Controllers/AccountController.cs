using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Validation;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using AspnetIdentitySample.Models;

namespace AspnetIdentitySample.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController()
        {
            IdentityStore = new IdentityStoreManager(new CustomUserContext(new MyDbContext()));
            AuthenticationManager = new IdentityAuthenticationManager(IdentityStore);
        }

        public AccountController(IdentityStoreManager storeManager, IdentityAuthenticationManager authManager)
        {
            IdentityStore = storeManager;
            AuthenticationManager = authManager;
        }

        public IdentityStoreManager IdentityStore { get; private set; }
        public IdentityAuthenticationManager AuthenticationManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // Validate the user password
                if (await AuthenticationManager.CheckPasswordAndSignIn(HttpContext, model.UserName, model.Password, model.RememberMe))
                {
                    return RedirectToLocal(returnUrl);
                }
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Create a profile, password, and link the local login before signing in the user
                    //var user = new MyUser() { UserName = model.UserName,BirthDate=model.BirthDate };
                    var user = new MyUser() { UserName = model.UserName };
                    if (await IdentityStore.CreateLocalUser(user, model.Password))
                    {
                        await AuthenticationManager.SignIn(HttpContext, user.Id, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to register user name: " + model.UserName);
                    }
                }
                catch (IdentityException e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            string userId = User.Identity.GetUserId();
            if (await IdentityStore.RemoveLogin(User.Identity.GetUserId(), loginProvider, providerKey))
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public async Task<ActionResult> Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = await IdentityStore.HasLocalLogin(User.Identity.GetUserId());
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            string userId = User.Identity.GetUserId();
            bool hasLocalLogin = await IdentityStore.HasLocalLogin(userId);
            ViewBag.HasLocalPassword = hasLocalLogin;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalLogin)
            {
                if (ModelState.IsValid)
                {
                    bool changePasswordSucceeded = await IdentityStore.ChangePassword(User.Identity.GetUserName(), model.OldPassword, model.NewPassword);
                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        // Create the local login info and link the local account to the user
                        if (await IdentityStore.CreateLocalLogin(userId, User.Identity.GetUserName(), model.NewPassword))
                        {
                            return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                        }
                        else
                        {
                            ModelState.AddModelError("", "Failed to set password");
                        }
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { loginProvider = provider, ReturnUrl = returnUrl }), AuthenticationManager);
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string loginProvider, string returnUrl)
        {
            ClaimsIdentity id = await AuthenticationManager.GetExternalIdentity(HttpContext);
            if (!AuthenticationManager.VerifyExternalIdentity(id, loginProvider))
            {
                return View("ExternalLoginFailure");
            }

            // Sign in this external identity if its already linked
            if (await AuthenticationManager.SignInExternalIdentity(HttpContext, id, loginProvider))
            {
                return RedirectToLocal(returnUrl);
            }
            else if (User.Identity.IsAuthenticated)
            {
                // Try to link if the user is already signed in
                if (await AuthenticationManager.LinkExternalIdentity(id, User.Identity.GetUserId(), loginProvider))
                {
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    return View("ExternalLoginFailure");
                }
            }
            else
            {
                // Otherwise prompt to create a local user
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = id.Name, LoginProvider = loginProvider });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                try
                {
                    if (await AuthenticationManager.CreateAndSignInExternalUser(HttpContext, model.LoginProvider, new MyUser(model.UserName)))
                    {
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        return View("ExternalLoginFailure");
                    }
                }
                catch (IdentityException e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(HttpContext);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return (ActionResult)PartialView("_ExternalLoginsListPartial", new List<AuthenticationDescription>(AuthenticationManager.GetExternalAuthenticationTypes(HttpContext)));
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            return Task.Run(async () =>
            {
                var linkedAccounts = await IdentityStore.GetLogins(User.Identity.GetUserId());
                ViewBag.ShowRemoveButton = linkedAccounts.Count > 1;
                return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
            }).Result;
        }

        //protected override void Dispose(bool disposing) {
        //    if (disposing && IdentityStore != null) {
        //        IdentityStore.Dispose();
        //        IdentityStore = null;
        //    }
        //    base.Dispose(disposing);
        //}

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUrl, IdentityAuthenticationManager manager)
            {
                LoginProvider = provider;
                RedirectUrl = redirectUrl;
                Manager = manager;
            }

            public string LoginProvider { get; set; }
            public string RedirectUrl { get; set; }
            public IdentityAuthenticationManager Manager { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                Manager.Challenge(context.HttpContext, LoginProvider, RedirectUrl);
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        #endregion
    }
}
