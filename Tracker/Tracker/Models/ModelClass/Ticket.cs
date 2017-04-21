using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tracker.Models.ModelClass;

namespace Tracker.Models
{
    public class Ticket
    {
        public Ticket()
        {
            TicketAttachments = new HashSet<TicketAttachment>();
            TicketComments = new HashSet<TicketComment>();
            TicketHistories = new HashSet<TicketHistory>();
            TicketNotifications = new HashSet<TicketNotification>();
            TicketMilestones = new HashSet<Milestone>();

        }
        public int Id { get; set; }
        [AllowHtml]
        public string Description { get; set; }

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }

        [Display(Name = "Project" )]
        public int ProjectId { get; set; }

        public string Title { get; set; }

        [Display(Name = "Ticket Type")]
        public int TicketTypeId { get; set; }

        [Display(Name = "Ticket Status")]
        public int TicketStatusId { get; set; }

        [Display(Name = "Ticket Priority")]
        public int TicketPriorityId { get; set; }

        [Display(Name = "Owner")]
        public string UserId { get; set; }

        [Display(Name = "Assigned")]
        public string AssignedToUserId { get; set; }

        
        public string FileUrl { get; set; }


        public virtual ICollection<TicketAttachment> TicketAttachments {get; set;}
        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<TicketHistory> TicketHistories { get; set; }
        public virtual ICollection<TicketNotification> TicketNotifications { get; set; }
        public virtual ICollection<Milestone> TicketMilestones { get; set; }

        public virtual Project Project { get; set; }
        public virtual TicketPriority TicketPriority { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }
        public virtual TicketType TicketType { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationUser AssignedToUser { get; set; }


      



     
    }
}