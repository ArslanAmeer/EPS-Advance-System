using EPS_Advance.Models;
using EPS_Advance_Classes_Library;
using EPS_Advance_Classes_Library.LocationMgmt;
using EPS_Advance_Classes_Library.ProductMgmt;
using EPS_Advance_Classes_Library.UserMgmt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EPS_Advance.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Login()
        {
            ViewBag.msg = TempData["msg"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel data)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.msg = "Invalid Entry!";
                return View();
            }

            User u = new UserHandler().GetUser(data.LoginId, data.Password);
            if (u != null)
            {
                if (data.RememberMe)
                {
                    HttpCookie c = new HttpCookie("idpas")
                    {
                        Expires = DateTime.Today.AddDays(7),
                    };
                    c.Values.Add("lid", u.LoginID);
                    c.Values.Add("psd", u.Password);
                    Response.SetCookie(c);
                }


                Session.Add(WebUtil.CURRENT_USER, u);

                string ctl = Request.QueryString["c"];
                string act = Request.QueryString["a"];

                if (!string.IsNullOrEmpty(ctl) && !string.IsNullOrEmpty(act))
                {
                    return RedirectToAction(act, ctl);
                }
                else if (u.IsInRole(WebUtil.ADMIN_ROLE))
                {
                    return RedirectToAction("AdminPanel", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.msg = "Invalid Username or Password! Please Try Again ";
                return View();
            }

        }

        [HttpGet]
        public ActionResult Register()
        {
            ViewBag.CountryList = ModelHelper.ToSelectItemList(new LocationHandler().GetCountries());
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user, FormCollection f)
        {
            User userExist = new UserHandler().GetUserByEmail(user.Email);
            if (userExist != null)
            {
                ViewBag.CountryList = ModelHelper.ToSelectItemList(new LocationHandler().GetCountries());
                ViewBag.msg = $"User with this '{user.Email}' already Exist!";
                return View();
            }
            ModelState.Remove("CityId");
            if (!ModelState.IsValid)
            {
                ViewBag.CountryList = ModelHelper.ToSelectItemList(new LocationHandler().GetCountries());
                return View();
            }

            if (user.Email.Equals("admin@email.com", StringComparison.InvariantCultureIgnoreCase) && user.Password.Equals("admin123", StringComparison.InvariantCultureIgnoreCase))
            {
                user.Role = new UserHandler().GetRoles(1);
            }
            else
            {
                user.Role = new UserHandler().GetRoles(2);
            }
            user.SecurityQuestion = Convert.ToString(f["sqQues"]);
            user.CityId = new LocationHandler().getCityById(Convert.ToInt32(f["CityId"]));
            user.IsActive = true;
            user.LoginID = user.Email;
            user.BirthDate = string.IsNullOrEmpty(f["BirthDate"]) ? null : (Convert.ToDateTime(f["BirthDate"])).ToShortDateString();
            new UserHandler().Adduser(user);
            TempData["msg"] = "You have been Registered Successfully";
            return RedirectToAction("Login");
        }

        public new ActionResult Profile()
        {
            User u = (User)Session[WebUtil.CURRENT_USER];
            if (u == null)
            {
                return RedirectToAction("Login", "User", new { ctl = "User", act = "Profile" });
            }
            ViewBag.products = new ProductHandler().ProductsByUser(u);
            ViewBag.recommended = new ProductHandler().RecommendedProductsForUser(u);
            ViewBag.categories = new ProductHandler().GetCategories();
            return View();
        }

        public ActionResult ProfileWithInterests(int id)
        {
            User u = (User)Session[WebUtil.CURRENT_USER];
            if (u == null)
            {
                return RedirectToAction("Login", "User", new { ctl = "User", act = "Profile" });
            }
            ViewBag.products = new ProductHandler().GetAllProductsByCategory(id);
            ViewBag.categories = new ProductHandler().GetCategories();
            return View();
        }

        public ActionResult UserManage()
        {
            User u = (User)Session[WebUtil.CURRENT_USER];
            if (u == null)
            {
                return RedirectToAction("Login", "User", new { ctl = "User", act = "UserManage" });
            }

            return View(u);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserManage(User user)
        {
            user.LoginID = user.Email;
            user.IsActive = true;
            if (ModelState.IsValid)
            {
                new UserHandler().UpdateUser(user);
                Session.Add(WebUtil.CURRENT_USER, new UserHandler().GetUser(user.Id));
                return RedirectToAction("Profile");
            }
            return View(user);
        }

        public ActionResult UserAdManage()
        {
            User u = (User)Session[WebUtil.CURRENT_USER];
            if (u == null)
            {
                return RedirectToAction("Login", "User", new { ctl = "User", act = "Profile" });
            }
            List<Product> userProducts = new ProductHandler().ProductsByUser(u);
            return View(userProducts);
        }

        public ActionResult UserAdDetail(int? id)
        {
            User u = (User)Session[WebUtil.CURRENT_USER];
            if (u == null)
            {
                return RedirectToAction("Login", "User", new { ctl = "User", act = "UserAdDetail" });
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product prod = new ProductHandler().GetProduct(id);
            if (prod == null)
            {
                return HttpNotFound();
            }
            return View(prod);
        }

        public ActionResult UserEditAd(int id)
        {
            User u = (User)Session[WebUtil.CURRENT_USER];
            if (u == null)
            {
                return RedirectToAction("Login", "User", new { ctl = "User", act = "UserEditProduct" });
            }

            Product prod = new ProductHandler().GetProduct(id);
            return View(prod);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserEditAd(Product prod)
        {
            ModelState.Remove("Category");
            ModelState.Remove("Images");
            if (ModelState.IsValid)
            {
                new ProductHandler().UpdateProduct(prod);
                return RedirectToAction("Profile");
            }
            return View(prod);
        }

        public ActionResult UserAdDelete(int? id)
        {
            User u = (User)Session[WebUtil.CURRENT_USER];
            if (u == null)
            {
                return RedirectToAction("Login", "User", new { ctl = "User", act = "UserAdDelete" });
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product prod = new ProductHandler().GetProduct(id);
            if (prod == null)
            {
                return HttpNotFound();
            }
            return View(prod);
        }

        [HttpPost, ActionName("UserAdDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult AdDeleteConfirmed(int id)
        {
            User u = (User)Session[WebUtil.CURRENT_USER];
            if (u == null)
            {
                return RedirectToAction("Login", "User", new { ctl = "User", act = "UserAdDelete" });
            }
            new ProductHandler().DeleteProduct(id);
            return RedirectToAction("UserAdManage");
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(recoveryEmail data)
        {

            if (ModelState.IsValid)
            {
                User user = new UserHandler().GetUserByEmail(data.Email);
                if (user == null)
                {
                    ViewBag.error = "Email Not Registered. Please Enter Registered Email Address";
                    return View();
                }

                try
                {
                    string randomnumb = Path.GetRandomFileName().Replace(".", "");
                    var message = new MailMessage { From = new MailAddress("eps.system54@gmail.com") };
                    message.To.Add(data.Email);
                    message.Subject = "-No-Reply- Password Recovery Email by EPS System";
                    message.IsBodyHtml = true;
                    message.Body = "Please use this password: <b><u>" + randomnumb +
                                   "</u></b> , Next Time You Login! And dont forget to change your password";

                    SmtpClient smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        Credentials = new System.Net.NetworkCredential
                            ("eps.system54@gmail.com", "eps.system786"),
                        EnableSsl = true
                    };



                    smtp.Send(message);
                    user.Password = randomnumb;
                    new UserHandler().UpdateUser(user);
                    ViewBag.success = "Email Has been sent to  " + data.Email;

                    ViewBag.HideSlider = true;
                    return View();
                }
                catch (Exception)
                {
                    ViewBag.error = "Error Sending Mail. Please Try Again Later!";
                    ViewBag.HideSlider = true;
                    return View();
                }
            }
            return View();
        }
        public ActionResult LogOff()
        {
            ActionResult obj;

            User u = (User)Session[WebUtil.CURRENT_USER];
            if (u != null && u.IsInRole(WebUtil.ADMIN_ROLE))
            {
                obj = RedirectToAction("Login", "User");
            }
            else
            {
                obj = RedirectToAction("Index", "Home");
            }

            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            HttpCookie ck = Request.Cookies["idpas"];
            if (ck != null)
            {
                ck.Expires = DateTime.Now;
                Response.SetCookie(ck);
            }
            return obj;
        }

        [HttpGet]
        public ActionResult CityLists(int id)
        {
            DDViewModel dm = new DDViewModel
            {
                Name = "CityId",
                Label = "- Your City -",
                Values = ModelHelper.ToSelectItemList(new LocationHandler().GetCities(new Country { Id = id }))
            };

            return PartialView("~/Views/Shared/_DDLView.cshtml", dm);
        }

        public ActionResult AllConverstaion()
        {
            User u = (User)Session[WebUtil.CURRENT_USER];
            if (u == null)
            {
                return RedirectToAction("Login", "User", new { ctl = "User", act = "AllConverstaion" });
            }

            //List<Converstion> convos = new MessagesHandler().Conversations(13, 12);
            List<Converstion> convos = new MessagesHandler().UserConversations(u.Id);
            return View(convos);
        }

        public int ConversationCount()
        {
            User currentUser = (User)Session[WebUtil.CURRENT_USER];
            if (currentUser != null)
            {
                int convoCount = new MessagesHandler().UserConversations(currentUser.Id).Count();
                return convoCount;
            }

            return 0;
        }

        public ActionResult Messages(int Id, int? convId)
        {
            User u = (User)Session[WebUtil.CURRENT_USER];
            if (u == null)
            {
                return RedirectToAction("Login", "User", new { ctl = "User", act = "Messages" });
            }

            List<Converstion> convoExist = new MessagesHandler().Conversations(u.Id, Id);

            if (convoExist == null || convoExist.Count == 0)
            {
                Converstion newConvo = new Converstion
                {
                    User1 = new UserHandler().GetUser(u.Id).Id,
                    User2 = new UserHandler().GetUser(Id).Id
                };
                new MessagesHandler().AddNewConverstion(newConvo);
            }

            if (convId == null)
            {
                List<Converstion> newconvoExist = new MessagesHandler().Conversations(u.Id, Id);
                ViewBag.convoId = newconvoExist.First().Id;
            }
            else
            {
                ViewBag.convoId = convId;
            }

            ViewBag.ToMsg = Id;
            ViewBag.OtherUsername = new UserHandler().GetUser(Id).FullName;
            ViewBag.FromMsg = u.Id;

            TempData["currentUser"] = u.Id;
            TempData.Keep("currentUser");
            return View();
        }

        public ActionResult GetMessages(int rcvId)
        {
            //int id = (int)TempData["currentUser"];
            User u = (User)Session[WebUtil.CURRENT_USER];
            int id = u.Id;
            List<Messages> sndmsgs = new MessagesHandler().GetAllSendMessages(id, rcvId);
            List<Messages> rcvdmsgs = new MessagesHandler().GetAllReceivedMessages(id, rcvId);
            sndmsgs.AddRange(rcvdmsgs);
            sndmsgs = sndmsgs.OrderBy(x => x.TimeOfMsg).ToList();
            return Json(new { list = sndmsgs }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddMessages(Messages msgData)
        {
            msgData.TimeOfMsg = DateTime.Now;
            msgData.ReadStatus = false;
            new MessagesHandler().AddMessage(msgData);
            return Json(msgData, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            DBContextClass db = new DBContextClass();
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}