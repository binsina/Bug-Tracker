using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tracker.Models
{
    public class ProjectPMViewModel
    {
        public Project Project { get; set; }
        public ApplicationUser ProjectManager { get; set; }



    }
}