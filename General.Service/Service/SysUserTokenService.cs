using General.Entity;
using General.IRepository;
using General.IService;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Service
{
    public class SysUserTokenService: BaseService<SysUserToken>,ISysUserTokenService
    {
        private readonly ISysUserTokenRepository sysUserTokenRepository;

        public SysUserTokenService(ISysUserTokenRepository repository)
        {
            this.baseRepository = repository;
            this.sysUserTokenRepository = repository;
        }
    }
}
