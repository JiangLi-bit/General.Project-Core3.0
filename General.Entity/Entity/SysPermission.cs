using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entity
{
    [Table("SysPermission")]
    public partial class SysPermission
    {
        public Guid Id { get; set; }

        public int CategoryId { get; set; }

        public Guid RoleId { get; set; }

        public Guid Creator { get; set; }

        public DateTime CreationTime { get; set; }

         
    }
}
