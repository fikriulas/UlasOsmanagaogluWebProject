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
        ISettingsRepository Settings { get; }
        IContactRepository Contacts { get; }
        IBlogCategoryRepository BlogCategory { get; }
        int SaveChanges();
    }
}
