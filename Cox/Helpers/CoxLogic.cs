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
        public static CategoryViewModel GetCurrentCategory(int categorySequence, int userID, int specialTaskID)
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

                categoryInfo.tasks = GetAllTasks(specialTaskID);

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

        public static ReportViewModel GetReport(int userID, int specialTaskID)
        {
            try
            {
                ReportViewModel report = new ReportViewModel();

                var context = new CoxEntities();

                report.Categories = GetAllCategories();

                report.UserResponses = GetResponseByUserID(userID);

                report.Courses = context.Topic_Task_Course.ToList();

                report.Tasks = GetAllTasks(specialTaskID);

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

        public static List<Task> GetAllTasks(int specialTaskID)
        {
            try
            {
                var context = new CoxEntities();

                var tasks = from t in context.Tasks
                            where t.ID != specialTaskID
                            select t;

                return tasks.ToList();
            }
            catch(Exception ex)
            {
                throw new Exception("Error from GetAllTasks ", ex.InnerException);
            }
        }

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

        public static Task GetSpecialTask(int specialTaskID)
        {
            try
            {
                var context = new CoxEntities();

                var tasks = from t in context.Tasks
                            where t.ID == specialTaskID
                            select t;

                return tasks.First();
            }
            catch(Exception ex)
            {
                throw new Exception("Error from GetSpecialTask ", ex.InnerException);
            }
        }

    }
}