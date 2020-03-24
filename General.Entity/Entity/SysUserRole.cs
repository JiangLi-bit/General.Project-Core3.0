using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entity
{
    [Table("SysUserRole")]
    public partial class SysUserRole
    {
        public Guid Id { get; set; }

        public Guid RoleId { get; set; }

        public Guid UserId { get; set; }

         
    }
}
