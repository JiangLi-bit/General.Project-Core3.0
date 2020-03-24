using System;
using System.ComponentModel.DataAnnotations;

namespace General.ViewModels
{
    public class LoginModel
    {
        /// <summary>
        /// 账号
        /// </summary>
        [Required(ErrorMessage = "请输入账号")]
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "请输入密码")]
        public string Password { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string R { get; set; }
    }
}
