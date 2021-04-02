using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrossCuttingEntities
{
    [Table("Department", Schema = "dbo")]

    public class Department
    {
        [Key]
        public short DeptId { get; set; } = 0;

        [Display(Name = "Department Name")]
        [Required]
        public string DeptName { get; set; } = "-";
    }
}
