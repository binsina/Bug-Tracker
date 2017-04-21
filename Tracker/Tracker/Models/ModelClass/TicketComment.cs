using System;
using System.Web.Mvc;

namespace Tracker.Models
{
    public class TicketComment
    {
        public int Id { get; set; }

        [AllowHtml]
        public string Comment { get; set; }

        public DateTimeOffset? Created { get; set; }
        public int TicketId { get; set; }
        public int UserId { get; set; }

        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser User  { get; set; }
    }
}