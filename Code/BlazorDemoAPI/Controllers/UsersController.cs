using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorDemoAPI.Models;
using CrossCuttingEntities;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace BlazorDemoAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly BlazorDemoDbContext _context;
		private readonly JWTSettings _jwtsettings;

		public UsersController(BlazorDemoDbContext context, IOptions<JWTSettings> jwtsettings)
		{
			_context = context;
			_jwtsettings = jwtsettings.Value;
		}

		// GET: api/Users
		[HttpGet]
		public async Task<ActionResult<IEnumerable<User>>> GettblUser()
		{
			return await _context.tblUser.ToListAsync();
		}

		// GET: api/Users/5
		[HttpGet("{id}")]
		public async Task<ActionResult<User>> GetUser(int id)
		{
			var user = await _context.tblUser.FindAsync(id);

			if (user == null)
			{
				return NotFound();
			}

			return user;
		}

		// PUT: api/Users/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
		public async Task<IActionResult> PutUser(int id, User user)
		{
			if (id != user.UserId)
			{
				return BadRequest();
			}

			_context.Entry(user).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!UserExists(id))
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

		// POST: api/Users
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ActionResult<User>> PostUser(User user)
		{
			user.CreatedDate = user.CreatedDate == null ? DateTime.Now : user.CreatedDate;
			user.ModifiedDate = user.ModifiedDate == null ? DateTime.Now : user.ModifiedDate;

			_context.tblUser.Add(user);
			await _context.SaveChangesAsync();

			if (user.User_Info != null)
			{
				_context.tblUserInfo.Add(new UserInfo()
				{
					FirstName = user.User_Info.FirstName,
					LastName = user.User_Info.LastName,
					UserId = user.UserId,
					CreatedDate = DateTime.Now,
					ModifiedDate = DateTime.Now
				});

				await _context.SaveChangesAsync();
			}

			return CreatedAtAction("GetUser", new { id = user.UserId }, user);
		}

		// DELETE: api/Users/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteUser(int id)
		{
			var user = await _context.tblUser.FindAsync(id);
			if (user == null)
			{
				return NotFound();
			}

			user.ModifiedDate = DateTime.Now;
			user.IsActive = false;
			_context.Entry(user).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch { }
			//_context.tblUser.Remove(user);
			//await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool UserExists(int id)
		{
			return _context.tblUser.Any(e => e.UserId == id);
		}

		// Extra Code

		// POST: api/Users
		[HttpPost("Login")]
		public async Task<ActionResult<UserWithToken>> Login([FromBody] User user)
		{
			user = await _context.tblUser
										.Where(u => u.EmailAddress == user.EmailAddress
												&& u.Password == user.Password).FirstOrDefaultAsync();

			UserWithToken userWithToken = null;

			if (user != null)
			{
				RefreshToken refreshToken = GenerateRefreshToken();
				user.RefreshTokens.Add(refreshToken);
				await _context.SaveChangesAsync();

				userWithToken = new UserWithToken(user, null);
				userWithToken.RefreshToken = refreshToken.Token;
				//userWithToken.User_Info
				userWithToken.User_Info = (from itemUserInfo in _context.tblUserInfo

										   join itemUser in _context.tblUser on itemUserInfo.UserId equals itemUser.UserId into ItemU
										   from _User in ItemU.DefaultIfEmpty()

										   join itemDept in _context.tblDepartment on itemUserInfo.DeptId.GetValueOrDefault() equals itemDept.DeptId into ItemD
										   from _Dept in ItemD.DefaultIfEmpty()

										   where itemUserInfo.UserId == user.UserId
										   select new UserInfo
										   {


											   CreatedDate = itemUserInfo.CreatedDate,
											   DeptId = itemUserInfo.DeptId.GetValueOrDefault(),
											   FirstName = itemUserInfo.FirstName,
											   LastName = itemUserInfo.LastName,
											   ModifiedDate = itemUserInfo.ModifiedDate,
											   UserDepartment = _Dept == null ? null : new Department() { DeptId = _Dept.DeptId, DeptName = _Dept.DeptName },
											   UserId = itemUserInfo.UserId,
											   UserInfoId = itemUserInfo.UserInfoId,
											   EmailAddress = _User.EmailAddress,
											   IsActive = _User.IsActive

										   }
								).Cast<UserInfo>().FirstOrDefault();
			}

			if (userWithToken == null)
			{
				return NotFound();
			}

			//sign your token here here..
			userWithToken.AccessToken = GenerateAccessToken(user.UserId);
			return userWithToken;
		}

		private RefreshToken GenerateRefreshToken()
		{
			RefreshToken refreshToken = new RefreshToken();

			var randomNumber = new byte[32];
			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(randomNumber);
				refreshToken.Token = Convert.ToBase64String(randomNumber);
			}
			refreshToken.ExpiryDate = DateTime.UtcNow.AddMonths(6);

			return refreshToken;
		}

		private string GenerateAccessToken(int userId)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_jwtsettings.SecretKey);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim(ClaimTypes.Name, Convert.ToString(userId))
				}),
				Expires = DateTime.UtcNow.AddDays(1),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
				SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}

		// POST: api/Users
		[HttpPost("RegisterUser")]
		public async Task<ActionResult<UserWithToken>> RegisterUser([FromBody] User user)
		{

			user.CreatedDate = user.CreatedDate == null ? DateTime.Now : user.CreatedDate;
			user.ModifiedDate = user.ModifiedDate == null ? DateTime.Now : user.ModifiedDate;

			_context.tblUser.Add(user);
			await _context.SaveChangesAsync();






			if (user.User_Info != null)
			{
				_context.tblUserInfo.Add(new UserInfo()
				{
					FirstName = user.User_Info.FirstName,
					LastName = user.User_Info.LastName,
					UserId = user.UserId,
					CreatedDate = DateTime.Now,
					ModifiedDate = DateTime.Now
				});

				await _context.SaveChangesAsync();
			}




			//load role for registered user
			//user = await _context.tblUser.Include(u => u.Role)
			//                            .Where(u => u.UserId == user.UserId).FirstOrDefaultAsync();

			UserInfo userInfo = (from itemUserInfo in _context.tblUserInfo

								 join itemUser in _context.tblUser on itemUserInfo.UserId equals itemUser.UserId into ItemU
								 from _User in ItemU.DefaultIfEmpty()

								 join itemDept in _context.tblDepartment on itemUserInfo.DeptId equals itemDept.DeptId into ItemD
								 from _Dept in ItemD.DefaultIfEmpty()

								 where itemUserInfo.UserId == user.UserId
								 select new UserInfo
								 {


									 CreatedDate = itemUserInfo.CreatedDate,
									 DeptId = itemUserInfo.DeptId,
									 FirstName = itemUserInfo.FirstName,
									 LastName = itemUserInfo.LastName,
									 ModifiedDate = itemUserInfo.ModifiedDate,
									 UserDepartment = _Dept==null?null: new Department() { DeptId = _Dept.DeptId, DeptName = _Dept.DeptName },
									 UserId = itemUserInfo.UserId,
									 UserInfoId = itemUserInfo.UserInfoId,
									 EmailAddress = _User.EmailAddress,
									 IsActive = _User.IsActive
								 }
								).Cast<UserInfo>().FirstOrDefault();

			UserWithToken userWithToken = null;

			if (user != null)
			{
				RefreshToken refreshToken = GenerateRefreshToken();
				user.RefreshTokens.Add(refreshToken);
				await _context.SaveChangesAsync();

				userWithToken = new UserWithToken(user, userInfo);
				userWithToken.RefreshToken = refreshToken.Token;
			}

			if (userWithToken == null)
			{
				return NotFound();
			}

			//sign your token here here..
			userWithToken.AccessToken = GenerateAccessToken(user.UserId);
			return userWithToken;
		}

		// GET: api/Users
		[HttpPost("GetUserByAccessToken")]
		public async Task<ActionResult<User>> GetUserByAccessToken([FromBody] string accessToken)
		{
			User user = await GetUserFromAccessToken(accessToken);

			if (user != null)
			{
				return user;
			}

			return null;
		}

		private async Task<User> GetUserFromAccessToken(string accessToken)
		{
			try
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				var key = Encoding.ASCII.GetBytes(_jwtsettings.SecretKey);

				var tokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuer = false,
					ValidateAudience = false
				};

				SecurityToken securityToken;
				var principle = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out securityToken);

				JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

				if (jwtSecurityToken != null && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
				{
					var userId = principle.FindFirst(ClaimTypes.Name)?.Value;


					User user = await _context.tblUser
										.Where(u => u.UserId == Convert.ToInt32(userId)
												 ).FirstOrDefaultAsync();

					user.User_Info = (from itemUserInfo in _context.tblUserInfo

									  join itemUser in _context.tblUser on itemUserInfo.UserId equals itemUser.UserId into ItemU
									  from _User in ItemU.DefaultIfEmpty()

									  join itemDept in _context.tblDepartment on itemUserInfo.DeptId.GetValueOrDefault() equals itemDept.DeptId into ItemD
									  from _Dept in ItemD.DefaultIfEmpty()

									  where itemUserInfo.UserId == Convert.ToInt32(userId)
									  select new UserInfo
									  {


										  CreatedDate = itemUserInfo.CreatedDate,
										  DeptId = itemUserInfo.DeptId.GetValueOrDefault(),
										  FirstName = itemUserInfo.FirstName,
										  LastName = itemUserInfo.LastName,
										  ModifiedDate = itemUserInfo.ModifiedDate,
										  UserDepartment = _Dept == null ? null : new Department() { DeptId = _Dept.DeptId, DeptName = _Dept.DeptName },
										  UserId = itemUserInfo.UserId,
										  UserInfoId = itemUserInfo.UserInfoId,
										  EmailAddress = _User.EmailAddress,
										  IsActive = _User.IsActive
									  }
								).Cast<UserInfo>().FirstOrDefault();

					return user;
				}
			}
			catch (Exception)
			{
				return new User();
			}

			return new User();
		}

	}
}
