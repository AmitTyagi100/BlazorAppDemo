using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrossCuttingEntities
{
	[Table("tblUserInfo", Schema = "dbo")]
	public partial class UserInfo
	{
		[Key]
		public int UserInfoId { get; set; }

		public int? UserId { get; set; } = 0;

		[Display(Name = "First Name")]
		[Required]
		public string FirstName { get; set; }

		[Display(Name = "Last Name")]
		[Required]
		public string LastName { get; set; }

		[Display(Name = "DeptId")]


		private int? _deptId;
		public int? DeptId
		{
			get { return _deptId; }
			set
			{
				if (value == null)
				{ _deptId = 0; }
				else { _deptId = value; }
			}
		}




		[Display(Name = "Created Date")]
		public DateTime? CreatedDate { get; set; }

		[Display(Name = "Modified Date")]
		public DateTime? ModifiedDate { get; set; }
	}

	public partial class UserInfo
	{
		//[Display(Name = "Department Name")]
		//[NotMapped]
		//public string DepName { get; set; }

		[Display(Name = "Email Address")]
		[NotMapped]
		public string EmailAddress { get; set; }

		[Display(Name = "Is Active")]
		[NotMapped]
		public bool? IsActive { get; set; }

		[NotMapped]
		public virtual Department UserDepartment { get; set; }

	}
}
