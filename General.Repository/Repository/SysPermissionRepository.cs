using General.Entity;
using General.IRepository;

namespace General.Repository
{
    public class SysPermissionRepository: BaseRepository<SysPermission>, ISysPermissionRepository
    {
        public SysPermissionRepository(GeneralDbContext context) :base(context)
        {
             
        }
    }
}
