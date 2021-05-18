using System;
using System.Collections.Generic;
using System.Text;

namespace UlasBlog.Data.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
        IBlogRepository Blogs { get; }
        ICategoryRepository Categories { get; }
        ICommentRepository Comments { get; }
        IBlogCategoryRepository BlogCategory { get; }
        int SaveChanges();
    }
}
