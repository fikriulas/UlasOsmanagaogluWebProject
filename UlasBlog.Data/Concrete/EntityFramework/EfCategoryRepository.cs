using UlasBlog.Data.Abstract;
using UlasBlog.Entity;

namespace UlasBlog.Data.Concrete.EntityFramework
{
    public class EfCategoryRepository : EfGenericRepository<Category>, ICategoryRepository
    {
        public EfCategoryRepository(AppDbContext context) : base(context)
        {

        }
        public AppDbContext AppDbContext
        {
            get { return context as AppDbContext; }
        }
    }
}