using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cox.ViewModels;
using Cox.Helpers;
using Rotativa;
using log4net;

namespace Cox.Controllers
{
    public class ReportController : Controller
    {
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(CategoryController));
        // GET: Report
        public ActionResult Index()
        {
            if (Session["UserID"] == null)
            {
                log.Error("Unauthrized access to the report page.");
                return RedirectToAction("Login", "Home");
            }
            try
            {
                int userID = Convert.ToInt32(Session["UserID"]);


                //int specialTaskID = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Special_Task_ID"].ToString());

                ReportViewModel reportInfo = CoxLogic.GetReport(userID);



                return View(reportInfo);
            }
            catch(Exception ex)
            {
                log.Error(ex.Message, ex);
                return null;
            }
        }

        public ActionResult PDF(ReportViewModel pdfReportInfo)
        {

            return View(pdfReportInfo);
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Download")]
        public ActionResult DownloadPDF()
        {
            int userID = Convert.ToInt32(Session["UserID"]);


            //int specialTaskID = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Special_Task_ID"].ToString());

            ReportViewModel reportInfo = CoxLogic.GetReport(userID);

            return new ViewAsPdf("PDF", reportInfo) { FileName = "MyReport.pdf" };
        }

    }
}