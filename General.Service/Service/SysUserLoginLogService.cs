using General.Entity;
using General.IRepository;
using General.IService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace General.Service
{
    public class SysUserLoginLogService: BaseService<SysUserLoginLog>,ISysUserLoginLogService
    {
        private readonly ISysUserLoginLogRepository sysUserLoginLogRepository;

        public SysUserLoginLogService(ISysUserLoginLogRepository repository)
        {
            this.sysUserLoginLogRepository = repository;
            this.baseRepository = repository;
        }

        public async Task<int> CreateAsync(Guid userId, string msg)
        {
            var loginLog = new SysUserLoginLog()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                IpAddress = "",
                LoginTime = DateTime.Now,
                Message = "用户删除成功"
            };
            return await sysUserLoginLogRepository.CreateAsync(loginLog);
        }
    }
}
