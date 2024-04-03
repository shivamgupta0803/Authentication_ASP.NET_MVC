using Microsoft.AspNetCore.Mvc;

namespace RegistrationAndLogin.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Welcome", "Account");
            }
            else
            {
                return RedirectToAction("Register", "Account");
            }
        }
    }
}
