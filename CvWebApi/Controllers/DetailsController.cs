using Microsoft.AspNetCore.Mvc;

namespace CvWebApi.Controllers
{
    public class DetailsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
