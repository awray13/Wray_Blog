using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Wray_Blog.Models
{
    public class BlogPost
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }
        // The ? is intended to denote the nullablity
        // public Nullable<DateTime> Updated { get; set;}
        public DateTime? Updated { get; set; }

        public string Title { get; set; }
        public string Slug { get; set; }

        public string Abstract { get; set; }

        [AllowHtml]
        public string Body { get; set; }

        public string MediaUrl { get; set; }

        public bool IsPublished { get; set; }

        // Tell the Blog model that it has the potential for children
        public virtual ICollection<Comment> Comments { get; set; }

        public BlogPost()
        {
            Comments = new HashSet<Comment>();
        }


    }
}