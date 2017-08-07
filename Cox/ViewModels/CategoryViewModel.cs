using System.Collections.Generic;
using Cox.Models;

namespace Cox.ViewModels
{
    public class CategoryViewModel
    {
        public Category category { get; set; }
        public List<Topic> topics { get; set; }
        public List<Task> tasks { get; set; }

        public List<User_Topic_Task> userResponses { get; set; }

        public bool IsLastCategory { get; set; }

        public bool IsRegionSelector { get; set; }
    }
}