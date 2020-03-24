using General.Entity;
using General.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using General.IRepository;
using General.ViewModels;
using General.Repository;
using General.Common;

namespace General.Service
{
    public class SysPermissionService: BaseService<SysPermission>, ISysPermissionService
    {
        private readonly GeneralDbContext dbContext;
        private ISysPermissionRepository sysPermissionRepository;

        public SysPermissionService(ISysPermissionRepository repository, GeneralDbContext context)
        {
            this.sysPermissionRepository = repository;
            this.baseRepository = repository;
            this.dbContext = context;
        }

        /// <summary>
        /// 通过Id获取角色信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<PermissionModel> GetRoleListById(Guid id)
        {
            var list = (from userRole in dbContext.SysUserRoles
                            join role in dbContext.SysRoles on userRole.RoleId equals role.Id
                            where userRole.UserId == id
                            select new PermissionModel
                            {
                                UserId = id,
                                RoleId = userRole.Id,
                                RoleName = role.Name
                            }).ToList();
            return list;
        }

        /// <summary>
        /// 通过Id获取权限信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<PermissionModel> GetPermissionListById(Guid id)
        {
            var list = (from userRole in dbContext.SysUserRoles
                        join role in dbContext.SysRoles on userRole.RoleId equals role.Id
                        join permission in dbContext.SysPermissions on role.Id equals permission.RoleId
                        join category in dbContext.Categories on permission.CategoryId equals category.Id
                        where userRole.UserId == id && category.SysResource != "/"
                        select new PermissionModel
                        {
                            UserId = id,
                            RoleId = userRole.Id,
                            RoleName = role.Name,
                            CategoryId = permission.CategoryId.ToString(),
                            Url = category.SysResource
                        }).ToList();
            return list;
        }
    }
}
