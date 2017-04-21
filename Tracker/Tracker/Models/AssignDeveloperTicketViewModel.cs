using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tracker.Models
{
    public class AssignDeveloperTicketViewModel
    {
        public Ticket Ticket { get; set; }
        public SelectList Developers { get; set; }
        public string SelectedUser { get; set; }
    }
}