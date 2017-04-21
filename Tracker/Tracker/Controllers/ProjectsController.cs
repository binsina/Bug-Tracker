using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Tracker.Models;
using System.IO;
using System.Drawing;
using Tracker.HelperClass;

using Tracker.Helpers;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;


namespace Tracker.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects1
        public ActionResult Index()
        {
            var projs = db.Projects.ToList();
            ViewBag.projs = projs.Count();
            List<ProjectPMViewModel> model = new List<ProjectPMViewModel>();
            foreach (var p in projs)
            {
                ProjectPMViewModel vm = new ProjectPMViewModel();
                vm.Project = p;
                vm.ProjectManager = p.PMID != null ? db.Users.Find(p.PMID) : null;
                vm.Project.Milestones = p.Milestones;
               model.Add(vm);
            }

            return View(model);
        }

        //get
        public ActionResult AssignDEV(int id)
        {
            ProjectDevViewModel vm = new ProjectDevViewModel();
            UserRolesHelper helper = new UserRolesHelper();
            ProjectsHelper pHelper = new ProjectsHelper();

            var dev = helper.UsersInRole("Developer");

            var projectdev = pHelper.ProjectUsersByRole(id,"Developer").Select(u=> u.Id).ToArray();

            vm.DevUsers = new MultiSelectList(dev,"Id", "DisplayName",projectdev);
            vm.Project = db.Projects.Find(id);

            return View(vm);

        }


        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignDev(ProjectDevViewModel model)
        {
            ProjectsHelper helper = new ProjectsHelper();
            if (ModelState.IsValid)
            {
                var prj = db.Projects.Find(model.Project.Id);

                foreach (var usr in prj.Users)
                {
                    helper.RemoveUserFromProject(usr.Id, prj.Id);

                }
                foreach (var dev in model.SelectedUsers)
                {
                    helper.AddUserToProject(dev, prj.Id);

                }

                //db.SaveChanges();
                return RedirectToAction("Index", new { id = model.Project.Id });

            }

            return View(model.Project.Id);
        }

        //Get
        //[ValidateAntiForgeryToken]
        public ActionResult AssignPM(int id)
        {
            AdminProjectViewModel vm = new AdminProjectViewModel();
            UserRolesHelper helper = new UserRolesHelper();

            var pms = helper.UsersInRole("ProjectManager");

            vm.PMUsers = new SelectList(pms, "Id", "DisplayName");
            vm.Project = db.Projects.Find(id);

            return View(vm);


        }
        //Post
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult AssignPM (AdminProjectViewModel adminVm)
        {
            if (ModelState.IsValid)
            {
                var prj = db.Projects.Find(adminVm.Project.Id);
                prj.PMID = adminVm.SelectedUser;

                db.SaveChanges();
                return RedirectToAction("index");

            }
            return View(adminVm.Project.Id);
        }



        // GET: Projects1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Details",project);
            
        }

        // GET: Projects1/Create
        public ActionResult Create()
        {
            
            return View();
        }

        // POST: Projects1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Description,Title,Created,MediaUrl,MilestoneId")] Project project, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                var callbackUrl = Url.Action("Index", "Projects", new { ProjectId = project.Id}, protocol: Request.Url.Scheme);
                EmailService EmailSend = new EmailService();
                IdentityMessage message = new IdentityMessage();
                message.Body = "A new Project has been Created. If you want to review the new Project- Please Click <a href=\"" + callbackUrl + "\">here</a>";
                message.Subject = "New Project Created Today";
                
                //if you want to send it to one user- use this code
                //message.Destination = db.Users.Select(m => m.Email);


                //put the code into a loop to find all users with an Email from Profile
                foreach (var email in db.Users.Select(m => m.Email))
                {
                    //add it to Destination list
                    message.Destination = email;
                    //send Email
                    await(EmailSend.SendMailAsync(message));
                }


               

                project.Created = DateTimeOffset.Now;
               

                ImageUploadValidator validator = new ImageUploadValidator();
                if (validator.IsWebFriendlyImage(Image))
                {
                    var fileName = Path.GetFileName(Image.FileName);
                    Image.SaveAs(Path.Combine(Server.MapPath("~/images/"), fileName));
                    project.MediaUrl = "~/images/" + fileName;
                }

                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Title,Created,MediaUrl,MilestoneId")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects1/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
           
            db.SaveChanges();
            return RedirectToAction("Index");
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
