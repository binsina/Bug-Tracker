
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tracker.HelperClass;
using Tracker.Helpers;
using Tracker.Models;
using Tracker.Models.ModelClass;

namespace Tracker.Controllers
{

    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: Tickets
        public ActionResult Index(int ProjectId)
        {
            TicketsIndexViewModel model = new TicketsIndexViewModel();
            UserRolesHelper Helper = new UserRolesHelper();

            var userId = User.Identity.GetUserId();
            if (User.IsInRole("Admin"))
            {
                var tickets = db.Tickets.Where(m=> m.ProjectId == ProjectId)
                    .Include(t => t.AssignedToUser)
                    .Include(t => t.User)
                    .Include(t => t.Project)
                    .Include(t => t.TicketMilestones)
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType);

                model.AdminTickets = tickets.ToList();

            }

            if (User.IsInRole("ProjectManager"))
            {
                var tickets = db.Projects.Where(p => p.PMID == userId && p.Id == ProjectId).SelectMany(p => p.Tickets)
                   .Include(t => t.User)
                    .Include(t => t.Project)
                    .Include(t => t.TicketMilestones)
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType);



                model.ProjectManagerTickets = tickets.ToList();
            }

            if (User.IsInRole("Submitter"))
            {
                var tickets = db.Tickets.Where(t => t.UserId == userId && t.ProjectId == ProjectId)
                     .Include(t => t.User)
                    .Include(t => t.Project)
                    .Include(t => t.TicketMilestones)
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType);
                    

                model.SubmitterTickets = tickets.ToList();
            }


            if (User.IsInRole("Developer"))
            {
                var tickets = db.Tickets.Where(t => t.AssignedToUserId == userId && t.ProjectId == ProjectId)
                    .Include(t => t.User)
                    .Include(t => t.Project)
                    .Include(t => t.TicketMilestones)
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType);


                model.DeveloperTickets = tickets.ToList();
            }

            model.ProjectId = ProjectId;
            
            return View(model);
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Tickets/Create
        //[Authorize(Roles = "Submitter")]
        public ActionResult Create(int ProjectId)

        {
            Ticket ticket = new Ticket();
            ticket.ProjectId = ProjectId;

            UserRolesHelper urh = new UserRolesHelper();

            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View(ticket);

        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public async Task<ActionResult> Create([Bind(Include = "FileUrl,Id,Description,ProjectId,Title,TicketTypeId,TicketStatusId,TicketPriorityId,UserId,AssignedToUserId")] Ticket ticket, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                var callbackUrl = Url.Action("Index", "Tickets", new { ProjectId = ticket.Id }, protocol: Request.Url.Scheme);
                EmailService EmailSend = new EmailService();
                IdentityMessage message = new IdentityMessage();
                message.Body = "A new Ticket has been Created and Assigned. If you want to review the new Ticket- Please Click <a href=\"" + callbackUrl + "\">here</a>";
                message.Subject = "New Ticket Created Today and Assigned";
                //message.Destination = db.Users.Select(m => m.Email);
                
                foreach (var email in db.Users.Select(m => m.Email))
                {
                    message.Destination = email;
                    await (EmailSend.SendMailAsync(message));
                }
                
                ImageUploadValidator validator = new ImageUploadValidator();
                if (validator.IsWebFriendlyImage(Image))
                {
                    var fileName = Path.GetFileName(Image.FileName);
                    Image.SaveAs(Path.Combine(Server.MapPath("~/images/Files/"), fileName));
                    ticket.FileUrl = "~/images/Files/" + fileName;
                }


                ticket.UserId = User.Identity.GetUserId();

                ticket.TicketPriorityId = db.TicketPriorities.FirstOrDefault(t => t.Name == "Low").Id;
                ticket.TicketStatusId = db.TicketStatus.FirstOrDefault(d => d.Name == "New").Id;

                
                ticket.Created = DateTime.UtcNow;




                db.Tickets.Add(ticket);
                db.SaveChanges();

                return RedirectToAction("Index", new { ProjectId = ticket.ProjectId });
            }
            UserRolesHelper urh = new UserRolesHelper();
            ViewBag.AssignedToUserId = new SelectList(urh.UsersInRole("Developer"), "Id", "FirstName", ticket.AssignedToUserId);

            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);

            return View(ticket);
        }

       
        // GET: Tickets/Edit/5
        public ActionResult DISP(int? id)

        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return PartialView("_DISP", ticket);
        }


        // GET: Tickets/Edit/5
        public ActionResult 
            Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);

            return PartialView("_Edit", ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Description,Created,Updated,ProjectId,Title,TicketTypeId,TicketStatusId,TicketPriorityId,OwnerUserId,AssignedToUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                Ticket oldTicket = db.Tickets.AsNoTracking().Where(m => m.Id == ticket.Id).FirstOrDefault();
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();

                if (oldTicket.Title != ticket.Title)
                {
                    TicketHistory tHistory = new TicketHistory();
                    tHistory.Changed = DateTimeOffset.Now;
                    tHistory.Property = "Title";
                    tHistory.OldValue = oldTicket.Title;
                    tHistory.NewValue = ticket.Title;
                    tHistory.TicketId = ticket.Id;
                    tHistory.UserId = User.Identity.GetUserId();
                    db.TicketHistories.Add(tHistory);
                    db.SaveChanges();
                    }

                if (oldTicket.Description != ticket.Description)
                {
                    TicketHistory history = new TicketHistory();
                    history.Changed = DateTimeOffset.Now;
                    history.Property = "Description";
                    history.OldValue = oldTicket.Description;
                    history.NewValue = ticket.Description;
                    history.TicketId = ticket.Id;
                    history.UserId = User.Identity.GetUserId();
                    db.TicketHistories.Add(history);
                    db.SaveChanges();

                }


                if (oldTicket.AssignedToUserId != ticket.AssignedToUserId)
                {
                    TicketHistory history = new TicketHistory();
                    history.Changed = DateTimeOffset.Now;
                    history.Property = "Asigned User";
                    if (oldTicket.AssignedToUserId == null)
                    {
                        history.OldValue = db.Users.Find(oldTicket.AssignedToUserId).FirstName;
                        history.NewValue = db.Users.Find(ticket.AssignedToUserId).FirstName;
                    }
                    else
                    {
                        history.OldValue = db.Users.Find(oldTicket.AssignedToUserId).FirstName;
                        history.NewValue = db.Users.Find(ticket.AssignedToUserId).FirstName;
                    }
                    history.TicketId = ticket.Id;
                    history.UserId = User.Identity.GetUserId();
                    db.TicketHistories.Add(history);
                    db.SaveChanges();
                         }


                if (oldTicket.TicketPriorityId != ticket.TicketTypeId)
                {
                    TicketHistory ticketHistory = new TicketHistory();
                    ticketHistory.Changed = DateTimeOffset.Now;
                    ticketHistory.Property = "TicketPriority";
                    ticketHistory.OldValue = db.TicketPriorities.Find(oldTicket.TicketPriorityId).Name;
                    ticketHistory.NewValue = db.TicketPriorities.Find(ticket.TicketPriorityId).Name;
                    ticketHistory.TicketId = ticket.Id;
                    ticketHistory.UserId = User.Identity.GetUserId();
                    db.TicketHistories.Add(ticketHistory);
                    db.SaveChanges();
                }

                if (oldTicket.TicketStatusId != ticket.TicketStatusId)
                {

                    TicketHistory ticketHistory = new TicketHistory();
                    ticketHistory.Changed = DateTimeOffset.Now;
                    ticketHistory.Property = "TicketStatus";
                    ticketHistory.OldValue = db.TicketStatus.Find(oldTicket.TicketStatusId).Name;
                    ticketHistory.NewValue = db.TicketStatus.Find(ticket.TicketStatusId).Name;
                    ticketHistory.TicketId = ticket.Id;
                    ticketHistory.UserId = User.Identity.GetUserId();
                    db.TicketHistories.Add(ticketHistory);
                    db.SaveChanges();
                }



                return RedirectToAction("Index", new { ProjectId = ticket.Id });
            }
            //ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            //ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            //ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            //ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);


            return View(ticket);
        }


       

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Delete",ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index", new { ProjectId = ticket.ProjectId});
        }

        //Add a milestone to Tickets
        [Authorize]
        [HttpPost]
        public ActionResult AddMilestones([Bind(Include = "Title,Description,DueDate,TicketId,ProjectId")] Milestone MilestoneMessage)
        {
            if (ModelState.IsValid)
            {

                var MessageBody = MilestoneMessage.Description;
                var MessageTitle = MilestoneMessage.Title;


                if (string.IsNullOrWhiteSpace(MessageBody))
                {
                    ModelState.AddModelError("Title", "Invalid Message");
                    return View();
                }
                else
                {
                    db.Milestones.Add(MilestoneMessage);
                    db.SaveChanges();
                    //return RedirectToAction("Details", new { id = MilestoneMessage.TicketId });

                }
            }
                return View();
            
        }


        //GET: Assign Developers to Tickets
        public ActionResult AssignDeveloperTicket(int ticketId)
        {
            AssignDeveloperTicketViewModel viewModel = new AssignDeveloperTicketViewModel();
            ProjectsHelper helper = new ProjectsHelper();


            var instanceofTicket = db.Tickets.Find(ticketId);
            var instanceofDeveloper = helper.ProjectUsersByRole(instanceofTicket.ProjectId, "Developer");
            var DeveloperT = 
            viewModel.Developers = new SelectList(instanceofDeveloper, "Id", "DisplayName");
            viewModel.Ticket = instanceofTicket;
            return View(viewModel);
        }

        //Post Assing Developers to Tickets
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignDeveloperTicket(AssignDeveloperTicketViewModel model)
        {
            if (ModelState.IsValid)
            {
                var Tkt = db.Tickets.Find(model.Ticket.Id);
                Tkt.AssignedToUserId = model.SelectedUser;
                db.SaveChanges();
                EmailService EmailSend = new EmailService();
                IdentityMessage message = new IdentityMessage();
                message.Body = "You have been assigned a new Issue Ticket.";
                message.Subject = "New Ticket Assigment";
                message.Destination = db.Users.Find(model.SelectedUser).Email;
                
                await (EmailSend.SendMailAsync(message));

                return RedirectToAction("Details", "Projects", new {id = Tkt.ProjectId} );

            }

            return View(model.Ticket.Id);
        }






        //Comments
        [Authorize]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult AddComment([Bind(Include = "Comment, Created,TicketId")] TicketComment commentMessage)
        {
            if (ModelState.IsValid)
            {
                var msgBody = commentMessage.Comment;
                commentMessage.Created = DateTimeOffset.Now;

                if (string.IsNullOrWhiteSpace(msgBody))
                {
                    ModelState.AddModelError("Title", "Invalid comment.");
                    return View();
                }
                else
                {
                    //commentMessage.Created = DateTimeOffset.Now;


                    //commentMessage.UserId = User.Identity.GetUserId();

                    db.TicketComments.Add(commentMessage);
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = commentMessage.TicketId });
                }
            }
            return View();
        }

        // GET: Comments/Edit/5
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment comment = await db.TicketComments.FindAsync(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditComment([Bind(Include = "Id,PostId,AuthorId,Created,Updated,UpdatedReason,Body")] TicketComment comment)
        {
            if (ModelState.IsValid)
            {
                TicketComment post = db.TicketComments.Find(comment.UserId);
                db.Entry(comment).State = EntityState.Modified;
                    
                await db.SaveChangesAsync();

                return RedirectToAction("Details");
            }
            return View(comment);
        }

        // GET: Comments/Delete/5
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment comment = await db.TicketComments.FindAsync(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Delete/5
        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> DeleteComment(int id)
        {

            TicketComment comment = await db.TicketComments.FindAsync(id);
            TicketComment post = db.TicketComments.Find(comment.UserId);
            if (comment == null)
            {
                return HttpNotFound();
            }
            db.TicketComments.Remove(comment);
            await db.SaveChangesAsync();
            return RedirectToAction("Details");

        }

       
        //Post Create attachment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAttachment([Bind(Include = "FileUrl,TicketId, UserId,Description")]TicketAttachment tattachment, HttpPostedFileBase attach)
        {
            if (ModelState.IsValid)
            {
                tattachment.Created = DateTimeOffset.Now;

                ImageUploadValidator validator = new ImageUploadValidator();
                if (validator.IsWebFriendlyImage(attach))
                {
                    var fileName = Path.GetFileName(attach.FileName);
                    attach.SaveAs(Path.Combine(Server.MapPath("~/images/Files/"), fileName));
                    tattachment.FileUrl = "~/images/Files/" + fileName;
                }
              



             
                Ticket ticket = db.Tickets.Find(tattachment.TicketId);


                //if (ticket.AssignedToUserId != null)
                //{
                //    ApplicationUser user = db.Users.Find(ticket.AssignedToUserId);
                //    Notification note = new Notification
                //    {
                //        TicketId = tatt.TicketId,
                //        UserId = user.Id,
                //        Change = "Attachment Added",
                //        Details = tatt.Description,
                //        DateNotified = now,
                //    };
                //    db.Notifications.Add(note);
                //    user.SendNotification(note);
                //}



                db.TicketAttachments.Add(tattachment);
                db.SaveChanges();
            }
            return RedirectToAction("Details", new { id = tattachment.TicketId });

        }






        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
