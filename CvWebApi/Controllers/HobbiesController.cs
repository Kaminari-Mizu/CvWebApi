using Microsoft.AspNetCore.Mvc;

namespace CvWebApi.Controllers
{
    public class HobbiesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
