using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MStack.MainSite.Models
{
    /// <summary>
    /// 论坛主帖
    /// </summary>
    public class Topic
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishDateTime { get; set; }

        public Author Author { get; set; }
    }

    public class Author
    {
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; }
    }

    public class Comment
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CommentDateTime { get; set; }
        public Author Author { get; set; }
    }
}