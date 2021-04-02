using CrossCuttingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorAppDemo.ApiConnect
{
	public interface IDepartmentData
	{

		Task<List<Department>> GetDepartments();
	}
}
