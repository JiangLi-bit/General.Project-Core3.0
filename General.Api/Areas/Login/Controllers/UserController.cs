using System;
using System.Linq;
using System.Threading.Tasks;
using General.Common;
using General.Entity;
using General.Framework;
using General.IService;
using General.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace General.Api.Areas.Login.Controllers
{
    /// <summary>
    /// 用户列表管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Permission.Name)]
    public class UserController : BaseContoller
    {
        private readonly ISysUserService sysUserService;
        private readonly IHttpContextAccessor httpContext;
         
        private readonly ISysUserLoginLogService sysUserLoginLogService;

        private Guid UserId;

        public UserController(ISysUserService service, 
            IHttpContextAccessor accessor,
            ISysUserLoginLogService loginLogService)
        {
            this.sysUserService = service;
            this.httpContext = accessor;
            sysUserLoginLogService = loginLogService;

            var currUid = (from item in httpContext.HttpContext.User.Claims where item.Type == "jti" select item.Value).FirstOrDefault().ToStr();
            UserId = new Guid(currUid);
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("list")]
        public IActionResult GetList()
        {
            var list =  sysUserService.GetAll().ToList();
            return Json(list);
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <returns></returns>
        [HttpPost("add")]
        public async Task<IActionResult> AddUser(RoleModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                if (!String.IsNullOrEmpty(model.MobilePhone))
                    model.MobilePhone = StringUitls.toDBC(model.MobilePhone);
                model.Name = model.Name.Trim();

                SysUser user = new SysUser();
                user.Id = Guid.NewGuid();
                user.Account = StringUitls.toDBC(model.Account.Trim());
                user.Name = model.Name;
                user.Email = model.Email;
                user.MobilePhone = model.MobilePhone;
                user.Sex = model.Sex;
                user.CreationTime = DateTime.Now;
                user.Salt = EncryptorHelper.CreateSaltKey();
                user.Enabled = true;
                user.IsAdmin = false;
                user.Password = EncryptorHelper.GetMD5("000000" + user.Salt);
                user.Creator = UserId;//暂时为null，做了JWT认证之后再加上去

                int count = await sysUserService.CreateAsync(user);
                if (count > 0)
                {
                    await sysUserLoginLogService.CreateAsync(user.Id, "添加用户");
                    AjaxData.Status = true;
                    AjaxData.Message = "操作成功";
                }
            }
            catch (Exception ex)
            {
                AjaxData.Status = false;
                AjaxData.Message = "操作失败>>"+ex.ToString();
            }
            return Json(AjaxData);
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <returns></returns>
        [HttpPost("edit")]
        public async Task<IActionResult> EditUser(RoleModel model,Guid id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                if (!String.IsNullOrEmpty(model.MobilePhone))
                    model.MobilePhone = StringUitls.toDBC(model.MobilePhone);
                model.Name = model.Name.Trim();

                var user = await sysUserService.GetById(id);
                if (user == null)
                {
                    AjaxData.Status = false;
                    AjaxData.Message = "没有找到当前要修改的数据！";
                }
                else
                {
                    user.Account = StringUitls.toDBC(model.Account.Trim());
                    user.Name = model.Name;
                    user.Email = model.Email;
                    user.MobilePhone = model.MobilePhone;
                    user.Sex = model.Sex;
                    user.ModifiedTime = DateTime.Now;
                    user.Modifier = UserId;  
                }
                int count = await sysUserService.EditAsync(user);
                if (count > 0)
                {
                    await sysUserLoginLogService.CreateAsync(user.Id, "修改用户");
                    AjaxData.Status = true;
                    AjaxData.Message = "操作成功";
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
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        //[Route("{id}")]
        [HttpPost("del")]
        public async Task<IActionResult> DelUser(Guid id)
        {
            try
            {
                var user =await sysUserService.GetById(id);
                if (user == null)
                {
                    AjaxData.Status = false;
                    AjaxData.Message = "没有找到当前要删除的数据！";
                }
                else
                {
                   int count = await sysUserService.RemoveAsync(user);
                    if (count > 0)
                    {
                        await sysUserLoginLogService.CreateAsync(user.Id, "删除用户");
                    }
                    AjaxData.Status = true;
                    AjaxData.Message = "操作成功";
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