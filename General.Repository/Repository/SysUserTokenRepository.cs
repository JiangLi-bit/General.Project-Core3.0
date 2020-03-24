using General.Entity;
using General.IRepository;

namespace General.Repository
{
    public class SysUserTokenRepository: BaseRepository<SysUserToken>,ISysUserTokenRepository
    {
        public SysUserTokenRepository(GeneralDbContext context) :base(context)
        {
        }
    }
}
