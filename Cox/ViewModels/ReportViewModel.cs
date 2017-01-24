using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cox.Models;

namespace Cox.ViewModels
{
    public class ReportViewModel
    {
        public List<Category> Categories { get; set; }

        public List<User_Topic_Task> UserResponses { get; set; }

        public List<Topic_Task_Course> Courses { get; set; }

        //public List<Task> Tasks { get; set; }

        public Dictionary<int, List<Task>> CategoryTasks { get; set; }
        
    }
}