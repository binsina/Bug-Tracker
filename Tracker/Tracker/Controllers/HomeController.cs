using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tracker.Models.ModelClass;
using System.Net;
using System.Net.Mail;
using Tracker.Migrations;
using System.Threading.Tasks;
using Tracker.Models;

namespace Tracker.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(Models.ModelClass.Email model)
        {
            if (ModelState.IsValid)
            {
                var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress("binsina9@gmail.com"));  // replace with valid value 
                message.From = new MailAddress("binsina9@gmail.com");  // replace with valid value
                message.Subject = "Your email subject";
                message.Body = string.Format(body, model.FromName, model.FromEmail, model.Message);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "AKIAIS6W6KKFYOKJ4D6A",  // replace with valid value
                        Password = "Ajm9Q3ZTnEBUUyAQs1nt8YM5UU3RpvW1Dinr+G6YVuYl"  // replace with valid value
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "email-smtp.us-west-2.amazonaws.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);
                    return RedirectToAction("Sent");
                }
            }
            return View(model);
        }


        public ActionResult Sent()
        {
            return View();
        }





        public FileContentResult UserPhotos()
        {
          
            if (User.Identity.IsAuthenticated)
            {
                
                //let pass User.Identity into userId
                String userId = User.Identity.GetUserId();

                if (userId == null)
                {
                    //if there is no photo chosen then use Stock photo- I am using CoderFoundry image
                    string fileName = HttpContext.Server.MapPath(@"~/Content/AdminLTE/img/bug.png");
                    //convert import image into byte file that can read by using FileStream and BinaryReader Method
                    byte[] imageData = null;
                    FileInfo fileInfo = new FileInfo(fileName);
                    long imageFileLength = fileInfo.Length;
                    FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    imageData = br.ReadBytes((int)imageFileLength);

                    return File(imageData, "image/png");

                }
                // to get the user details to load user Image 
                var bdUsers = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
                var UserImage = bdUsers.Users.Where(photo => photo.Id == userId).FirstOrDefault();

                return new FileContentResult(UserImage.UserPhoto, "image/jpeg");
            }
            else
            {
                string fileName = HttpContext.Server.MapPath(@"~/Content/AdminLTE/img/bug.png");

                byte[] imageData = null;
                FileInfo fileInfo = new FileInfo(fileName);
                long imageFileLength = fileInfo.Length;
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                imageData = br.ReadBytes((int)imageFileLength);
                return File(imageData, "image/png");

            }
        }


    }
}