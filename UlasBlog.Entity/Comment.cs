using System;
using System.Collections.Generic;
using System.Text;

namespace UlasBlog.Entity
{
    public class Comment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public DateTime dateAdded { get; set; }
        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
