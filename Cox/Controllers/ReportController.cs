using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cox.ViewModels;
using Cox.Helpers;
using Rotativa;
using log4net;
using Cox.Models;

namespace Cox.Controllers
{
    public class ReportController : Controller
    {
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(ReportController));
        // GET: Report
        public ActionResult Index(string SendEmail)
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

                if (SendEmail == "True")
                {
                    SendEmailToSupervisor(userID, reportInfo);
                }

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
            try
            {
                int userID = Convert.ToInt32(Session["UserID"]);


                //int specialTaskID = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Special_Task_ID"].ToString());

                ReportViewModel reportInfo = CoxLogic.GetReport(userID);

                return new ViewAsPdf("PDF", reportInfo) { FileName = "MyReport.pdf" };
            }
            catch(Exception ex)
            {
                log.Error("Download PDF Error for User " + Session["UserID"] + ": " + ex.Message, ex);
                return null;
            }
        }

        private void SendEmailToSupervisor(int userID, ReportViewModel reportInfo)
        {
            try
            {
                string recipients = "";

                string reportName = "";

                using (var context = new CoxEntities())
                {
                    Cox.Models.User user = context.Users.Where(u => u.ID == userID).FirstOrDefault();
                    recipients = user.Email + "," + user.SupervisorEmail;

                    reportName = "Report_" + user.ID + "_" + user.FirstName + "_" + user.LastName + ".pdf";

                }

                var pdfResult = new ViewAsPdf("PDF", reportInfo);



                var binary = pdfResult.BuildPdf(this.ControllerContext);

                System.IO.File.WriteAllBytes(Server.MapPath("~/App_Data/" + reportName), binary);


                Mailer.SendMail(recipients, Server.MapPath("~/App_Data/" + reportName));



                if (System.IO.File.Exists(Server.MapPath("~/App_Data/" + reportName))) System.IO.File.Delete(Server.MapPath("~/App_Data/" + reportName));

            }
            catch(Exception ex)
            {
                log.Error("Error Send report email to supervisor: " + ex.Message, ex);
            }
        }

    }
}