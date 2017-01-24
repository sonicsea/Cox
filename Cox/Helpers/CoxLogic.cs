using Cox.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cox.Models;

namespace Cox.Helpers
{
    public static class CoxLogic
    {
        public static CategoryViewModel GetCurrentCategory(int categorySequence, int userID)
        {
            try
            {
                CategoryViewModel categoryInfo = new CategoryViewModel();

                var context = new CoxEntities();

                var categories = from c in context.Categories
                                 where c.Ordinal == categorySequence
                                 select c;

                categoryInfo.category = categories.First();

                var topics = from t in context.Topics
                             where t.Category_ID == categoryInfo.category.ID
                             select t;

                categoryInfo.topics = topics.ToList();

                //var tasks = from ta in context.Tasks
                //            select ta;

                categoryInfo.tasks = GetTasks(categoryInfo.category.ID);

                categoryInfo.userResponses = GetUserResponses(userID);

                if (categorySequence == GetAllCategories().Count()) categoryInfo.IsLastCategory = true;
                else categoryInfo.IsLastCategory = false;

                return categoryInfo;
            }
            catch(Exception ex)
            {
                throw new Exception("Error retrieveing current category information", ex.InnerException);
            }
        }


        public static List<Task> GetTasks(int categoryID)
        {
            try
            {
                var context = new CoxEntities();

                List<Task> tasks = new List<Task>();

                List<Category_Task> categoryTasks = context.Category_Task.Where(c => c.Category_ID == categoryID).ToList();

                if (categoryTasks.Count == 0)
                {

                    tasks = context.Tasks.Where(t => t.IsDefault == 1).ToList();


                }
                else
                {
                    foreach (Category_Task ct in categoryTasks)
                    {
                        tasks.Add(ct.Task);
                    }
                }

                return tasks;
            }
            catch(Exception ex)
            {
                throw new Exception("Error GetTasks: " + ex.Message, ex.InnerException);
            }
        }

        public static int GetCategoryID(int sequence)
        {
            int categoryID = 0;

            try
            {
                var context = new CoxEntities();

                categoryID = context.Categories.Where(c => c.Ordinal == sequence).FirstOrDefault().ID;

            }
            catch(Exception ex)
            {
                throw new Exception("Error GetCategoryID: " + ex.Message, ex.InnerException);
            }

            return categoryID;
        }

        public static List<User_Topic_Task> GetUserResponses(int userID)
        {
            try
            {
                List<User_Topic_Task> userResponses = new List<User_Topic_Task>();

                var context = new CoxEntities();

                var responses = from r in context.User_Topic_Task
                                where r.User_ID == userID
                                select r;

                foreach (var response in responses)
                {
                    userResponses.Add(response);
                }

                return userResponses;
            }
            catch(Exception ex)
            {
                throw new Exception("Error retrieveing user responses", ex.InnerException);
            }
        }

        public static void SaveUserResponse(User_Topic_Task response)
        {
            try
            {
                var context = new CoxEntities();


                if (ResponseExists(response) < 0)
                {

                    context.User_Topic_Task.Add(response);

                    context.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Error saving user response", ex.InnerException);
            }

        }

        public static void DeleteUserResponse(User_Topic_Task response)
        {
            try
            {
                var context = new CoxEntities();

                int targetID = ResponseExists(response);

                if (targetID > 0)
                {
                    var targetResponse = from r in context.User_Topic_Task
                                         where r.ID == targetID
                                         select r;

                    context.User_Topic_Task.Remove(targetResponse.First());
                    context.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Error deleting user response", ex.InnerException);
            }
        }

        public static int ResponseExists(User_Topic_Task response)
        {
            try
            {
                int responseID = -1;

                var context = new CoxEntities();

                foreach (var userResponse in context.User_Topic_Task)
                {
                    if (userResponse.User_ID == response.User_ID && userResponse.Topic_ID == response.Topic_ID && userResponse.Task_ID == response.Task_ID)
                    {
                        responseID = userResponse.ID;
                        break;
                    }
                }

                return responseID;
            }
            catch(Exception ex)
            {
                throw new Exception("Error from ResponseExists ", ex.InnerException);
            }
        }

        public static ReportViewModel GetReport(int userID)
        {
            try
            {
                ReportViewModel report = new ReportViewModel();

                var context = new CoxEntities();

                report.Categories = GetAllCategories();

                report.UserResponses = GetResponseByUserID(userID);

                report.Courses = context.Topic_Task_Course.ToList();

                report.CategoryTasks = new Dictionary<int, List<Task>>();

                foreach(Category c in report.Categories)
                {
                    List<Task> tasks = GetTasks(c.ID);
                    report.CategoryTasks.Add(c.ID, tasks);
                }

                return report;
            }
            catch(Exception ex)
            {
                throw new Exception("Error from GetReport ", ex.InnerException);
            }
        }

        public static List<Category> GetAllCategories()
        {
            try
            {
                var context = new CoxEntities();

                return context.Categories.ToList();
            }
            catch(Exception ex)
            {
                throw new Exception("Error from GetAllCategories ", ex.InnerException);
            }
        }

        //public static List<Task> GetAllTasks(int specialTaskID)
        //{
        //    try
        //    {
        //        var context = new CoxEntities();

        //        var tasks = from t in context.Tasks
        //                    where t.ID != specialTaskID
        //                    select t;

        //        return tasks.ToList();
        //    }
        //    catch(Exception ex)
        //    {
        //        throw new Exception("Error from GetAllTasks ", ex.InnerException);
        //    }
        //}

        public static List<User_Topic_Task> GetResponseByUserID(int userID)
        {
            try
            {
                var context = new CoxEntities();

                var responses = from r in context.User_Topic_Task
                                where r.User_ID == userID
                                select r;

                return responses.ToList();
            }
            catch(Exception ex)
            {
                throw new Exception("Error from GetResponseByUserID ", ex.InnerException);
            }
        }

        public static List<Topic_Task_Course> GetRelatedCourses(User_Topic_Task response)
        {
            try
            {
                var context = new CoxEntities();

                List<Topic_Task_Course> courses = new List<Topic_Task_Course>();

                foreach (var course in context.Topic_Task_Course)
                {
                    if (course.Topic_ID == response.Topic_ID && course.Task_ID == response.Task_ID)
                        courses.Add(course);
                }

                return courses;
            }
            catch(Exception ex)
            {
                throw new Exception("Error from GetRelatedCourses ", ex.InnerException);
            }
        }

        //public static Task GetSpecialTask(int specialTaskID)
        //{
        //    try
        //    {
        //        var context = new CoxEntities();

        //        var tasks = from t in context.Tasks
        //                    where t.ID == specialTaskID
        //                    select t;

        //        return tasks.First();
        //    }
        //    catch(Exception ex)
        //    {
        //        throw new Exception("Error from GetSpecialTask ", ex.InnerException);
        //    }
        //}

    }
}