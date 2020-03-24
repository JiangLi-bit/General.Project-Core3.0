using General.Entity;
using General.IRepository;
using General.IService;

namespace General.Service
{
    public class CategoryService: BaseService<Category>, ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoryService(ICategoryRepository repository)
        {
            this.categoryRepository = repository;
            this.baseRepository = repository;
        }
    }
}
