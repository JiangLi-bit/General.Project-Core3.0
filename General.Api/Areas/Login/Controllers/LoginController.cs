using System;
using System.Linq;
using System.Threading.Tasks;
using General.Common;
using General.Framework;
using General.IService;
using General.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace General.Api.Areas.Login.Controllers
{
    /// <summary>
    /// 登录验证
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : BaseContoller
    {
        private readonly ISysUserService sysUserService;
        private readonly ISysPermissionService sysPermissionService;

        public LoginController(ISysUserService service, ISysPermissionService sysPermission)
        {
            this.sysUserService = service;
            this.sysPermissionService = sysPermission;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login(string account, string password)
        {
            string r = EncryptorHelper.GetMD5(Guid.NewGuid().ToString());
            if (!ModelState.IsValid)
            {
                AjaxData.Message = "请输入用户账号和密码";
                return Json(AjaxData);
            }
            var result = await sysUserService.validateUser(account, password, r);
            AjaxData.Status = result.Status;
            AjaxData.Message = result.Message;
            
            //登录成功后生成 token
            if (result.Status)
            {
                string roleList = string.Empty;
                var userId = result.User.Id;
                var list = sysPermissionService.GetRoleListById(userId);

                if (list.Count > 0)
                {
                    roleList = string.Join(',', list.Select(r => r.RoleName).ToArray()).Trim(',');
                }

                TokenModel model = new TokenModel() { Uid= userId.ToStr() ,Role = roleList };
                var token = JwtHelper.GenerateToken(model);

                AjaxData.Data = token;
            }
            return Json(AjaxData);
        }
    }
}