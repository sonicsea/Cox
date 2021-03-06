﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cox.Models;
using Cox.ViewModels;
using Cox.Helpers;
using log4net;

namespace Cox.Controllers
{
    public class CategoryController : Controller
    {
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(CategoryController));

        // GET: Category
        public ActionResult Index()
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    log.Error("Unauthrized access to learning accessment page.");
                    return RedirectToAction("Login", "Home");
                }

                int currentSeq = 1;

                if (Session["currentseq"] == null)
                    Session["currentseq"] = 1;
                else
                    currentSeq = Convert.ToInt32(Session["currentseq"].ToString());

                int userID = Convert.ToInt32(Session["UserID"]);
                //Session["UserID"] = 1;

                //int specialTaskID = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Special_Task_ID"].ToString());

                CategoryViewModel categoryInfo = CoxLogic.GetCurrentCategory(currentSeq, userID);



                //if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["Special_Category_Sequence"].ToString()))
                //{
                //    int specialSeq = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Special_Category_Sequence"].ToString());



                //    if (currentSeq == specialSeq)
                //    {
                //        categoryInfo.tasks.Clear();
                //        Task supportTool = CoxLogic.GetSpecialTask(specialTaskID);

                //        categoryInfo.tasks.Add(supportTool);
                //    }
                //}


                return View(categoryInfo);
            }
            catch(Exception ex)
            {
                log.Error("Error on the accessment page", ex);
                
            }
            return View();
        }

        [HttpPost]
        public ActionResult SaveResponse(string name, bool isChecked)
        {
            int userID = Convert.ToInt32(Session["UserID"]);

            int topicID = Convert.ToInt32(name.Substring(2, name.IndexOf("Ta") - 2));
            int taskID = Convert.ToInt32(name.Substring(name.IndexOf("Ta") + 2));

            User_Topic_Task response = new User_Topic_Task();
            response.User_ID = userID;
            response.Topic_ID = topicID;
            response.Task_ID = taskID;

            if (isChecked)
                CoxLogic.SaveUserResponseAsync(response);
            else
                CoxLogic.DeleteUserResponseAsync(response);
            return View();
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Next")]
        public ActionResult SaveAndContinue()
        {

            try
            {

                int currentSeq = 1;

                if (Session["currentseq"] == null)
                    Session["currentseq"] = 1;
                else
                    currentSeq = Convert.ToInt32(Session["currentseq"].ToString());

                int userID = Convert.ToInt32(Session["UserID"]);
                //int specialTaskID = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Special_Task_ID"].ToString());

                CategoryViewModel categoryInfo = CoxLogic.GetCurrentCategory(currentSeq, userID);

                //foreach (Topic topic in categoryInfo.topics)
                //{

                //    //int specialSeq = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Special_Category_Sequence"].ToString());
                    
                //    foreach (Task task in categoryInfo.tasks)
                //    {
                //        User_Topic_Task response = new User_Topic_Task();
                //        response.User_ID = userID;
                //        response.Topic_ID = topic.ID;
                //        response.Task_ID = task.ID;

                //        if (Request["To" + topic.ID + "Ta" + task.ID] == null)
                //        {
                //            //CoxLogic.DeleteUserResponse(response);
                //            CoxLogic.DeleteUserResponseAsync(response);
                //        }
                //        else
                //        {
                //            //CoxLogic.SaveUserResponse(response);
                //            CoxLogic.SaveUserResponseAsync(response);
                //        }
                //    }
                    



                //}

                if (categoryInfo.IsLastCategory) {
                    
                        CoxLogic.UpdateUserReportDate(userID);
                        return RedirectToAction("Index", "Report", new { SendEmail = true });
                    

                }

                if (Session["currentseq"] == null)
                    Session["currentseq"] = 1;
                else
                    Session["currentseq"] = Convert.ToInt32(Session["currentseq"].ToString()) + 1;
            }

            catch(Exception ex)
            {
                log.Error("Error loading next category information. User ID: " + Session["UserID"], ex);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Later")]
        public ActionResult SaveAndLater()
        {

            try
            {

                int currentSeq = 1;

                if (Session["currentseq"] == null)
                    Session["currentseq"] = 1;
                else
                    currentSeq = Convert.ToInt32(Session["currentseq"].ToString());

                int userID = Convert.ToInt32(Session["UserID"]);
                //int specialTaskID = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Special_Task_ID"].ToString());

                CategoryViewModel categoryInfo = CoxLogic.GetCurrentCategory(currentSeq, userID);

                foreach (Topic topic in categoryInfo.topics)
                {

                    //int specialSeq = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Special_Category_Sequence"].ToString());

                    foreach (Task task in categoryInfo.tasks)
                    {
                        User_Topic_Task response = new User_Topic_Task();
                        response.User_ID = userID;
                        response.Topic_ID = topic.ID;
                        response.Task_ID = task.ID;

                        if (Request["To" + topic.ID + "Ta" + task.ID] == null)
                        {
                            CoxLogic.DeleteUserResponse(response);
                        }
                        else
                        {
                            CoxLogic.SaveUserResponse(response);
                        }
                    }




                }

                Session["currentseq"] = null;



                return RedirectToAction("Index", "Home");

                
            }

            catch (Exception ex)
            {
                log.Error("Error loading next category information. User ID: " + Session["UserID"], ex);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Previous")]
        public ActionResult Previous()
        {
            try
            {
                if (Session["currentseq"] == null)
                    Session["currentseq"] = 1;
                else
                    Session["currentseq"] = Convert.ToInt32(Session["currentseq"].ToString()) - 1;
            }
            catch (Exception ex)
            {
                log.Error("Error from Prevous button click. User ID: " + Session["UserID"], ex);
            }
            return RedirectToAction("Index");
        }

    }
}