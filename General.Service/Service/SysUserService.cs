using General.Common;
using General.Entity;
using General.IRepository;
using General.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace General.Service
{
    public class SysUserService: BaseService<SysUser>,ISysUserService
    {
        private readonly ISysUserRepository sysUserRepository;
        private readonly ISysUserTokenRepository sysUserTokenRepository;
        private readonly ISysUserLoginLogRepository sysUserLoginLogReository;

        public SysUserService(ISysUserRepository sysUserRepository,
            ISysUserTokenRepository sysUserTokenRepository,
            ISysUserLoginLogRepository loginLogRepository)
        {
            this.baseRepository = sysUserRepository;
            this.sysUserRepository = sysUserRepository;
            this.sysUserTokenRepository = sysUserTokenRepository;
            this.sysUserLoginLogReository = loginLogRepository;
        }

        /// <summary>
        /// 验证登录状态
        /// </summary>
        /// <param name="account">登录账号</param>
        /// <param name="password">登录密码</param>
        /// <param name="r">登录随机数</param>
        /// <returns></returns>
        public async Task<(bool Status, string Message, string Token, SysUser User)> validateUser(string account, string password, string r)
        {
            var user = getByAccount(account);
            if (user == null)
                return (false, "用户名或密码错误", null, null);
            if (!user.Enabled)
                return (false, "你的账号已被冻结", null, null);

            if (user.LoginLock)
            {
                if (user.AllowLoginTime > DateTime.Now)
                {
                    return (false, "账号已被锁定" + ((int)(user.AllowLoginTime - DateTime.Now).Value.TotalMinutes + 1) + "分钟。", null, null);
                }
            }
            //密码加密
            password = EncryptorHelper.GetMD5(password + user.Salt);
            password = EncryptorHelper.GetMD5(password + r);

            var md5Password = EncryptorHelper.GetMD5(user.Password + r);
            //匹配密码
            if (password.Equals(md5Password, StringComparison.InvariantCultureIgnoreCase))
            {
                user.LoginLock = false;
                user.LoginFailedNum = 0;
                user.AllowLoginTime = null;
                user.LastLoginTime = DateTime.Now;
                user.LastIpAddress = "";

                //登录日志
                var userLoginLog =  new SysUserLoginLog(){
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    IpAddress = "",
                    LoginTime = DateTime.Now,
                    Message = "登录：成功"
                };
                await sysUserLoginLogReository.CreateAsync(userLoginLog);

                //单点登录,移除旧的登录token
                var userToken = new SysUserToken()
                {
                    Id = Guid.NewGuid(),
                    SysUserId = user.Id,
                    ExpireTime = DateTime.Now.AddDays(15)
                };
                await sysUserTokenRepository.CreateAsync(userToken);

                return (true, "登录成功", userToken.Id.ToString(), user);
            }
            else
            {
                //登录日志
                var userLoginLog = new SysUserLoginLog()
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    IpAddress = "",
                    LoginTime = DateTime.Now,
                    Message = "登录：成功"
                };
                await sysUserLoginLogReository.CreateAsync(userLoginLog);

                user.LoginFailedNum++;
                if (user.LoginFailedNum > 5)
                {
                    user.LoginLock = true;
                    user.AllowLoginTime = DateTime.Now.AddHours(2);
                }
            }
            return (false, "用户名或密码错误", null, null);
        }

        /// <summary>
        /// 通过账号获取用户
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public SysUser getByAccount(string account)
        {
            return sysUserRepository.GetAll().FirstOrDefault(o => o.Account == account && !o.IsDeleted);
        }
    }
}
