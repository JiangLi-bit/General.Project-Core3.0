using General.Entity;
using General.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.IService
{
    public interface ISysPermissionService: IBaseService<SysPermission>
    {
        /// <summary>
        /// 通过Id获取角色信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<PermissionModel> GetRoleListById(Guid id);

        /// <summary>
        /// 通过Id获取权限信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<PermissionModel> GetPermissionListById(Guid id);
    }
}
