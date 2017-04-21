using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tracker.Models
{
    public class TicketsIndexViewModel
    {
        public ApplicationUser User { get; set; }
        public int ProjectId { get; set; }

        public string   Role { get; set;}
        public List<Ticket> AdminTickets { get; set; }
        public List<Ticket> DeveloperTickets { get; set; }
        public List<Ticket>SubmitterTickets { get; set; }
        public List<Ticket>ProjectManagerTickets { get; set; }
       



    }
}