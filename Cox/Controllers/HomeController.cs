using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cox.ViewModels;
using Cox.Helpers;
using log4net;

namespace Cox.Controllers
{
    public class HomeController : Controller
    {
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(CategoryController));
        public ActionResult Index()
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    log.Error("Unauthorized access to home page");
                    return RedirectToAction("Login");
                }


                return View(Security.GetUser(Convert.ToInt32(Session["UserID"])));
            }
            catch (Exception ex)
            {
                log.Error("Error from Home page: " + ex.Message + " User ID: " + Session["UserID"], ex);
                return null;
            }
        }
        

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserAuthViewModel userAuth)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int userID = Security.Authenticate(userAuth);

                    if (userID > 0)
                    {
                        Session["UserID"] = userID;
                        log.Info("User: " + userAuth.Email + " login successfully");
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Error"] = "Invalid email or password.";
                        log.Error("User: " + userAuth.Email + " login failed");
                    }
                }
                else
                {
                    log.Error("Error Model State from login action");
                }
                
            }
            catch(Exception ex)
            {
                log.Error(ex.Message, ex);
                
            }
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel newUser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (Security.EmailAlreadyExists(newUser.Email)) TempData["Error"] = "Email already exists.";
                    else
                    {
                        int userID = Security.Register(newUser);
                        if (userID > 0)
                        {
                            Session["UserID"] = userID;
                            log.Info("User: " + newUser.Email + " created and login successfully");
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            log.Error("User: " + newUser.Email + " creation failed");
                            TempData["Error"] = "Error creating accouting.";
                        }
                    }
                }
                else
                {
                    log.Error("Error Model State from register action");
                }
            }
            catch(Exception ex)
            {
                log.Error(ex.Message, ex);
                
            }
            return View();
        }
    }
}