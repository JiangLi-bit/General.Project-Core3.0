using General.Entity;
using General.IRepository;

namespace General.Repository
{
    public class SysUserLoginLogRepository: BaseRepository<SysUserLoginLog>,ISysUserLoginLogRepository
    {
        public SysUserLoginLogRepository(GeneralDbContext context) :base(context)
        {
        }
    }
}
