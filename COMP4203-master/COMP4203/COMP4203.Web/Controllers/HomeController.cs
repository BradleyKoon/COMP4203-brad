using System.Web.Mvc;

namespace COMP4203.Web.Controllers
{
    public class HomeController : Controller
    {
	    public ActionResult Index()
	    {
		    return View("Index");
	    }
    }
}