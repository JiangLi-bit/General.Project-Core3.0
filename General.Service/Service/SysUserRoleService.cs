using General.Entity;
using General.IRepository;
using General.IService;
using General.Repository;

namespace General.Service
{
    public class SysUserRoleService: BaseService<SysUserRole>,ISysUserRoleService
    {
        private ISysUserRoleRepository sysUserRoleRepository;

        public SysUserRoleService(ISysUserRoleRepository repository)
        {
            this.sysUserRoleRepository = repository;
            this.baseRepository = repository;
        }

    }
}
