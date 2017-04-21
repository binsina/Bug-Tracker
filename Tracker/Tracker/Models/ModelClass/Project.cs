using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tracker.Models.ModelClass;

namespace Tracker.Models
{
    public class Project
    {
        public Project()
        {
            Tickets = new HashSet<Ticket>();
            Users = new HashSet<ApplicationUser>();
            Milestones = new HashSet<Milestone>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        [AllowHtml]
        public string Description { get; set; }
        
        public string Title { get; set; }
        public DateTimeOffset Created { get; set; }
        public string MediaUrl { get; set; }
        public string PMID { get; set; }


        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection <ProjectManager> ProjectManagers { get; set; }
        public  virtual ICollection <Milestone> Milestones { get; set; }
    }
}