using Microsoft.AspNetCore.Mvc;

namespace CvWebApi.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
