using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace General.ViewModels
{
    public class RoleModel
    {
        /// <summary>
        /// 账号
        /// </summary>
        [Required(ErrorMessage ="请输入账号，支持5~18位数字、字母组合")]
        [RegularExpression("^[1-9a-zA-Z]{5,18}$", ErrorMessage = "5~18数字、字母组合")]
        public string Account { get; set; }

        [Required(ErrorMessage = "请输入真实姓名")]
        public string Name { get; set; }

        [RegularExpression(@"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "邮箱格式错误")]
        public string Email { get; set; }

        [RegularExpression(@"^1[345678]\d{9}$", ErrorMessage = "请输入11位手机号")]
        public string MobilePhone { get; set; }
        public string Sex { get; set; }
    }
}
