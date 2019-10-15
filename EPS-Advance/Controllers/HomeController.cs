using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPS_Advance_Classes_Library;
using EPS_Advance_Classes_Library.ProductMgmt;
using EPS_Advance_Classes_Library.UserMgmt;


namespace EPS_Advance.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            User u = (User)Session[WebUtil.CURRENT_USER];
            if (u == null)
            {
                //return RedirectToAction("Login", "User", new { ctl = "Home", act = "Index" });
                ViewBag.recomProds = new ProductHandler().GetAllProducts();
            }
            else
            {
                ViewBag.recomProds = new ProductHandler().AllProductsByCity(u);
            }

            ViewBag.categories = new ProductHandler().GetCategories();

            return View();
        }

        public ActionResult ProductsByCategory(int id)
        {
            List<Product> productsByCat = new ProductHandler().GetAllProductsByCategory(id);
            ViewBag.categories = new ProductHandler().GetCategories();
            return View(productsByCat);
        }

        public ActionResult About()
        {
            ViewBag.Message = "EPS is Exchange Product System. First Ever In Pakistan";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "For Any Queries, Dont Hesitate to Contact Us";

            return View();
        }

        [Route("item/{_item}")]
        public ActionResult ItemPage(String _item)
        {
            return View();
        }

        [Route("post")]
        public ActionResult Post()
        {
            return View();
        }

        public ActionResult Messages()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(string search)
        {
            List<Product> products = new ProductHandler().SearchProduct(search);
            return View(products);
        }
    }
}