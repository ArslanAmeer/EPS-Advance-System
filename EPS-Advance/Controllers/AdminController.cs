using EPS_Advance.Models;
using EPS_Advance_Classes_Library;
using EPS_Advance_Classes_Library.LocationMgmt;
using EPS_Advance_Classes_Library.ProductMgmt;
using EPS_Advance_Classes_Library.UserMgmt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace EPS_Advance.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult AdminPanel()
        {
            User u = (User)Session[WebUtil.CURRENT_USER];
            if (!(u != null && u.IsInRole(WebUtil.ADMIN_ROLE)))
            {
                return RedirectToAction("Login", "User", new { ctl = "Admin", act = "AdminPanel" });
            }
            return View();
        }

        public ActionResult ManageAds()
        {
            User u = (User)Session[WebUtil.CURRENT_USER];
            if (!(u != null && u.IsInRole(WebUtil.ADMIN_ROLE)))
            {
                return RedirectToAction("Login", "User", new { ctl = "Admin", act = "ManageAds" });
            }
            List<Product> allProducts = new ProductHandler().GetAllProducts();
            return View(allProducts);
        }

        public ActionResult AdDetails(int? Id)
        {
            User u = (User)Session[WebUtil.CURRENT_USER];
            if (!(u != null && u.IsInRole(WebUtil.ADMIN_ROLE)))
            {
                return RedirectToAction("Login", "User", new { ctl = "Admin", act = "AdDetails" });
            }
            Product product = new ProductHandler().GetProduct(Id);
            return View(product);
        }

        public ActionResult AdEdit(int? Id)
        {
            User u = (User)Session[WebUtil.CURRENT_USER];
            if (!(u != null && u.IsInRole(WebUtil.ADMIN_ROLE)))
            {
                return RedirectToAction("Login", "User", new { ctl = "Admin", act = "AdEdit" });
            }
            Product product = new ProductHandler().GetProduct(Id);
            return View(product);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AdEdit(Product product)
        {
            new ProductHandler().UpdateProduct(product);
            return RedirectToAction("AdDetails", new { Id = product.Id });
        }

        public ActionResult AdDelete(int? id)
        {
            User u = (User)Session[WebUtil.CURRENT_USER];
            if (!(u != null && u.IsInRole(WebUtil.ADMIN_ROLE)))
            {
                return RedirectToAction("Login", "User", new { ctl = "Admin", act = "AdDelete" });
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = new ProductHandler().GetProduct(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("AdDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult AdDeleteConfirmed(int id)
        {
            User u = (User)Session[WebUtil.CURRENT_USER];
            if (!(u != null && u.IsInRole(WebUtil.ADMIN_ROLE)))
            {
                return RedirectToAction("Login", "User", new { ctl = "Admin", act = "AdDelete" });
            }

            try
            {
                new ProductHandler().DeleteProduct(id);
                return RedirectToAction("ManageAds");
            }
            catch
            {
                ViewBag.message = "Unable To Delete This Product.";
                TempData["message"] = ViewBag.message;
                return RedirectToAction("ManageAds");
            }
        }

        public ActionResult ManageUsers()
        {
            User u = (User)Session[WebUtil.CURRENT_USER];
            if (!(u != null && u.IsInRole(WebUtil.ADMIN_ROLE)))
            {
                return RedirectToAction("Login", "User", new { ctl = "Admin", act = "AdEdit" });
            }
            List<User> allUsers = new UserHandler().GetUsers();
            return View(allUsers);
        }

        public ActionResult UserDetail(int? Id)
        {
            return View(new UserHandler().GetUser(Id));
        }

        public ActionResult UserEdit(int? id)
        {
            User u = (User)Session[WebUtil.CURRENT_USER];
            if (!(u != null && u.IsInRole(WebUtil.ADMIN_ROLE)))
            {
                return RedirectToAction("Login", "User", new { ctl = "Admin", act = "UserEdit" });
            }
            ViewBag.CountryList = ModelHelper.ToSelectItemList(new LocationHandler().GetCountries());
            User oldUser = new UserHandler().GetUser(id);
            return View(oldUser);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UserEdit(User user)
        {
            new UserHandler().UpdateUserByAdmin(user);
            return RedirectToAction("UserDetail", new { Id = user.Id });
        }

        public ActionResult UserDelete(int? id)
        {
            User u = (User)Session[WebUtil.CURRENT_USER];
            if (!(u != null && u.IsInRole(WebUtil.ADMIN_ROLE)))
            {
                return RedirectToAction("Login", "User", new { ctl = "Admin", act = "UserDelete" });
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = new UserHandler().GetUser(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("UserDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult UserDeleteConfirmed(int id)
        {
            User u = (User)Session[WebUtil.CURRENT_USER];
            if (!(u != null && u.IsInRole(WebUtil.ADMIN_ROLE)))
            {
                return RedirectToAction("Login", "User", new { ctl = "Admin", act = "UserDelete" });
            }
            new UserHandler().DeleteUser(id);
            return RedirectToAction("ManageUsers");
            //try
            //{
            //    new UserHandler().DeleteUser(id);
            //    return RedirectToAction("ManageUsers");
            //}
            //catch
            //{
            //    ViewBag.message = "Unable To Delete This User.";
            //    TempData["message"] = ViewBag.message;
            //    return RedirectToAction("ManageUsers");
            //}
        }
    }
}