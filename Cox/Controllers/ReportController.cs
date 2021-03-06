﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cox.ViewModels;
using Cox.Helpers;
using log4net;
using Cox.Models;
using RotativaHQ.MVC5;

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
            if (Session["currentseq"] != null) Session["currentseq"] = null;

            try
            {
                int userID = Convert.ToInt32(Session["UserID"]);

                ReportViewModel reportInfo = CoxLogic.GetReport(userID);

                if (SendEmail == "True")
                {
                    SendEmailToSupervisor(userID, reportInfo);
                }

                return View(reportInfo);
                //int userID = Convert.ToInt32(Session["UserID"]);



                //ReportViewModel reportInfo = CoxLogic.GetReport(userID);
                //return RedirectToAction("PDF", reportInfo);
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

                //return new ViewAsPdf("PDF", reportInfo) { FileName = "Learning_Assessment_Report.pdf" };
                return new ViewAsPdf("PDF", reportInfo) { FileName = "Learning_Assessment_Report.pdf",
                    PageSize=Size.A4,
                    PageOrientation = Orientation.Portrait,
                    PageMargins = new Margins(10, 10, 10, 10),
                    CustomSwitches = "--disable-smart-shrinking"
                };
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



                    //var pdfResult = new ViewAsPdf("PDF", reportInfo) { FileName = reportName };


                    //var binary = pdfResult.BuildPdf(this.ControllerContext);
                    var binary = PdfHelper.GetPdf("~/Views/Report/PDF.cshtml", reportInfo, switches: "--disable-smart-shrinking");

                    System.IO.File.WriteAllBytes(Server.MapPath("~/App_Data/" + reportName), binary);



                    Mailer.SendMail(recipients, Server.MapPath("~/App_Data/" + reportName), user.FirstName);



                    if (System.IO.File.Exists(Server.MapPath("~/App_Data/" + reportName))) System.IO.File.Delete(Server.MapPath("~/App_Data/" + reportName));
                }

            }
            catch(Exception ex)
            {
                log.Error("Error Send report email to supervisor: " + ex.Message, ex);
            }
        }

    }
}