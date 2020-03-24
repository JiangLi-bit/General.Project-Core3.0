using General.Entity;
using System;
using System.Threading.Tasks;

namespace General.IService
{
    public interface ISysUserLoginLogService: IBaseService<SysUserLoginLog>
    {
        Task<int> CreateAsync(Guid userId,string msg);
    }
}
