using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tracker.Models
{
    public class ProjectDevViewModel
    {
        public Project Project { get; set; }
        public MultiSelectList DevUsers { get; set; }
        public  List<string> SelectedUsers { get; set; }




    }
}