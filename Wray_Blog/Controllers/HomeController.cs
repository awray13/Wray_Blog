using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using Wray_Blog.Helpers;
using Wray_Blog.Models;

namespace Wray_Blog.Controllers
{
    
    [RequireHttps]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private SearchHelper SearchHelper = new SearchHelper();

        public ActionResult Index(int? page, string searchStr)
        {
            ViewBag.Search = searchStr;
            var blogPosts = SearchHelper.IndexSearch(searchStr);
            
            int pageSize = 3; // display three blogposts at a time on this page
            int pageNumber = (page ?? 1);

            return View(blogPosts.ToPagedList(pageNumber, pageSize));

            // Get all the BlogPosts that have been marked as published to the page that the User will see
            //return View(db.BlogPosts.Where(foo => foo.IsPublished).OrderByDescending(foo => foo.Created).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            EmailModel model = new EmailModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = "<p>Email From: <bold>{0}</bold>({1})</p><p>Message:</p><p>{2}</p>";
                    var from = $"Ashton's Blog<{WebConfigurationManager.AppSettings["emailfrom"]}>";
                    model.Body = "This is a message from your portfolio site. The name and the email of the contacting person is above.";

                    var email = new MailMessage(from, WebConfigurationManager.AppSettings["emailto"])
                    {
                        Subject = "Portfolio Contact Email",
                        Body = string.Format(body, model.FromName, model.FromEmail, model.Body),
                        IsBodyHtml = true
                    };

                    var svc = new EmailService();
                    await svc.SendAsync(email);

                    //return View(new EmailModel());
                    return RedirectToAction("Index", "Home");

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                    
                }
            }
            return View(model);
        }

    }
}