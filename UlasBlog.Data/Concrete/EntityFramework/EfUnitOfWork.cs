using System;
using System.Collections.Generic;
using System.Text;
using UlasBlog.Data.Abstract;

namespace UlasBlog.Data.Concrete.EntityFramework
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext dbContext;
        public EfUnitOfWork(AppDbContext _dbContext)
        {
            dbContext = _dbContext ?? throw new ArgumentNullException("DbContext can not be null!");
        }

        private ICategoryRepository _categories;
        private IBlogRepository _blogs;
        private ICommentRepository _comments;
        private IContactRepository _contacts;
        private ISettingsRepository _settings;
        private IBlogCategoryRepository _BlogCategory;

        public ICategoryRepository Categories
        {
            get
            {
                return _categories ?? (_categories = new EfCategoryRepository(dbContext));
            }
        }

        public ICommentRepository Comments
        {
            get
            {
                return _comments ?? (_comments = new EfCommentRepository(dbContext));
            }
        }
        public IBlogRepository Blogs
        {
            get
            {
                return _blogs ?? (_blogs = new EfBlogRepository(dbContext));
            }
        }


        public IBlogCategoryRepository BlogCategory
        {
            get
            {
                return _BlogCategory ?? (_BlogCategory = new EfBlogCategoryRepository(dbContext));
            }
        }

        public IContactRepository Contacts
        {
            get
            {
                return _contacts ?? (_contacts = new EfContactRepository(dbContext));
            }
        }

        public ISettingsRepository Settings
        {
            get
            {
                return _settings ?? (_settings = new EfSettingsRepository(dbContext));
            }
        }

        public int SaveChanges()
        {
            try
            {
                return dbContext.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void Dispose()
        {
            dbContext.Dispose();
        }
    }
}
