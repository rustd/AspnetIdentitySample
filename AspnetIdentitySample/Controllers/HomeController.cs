using System.Web.Mvc;
using AspnetIdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AspnetIdentitySample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        // Only Authenticated users can access thier profile
        [Authorize]
        public new ActionResult Profile()
        {
            // Instantiate the ASP.NET Identity system
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new MyDbContext()));

            // Get the current logged in User and look up the user in ASP.NET Identity
            var currentUser = manager.FindById(User.Identity.GetUserId());

            // Recover the profile information about the logged in user
            ViewBag.HomeTown = currentUser.HomeTown;
            ViewBag.FirstName = currentUser.MyUserInfo.FirstName;

            return View();
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}