using General.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace General.IService
{
    public interface ISysUserService: IBaseService<SysUser>
    {
        /// <summary>
        /// 验证登录状态
        /// </summary>
        /// <param name="account">登录账号</param>
        /// <param name="password">登录密码</param>
        /// <param name="r">登录随机数</param>
        /// <returns></returns>
        Task<(bool Status, string Message, string Token, SysUser User)> validateUser(string account, string password, string r);

        /// <summary>
        /// 通过账号获取用户
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        SysUser getByAccount(string account);
    }
}
