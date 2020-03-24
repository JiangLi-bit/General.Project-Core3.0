using General.Entity;
using General.IRepository;

namespace General.Repository
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(GeneralDbContext context) :base(context)
        {
            
        }
    }
}
