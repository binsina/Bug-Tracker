using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tracker.Models.ModelClass
{
    public class Milestone
    {
        public int Id   { get; set; }
        public string Title { get; set; }

        [AllowHtml]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTimeOffset DueDate { get; set; }

        //public int TicketId { get; set; }

        [Display(Name = "Project")]
        //public int ProjectId { get; set; }

        public int UserId { get; set; }


        public virtual Project Project { get; set; }
        //public virtual Ticket Ticket { get; set; }

        public virtual ApplicationUser User { get; set; }

    }
}