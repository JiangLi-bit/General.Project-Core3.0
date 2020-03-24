using System;
using System.Linq;
using System.Threading.Tasks;
using General.Common;
using General.Entity;
using General.Framework;
using General.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace General.Api.Areas.Login.Controllers
{
    /// <summary>
    /// 角色列表管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Permission.Name)]
    public class RoleController : BaseContoller
    {
        private readonly ISysRoleService sysRoleService;
        private readonly IHttpContextAccessor httpContext;
        private readonly ISysUserLoginLogService sysUserLoginLogService;
        private Guid UserId;

        public RoleController(ISysRoleService service, 
            IHttpContextAccessor accessor,
            ISysUserLoginLogService loginLogService)
        {
            this.sysRoleService = service;
            this.httpContext = accessor;
            this.sysUserLoginLogService = loginLogService;

            var currUid = (from item in httpContext.HttpContext.User.Claims where item.Type == "jti" select item.Value).FirstOrDefault().ToStr();
            UserId = new Guid(currUid);
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("list")]
        public IActionResult GetList()
        {
            var list = sysRoleService.GetAll().ToList();
            return Json(list);
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <returns></returns>
        [HttpPost("add")]
        public async Task<IActionResult> AddRole(string roleName)
        {
            try
            {
                if (string.IsNullOrEmpty(roleName))
                {
                    AjaxData.Status = false;
                    AjaxData.Message = "角色名称不能为空！！";
                }
                else
                {
                    SysRole role = new SysRole();
                    role.Id = Guid.NewGuid();
                    role.Name = roleName;
                    role.CreationTime = DateTime.Now;
                    role.Creator = UserId;//暂时为null，做了JWT认证之后再加上去
                    int count = await sysRoleService.CreateAsync(role);
                    if (count > 0)
                    {
                        await sysUserLoginLogService.CreateAsync(UserId, "添加角色");
                        AjaxData.Status = true;
                        AjaxData.Message = "操作成功";
                    }
                }
            }
            catch (Exception ex)
            {
                AjaxData.Status = false;
                AjaxData.Message = "操作失败>>" + ex.ToString();
            }
            return Json(AjaxData);
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        /// <returns></returns>
        [HttpPost("edit")]
        public async Task<IActionResult> EditRole(Guid id,string roleName)
        {
            try
            {
                var user = await sysRoleService.GetById(id);
                if (string.IsNullOrEmpty(roleName))
                {
                    AjaxData.Status = false;
                    AjaxData.Message = "角色名称不能为空！！";
                    return Json(AjaxData);
                }

                if (user == null)
                {
                    AjaxData.Status = false;
                    AjaxData.Message = "没有找到当前要修改的数据！";
                }
                else
                {
                    user.Name = roleName;
                    user.ModifiedTime = DateTime.Now;
                    user.Modifier = UserId;

                    int count = await sysRoleService.EditAsync(user);
                    if (count > 0)
                    {
                        await sysUserLoginLogService.CreateAsync(UserId, "修改角色");
                        AjaxData.Status = true;
                        AjaxData.Message = "操作成功";
                    }
                }
                 
            }
            catch (Exception ex)
            {
                AjaxData.Status = false;
                AjaxData.Message = "操作失败>>" + ex.ToString();
            }
            return Json(AjaxData);
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <returns></returns>
        [HttpPost("del")]
        public async Task<IActionResult> DelUser(Guid id)
        {
            try
            {
                var user = await sysRoleService.GetById(id);
                if (user == null)
                {
                    AjaxData.Status = false;
                    AjaxData.Message = "没有找到当前要删除的数据";
                }
                else
                {
                    int count = await sysRoleService.RemoveAsync(user);
                    if (count > 0)
                    {
                        await sysUserLoginLogService.CreateAsync(UserId, "删除角色");
                        AjaxData.Status = true;
                        AjaxData.Message = "操作成功";
                    }
                }
            }
            catch (Exception ex)
            {
                AjaxData.Status = false;
                AjaxData.Message = "操作失败>>" + ex.ToString();
            }
            return Json(AjaxData);
        }
    }
}