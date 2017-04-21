﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Tracker.HelperClass;
using Tracker.Models;

namespace Tracker.Controllers
{
    public class AdminUserController : Controller
    {

        ApplicationDbContext db = new ApplicationDbContext();



        // GET: AdminUser
        public ActionResult Users()
        {
            var users = new List<UsersViewModel>();
            UserRolesHelper helper = new UserRolesHelper();

            foreach (var user in db.Users)
            {
                var uservm = new UsersViewModel();
                uservm.User = user;
                uservm.Roles = helper.ListUserRoles(user.Id).ToList();
                users.Add(uservm);

            }

            return View(users);
        }

        //GET: List Tickets
        public ActionResult ListTickets(int? projectId)
        {
            if (projectId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(projectId);
            if (project == null)
            {
                return HttpNotFound();
            }
            ViewBag.Title = project.Name;
            return View(project.Tickets.OrderByDescending(t => t.Created).ToList());
        }

        public ActionResult EditUser(string Id)
        {
            var user = db.Users.Find(Id);
            var roleList = db.Roles.Select(r => new UserRoleViewModel { Name = r.Name, UserId = Id, IsInRole = r.Users.Any(u => u.UserId == Id) });
            var selected = roleList.Where(r => r.IsInRole).Select(n => n.Name).ToArray();
            var selectList = new MultiSelectList(roleList, "Name", "Name", selected);
            var model = new AdminUserViewModel
            {
                User = user,
                Roles = selectList,
                SelectedRoles = selected
            };

            return View(model);
        }


        [HttpPost]
        public ActionResult EditUser(AdminUserViewModel model)
        {
            model.User = db.Users.Find(model.User.Id);
            var um = Request.GetOwinContext().Get<ApplicationUserManager>();
            string[] sel = { };
            var SelRoles = model.SelectedRoles != null ? model.SelectedRoles : sel;
            foreach (var role in db.Roles.ToList())
            {
                if (SelRoles.Contains(role.Name))
                    um.AddToRole(model.User.Id, role.Name);
                else
                    if (!(role.Name == "Admin" && model.User.UserName == "binsina9@gmail.com"))
                    um.RemoveFromRole(model.User.Id, role.Name);
            }
            //return RedirectToAction("EditUser", new { Id = model.User.Id });
            return RedirectToAction("DetailUserRoles", new { Id = model.User.Id });
        }



        public ActionResult DetailUserRoles(string Id)
        {
            var user = db.Users.Find(Id);
            UserRolesHelper helper = new UserRolesHelper();
            var urvm = new UserRolesVM();
            urvm.UserId = user.Id;
            urvm.UserName = user.DisplayName;
            urvm.Roles = helper.ListUserRoles(user.Id);
            return View(urvm);


        }








    }
}

























