using General.Entity;
using General.IRepository;

namespace General.Repository
{
    public class SysUserRepository: BaseRepository<SysUser>,ISysUserRepository
    {
        public SysUserRepository(GeneralDbContext context) : base(context)
        {
        }
    }
}
