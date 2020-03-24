using General.Entity;
using General.IRepository;

namespace General.Repository
{
    public class SysUserRoleRepository: BaseRepository<SysUserRole>,ISysUserRoleRepository
    {
        public SysUserRoleRepository(GeneralDbContext context) :base(context)
        {
        }
    }
}
