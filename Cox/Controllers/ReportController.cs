using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cox.ViewModels;
using Cox.Helpers;

namespace Cox.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            if (Session["UserID"] == null) RedirectToAction("Login", "Home");
            int userID = Convert.ToInt32(Session["UserID"]);
            

            int specialTaskID = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Special_Task_ID"].ToString());

            ReportViewModel reportInfo = CoxLogic.GetReport(userID, specialTaskID);

            

            return View(reportInfo);
        }
    }
}