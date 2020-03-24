using System;
using System.Collections.Generic;
using System.Text;

namespace General.ViewModels
{
    public class PermissionModel
    {
        public Guid UserId { get; set; }

        public Guid RoleId { get; set; }

        public string RoleName { get; set; }

        public string CategoryId { get; set; }

        public string Url { get; set; }
    }
}
