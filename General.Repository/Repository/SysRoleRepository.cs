using General.Entity;
using General.IRepository;

namespace General.Repository
{
    public class SysRoleRepository: BaseRepository<SysRole>,ISysRoleRepository
    {
        public SysRoleRepository(GeneralDbContext context) :base(context)
        {
        }
    }
}
