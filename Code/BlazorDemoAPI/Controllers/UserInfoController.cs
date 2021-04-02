using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorDemoAPI.Models;
using CrossCuttingEntities;
using Microsoft.AspNetCore.Authorization;

namespace BlazorDemoAPI.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class UserInfoController : ControllerBase
	{
		private readonly BlazorDemoDbContext _context;

		public UserInfoController(BlazorDemoDbContext context)
		{
			_context = context;
		}

		// GET: api/UserInfo
		[HttpGet]
		 
		public async Task<ActionResult<IEnumerable<UserInfo>>> GettblUserInfo()
		{
			//return await _context.tblUserInfo.ToListAsync();

			var User_Info = await (from itemUserInfo in _context.tblUserInfo

								   join itemUser in _context.tblUser on itemUserInfo.UserId equals itemUser.UserId into ItemU
								   from _User in ItemU.DefaultIfEmpty()

								   join itemDept in _context.tblDepartment on itemUserInfo.DeptId.GetValueOrDefault() equals itemDept.DeptId into ItemD
								   from _Dept in ItemD.DefaultIfEmpty()


								   select new UserInfo
								   {


									   CreatedDate = itemUserInfo.CreatedDate,
									   DeptId = itemUserInfo.DeptId.GetValueOrDefault(),
									   FirstName = itemUserInfo.FirstName,
									   LastName = itemUserInfo.LastName,
									   ModifiedDate = itemUserInfo.ModifiedDate,
									   UserDepartment = _Dept == null ? new Department() { DeptId = 0, DeptName = "" } : new Department() { DeptId = _Dept.DeptId, DeptName = _Dept.DeptName },
									   UserId = itemUserInfo.UserId,
									   UserInfoId = itemUserInfo.UserInfoId,
									   EmailAddress = _User.EmailAddress,
									    IsActive=_User.IsActive
								   }
								).Cast<UserInfo>().ToListAsync();

			return User_Info;
		}

		// GET: api/UserInfo/5
		[HttpGet("{id}")]
		public async Task<ActionResult<UserInfo>> GetUserInfo(int id)
		{
			var userInfo = await _context.tblUserInfo.FindAsync(id);

			if (userInfo == null)
			{
				return NotFound();
			}

			return userInfo;
		}

		// PUT: api/UserInfo/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
		public async Task<IActionResult> PutUserInfo(int id, UserInfo userInfo)
		{
			if (id != userInfo.UserId)
			{
				return BadRequest();
			}

			userInfo.ModifiedDate = DateTime.Now;
			 
			_context.Entry(userInfo).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();


			}
			catch (DbUpdateConcurrencyException)
			{
				if (!UserInfoExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		// POST: api/UserInfo
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ActionResult<UserInfo>> PostUserInfo(UserInfo userInfo)
		{
			_context.tblUserInfo.Add(userInfo);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetUserInfo", new { id = userInfo.UserInfoId }, userInfo);
		}

		// DELETE: api/UserInfo/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteUserInfo(int id)
		{
			

			var user = await _context.tblUser.FindAsync(id);
			if (user == null)
			{
				return NotFound();
			}
			user.IsActive = false;

			user.ModifiedDate = DateTime.Now;

			_context.Entry(user).State = EntityState.Modified;


			try
			{
				await _context.SaveChangesAsync();


			}
			catch (DbUpdateConcurrencyException)
			{
				if (!UserInfoExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		private bool UserInfoExists(int id)
		{
			return _context.tblUserInfo.Any(e => e.UserInfoId == id);
		}
	}
}
