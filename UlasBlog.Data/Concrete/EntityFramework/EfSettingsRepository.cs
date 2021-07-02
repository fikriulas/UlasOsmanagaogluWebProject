using System;
using System.Collections.Generic;
using System.Text;
using UlasBlog.Data.Abstract;
using UlasBlog.Entity;

namespace UlasBlog.Data.Concrete.EntityFramework
{
    class EfSettingsRepository : EfGenericRepository<Settings>, ISettingsRepository
    {
        public EfSettingsRepository(AppDbContext context) : base(context)
        {

        }
        public AppDbContext AppDbContext
        {
            get { return context as AppDbContext; }
        }
    }
}
