using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cox.ViewModels;
using Cox.Helpers;
using log4net;
using System.Net.Mail;

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
                    //log.Error("Unauthorized access to home page");
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

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        //[AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(LostPasswordViewModel model)
        {

            if (ModelState.IsValid)
            {
                Cox.Models.User user = null;
                using (var context = new Cox.Models.CoxEntities())
                {
                        user = (from u in context.Users
                                         where u.Email == model.Email
                                         select u).FirstOrDefault();
                    
                }
                if (user != null)
                {
                    // Generae password token that will be used in the email link to authenticate user
                    var token = Security.CreateSalt(10);
                    // Generate the html link sent via email 
                    string resetLink = "<a href='"
                       + Url.Action("ResetPassword", "Home", new { rt = token, id = user.ID }, "http")
                       + "'>Reset Password Link</a>";

                    // Email stuff
                    //string subject = "Reset your password for Self-Directed Learning Assessment";
                    string subject = System.Configuration.ConfigurationManager.AppSettings["Email_Subject_Password_Reset"].ToString();
                    //string body = "You link: " + resetLink;
                    string from = System.Configuration.ConfigurationManager.AppSettings["Email_Sender"].ToString();
                    string body = System.Configuration.ConfigurationManager.AppSettings["Email_Body_Password_Reset"].ToString() + resetLink;

                    MailMessage message = new MailMessage(from, model.Email);
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;
                    //SmtpClient client = new SmtpClient();

                    // Attempt to send the email
                    try
                    {
                        Security.CreateNewResetPWSession(user.ID, token);
                        Mailer.SendMail(message);
                    }
                    catch (Exception e)
                    {
                        log.Error("Issue sending email: " + e.Message);
                    }
                }
                else // Email not found
                {
                    log.Error("LostPassword: " + model.Email + ". No user found for this email");
                    TempData["Error"] = "No user found for this email.";
                }
            }
            else
                log.Error("Error Model State from register action");

            /* You may want to send the user to a "Success" page upon the successful
            * sending of the reset email link. Right now, if we are 100% successful
            * nothing happens on the page. :P
            */
            return RedirectToAction("PasswordResetSendConfirmation");
        }

        public ActionResult PasswordResetSendConfirmation()
        {
            return View();
        }

        public ActionResult ResetPassword(string rt, string id)
        {
            ResetPasswordViewModel model = new ResetPasswordViewModel();
            model.ReturnToken = rt;
            model.UserID = Convert.ToInt32(id);
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (Security.IsResetSessionExpired(model.UserID, model.ReturnToken)) return RedirectToAction("ResetSessionExpired");

                Security.ResetPassword(model.UserID, model.ReturnToken, model.Password);
                
                    ViewBag.Message = "Successfully Changed";
                
            }
            return RedirectToAction("ResetSuccessful") ;
        }

        public ActionResult ResetSuccessful()
        {
            return View();
        }

        public ActionResult ResetSessionExpired()
        {
            return View();
        }

    }
}