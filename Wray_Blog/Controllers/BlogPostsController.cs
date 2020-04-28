using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Wray_Blog.Helpers;
using Wray_Blog.Models;

namespace Wray_Blog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BlogPosts
        
        public ActionResult Index()
        {
            return View(db.BlogPosts.OrderByDescending(b => b.Created).ToList());
            //return View(db.BlogPosts.ToList());
        }

        //public ActionResult PublishedIndex()
        //{
        //    return View(db.BlogPosts.OrderByDescending(b => b.Created).ToList());
        //}

        // GET: BlogPosts/Details/5
        [AllowAnonymous]
        public ActionResult Details(string Slug)
        {
            if (String.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.FirstOrDefault(b => b.Slug == Slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Create
        // How do I prevent someone from  getting to this Create View if they are not an Admin
        //[Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Body,MediaUrl,IsPublished")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                
                var slug = StringUtilities.URLFriendly(blogPost.Title);
                // I want to check for a few error conditions and if they exist I will return an error
                // First: Lets check if the slug is empty for some reason
                if (string.IsNullOrWhiteSpace(slug))
                {
                    // This is my opportunity to display a custom error using the ValidationMessageFor
                    // I determine where the error shows by specifying the property in the first set of quotes
                    ModelState.AddModelError("Title", "Title cannot be empty");
                    return View(blogPost);
                }

                // Second: We have to make sure this slug has not already been used on a previous BlogPost
                if (db.BlogPosts.Any(p => p.Slug == slug))
                {
                    // I can also display custom errors using the ValidationSummary by leaving the first set of quotes empty ""
                    ModelState.AddModelError("Title", "The title must be unique");
                    return View(blogPost);
                }

                

                // Class is a type, type is a class
                

                // If neither error conditions were detected then it is okay to use the slug variable to populate the BlogPost's Slug
                blogPost.Slug = slug;
                // hard code the date time 
                blogPost.Created = DateTime.Now;

                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    //1. I need to isolate just the file name and ignoe the extension
                    var justFileName = Path.GetFileNameWithoutExtension(image.FileName);
                    justFileName = StringUtilities.URLFriendly(justFileName);
                    justFileName = $"{justFileName}-{DateTime.Now.Ticks}";

                    justFileName = $"{justFileName}{Path.GetExtension(image.FileName)}";

                    //var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), justFileName));
                    blogPost.MediaUrl = "/Uploads/" + justFileName;
                }
                db.BlogPosts.Add(blogPost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Created,Title,Slug,Abstract,Body,MediaUrl,IsPublished")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {

                var slug = StringUtilities.URLFriendly(blogPost.Title);
                if (blogPost.Slug != slug)
                {
                    if (string.IsNullOrEmpty(slug))
                    {
                        ModelState.AddModelError("Title", "Oops, the Title cannot be empty!");
                        return View(blogPost);

                    }

                    if (db.BlogPosts.Any(b => b.Slug == slug))
                    {
                        ModelState.AddModelError("Title", "The title must be unique");
                        return View(blogPost);

                    }

                    blogPost.Slug = slug;
                }
                blogPost.Updated = DateTime.Now;

                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    //1. I need to isolate just the file name and ignoe the extension
                    var justFileName = Path.GetFileNameWithoutExtension(image.FileName);
                    justFileName = StringUtilities.URLFriendly(justFileName);
                    justFileName = $"{justFileName}-{DateTime.Now.Ticks}";

                    justFileName = $"{justFileName}{Path.GetExtension(image.FileName)}";

                    //var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), justFileName));
                    blogPost.MediaUrl = "/Uploads/" + justFileName;
                }



                db.Entry(blogPost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPost blogPost = db.BlogPosts.Find(id);
            db.BlogPosts.Remove(blogPost);
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
