using EPS_Advance.Models;
using EPS_Advance_Classes_Library;
using EPS_Advance_Classes_Library.ProductMgmt;
using EPS_Advance_Classes_Library.UserMgmt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPS_Advance.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddProduct()
        {
            User u = (User)Session[WebUtil.CURRENT_USER];
            if (u == null)
            {
                return RedirectToAction("Login", "User", new { ctl = "Product", act = "AddProduct" });
            }
            ViewBag.categories = ModelHelper.ToSelectItemList(new ProductHandler().GetCategories());
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProduct(FormCollection fData)
        {
            User u = (User)Session[WebUtil.CURRENT_USER];
            if (u == null)
            {
                return RedirectToAction("Login", "User", new { ctl = "Product", act = "AddProduct" });
            }
            Product product = new Product();
            product.Name = fData["itemname"];
            product.MinPrice = Convert.ToInt32(fData["minpr"]);
            product.MaxPrice = Convert.ToInt32(fData["maxpr"]);
            product.Condition = Convert.ToInt32(fData["condition"]);
            product.Available = true;
            product.Category = new Category { Id = Convert.ToInt32(fData["Category"]) };
            product.Description = fData["description"];
            product.DateAdded = DateTime.Now.ToShortDateString();
            product.User = (User)Session[WebUtil.CURRENT_USER];

            long numb = DateTime.Now.Ticks;
            int count = 0;

            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i];
                if (file != null && file.ContentLength > 0)
                {
                    string name = file.FileName;
                    string url = "/ImagesData/ProductImages/" + numb + "_" + ++count + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    string path = Request.MapPath(url);
                    file.SaveAs(path);
                    product.Images.Add(new ProductImages { Caption = name, Image_Url = url });
                }
                else
                {
                    string name = "No Image";
                    string url = "https://dummyimage.com/800x600/d6d6d6/000000.jpg&text=Sorry+No+Image+Avaialable";
                    product.Images.Add(new ProductImages { Caption = name, Image_Url = url });
                }
            }
            new ProductHandler().AddProduct(product);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult ProductDetail(int id)
        {
            Product product = new ProductHandler().GetProduct(id);
            return View(product);
        }
    }
}