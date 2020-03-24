using General.Entity;
using General.IRepository;
using General.IService;
using General.Repository;
using General.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace General.Service
{
    public class SysRoleService: BaseService<SysRole>,ISysRoleService
    {
        private readonly ISysRoleRepository sysRoleRepository;
        private readonly GeneralDbContext dbContext;

        public SysRoleService(ISysRoleRepository repository,GeneralDbContext context)
        {
            this.sysRoleRepository = repository;
            this.baseRepository = repository;
            this.dbContext = context;
        }

    }
}
