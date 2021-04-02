using CrossCuttingEntities;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using BlazorAppDemo.ApiConnect;
using System.Net.Http;

namespace BlazorAppDemo.Data
{
	public class CustomAuthenticationStateProvider : AuthenticationStateProvider
	{


		public ILocalStorageService _localStorageService { get; }
		public IUserData _userService { get; set; }
		private readonly HttpClient _httpClient;

		public CustomAuthenticationStateProvider(ILocalStorageService localStorageService,
			IUserData userService,
			HttpClient httpClient)
		{
			//throw new Exception("CustomAuthenticationStateProviderException");
			_localStorageService = localStorageService;
			_userService = userService;
			_httpClient = httpClient;
		}

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var accessToken = await _localStorageService.GetItemAsync<string>("accessToken");

			//var accessTokenResult = await _localStorageService.GetItemAsync<string>("accessToken");
			//var accessToken = accessTokenResult.Success ? accessTokenResult.Value : string.Empty;

			ClaimsIdentity identity;

			if (accessToken != null && accessToken != string.Empty)
			{
				User user = await _userService.GetUserByAccessTokenAsync(accessToken);
				identity = GetClaimsIdentity(user);
			}
			else
			{
				identity = new ClaimsIdentity();
			}

			var claimsPrincipal = new ClaimsPrincipal(identity);

			return await Task.FromResult(new AuthenticationState(claimsPrincipal));
		}

		public async Task MarkUserAsAuthenticated(User user)
		{
			await _localStorageService.SetItemAsync("accessToken", user.AccessToken);
			await _localStorageService.SetItemAsync("refreshToken", user.RefreshToken);

			var identity = GetClaimsIdentity(user);

			var claimsPrincipal = new ClaimsPrincipal(identity);

			NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
		}

		public async Task MarkUserAsLoggedOut()
		{
			await _localStorageService.RemoveItemAsync("refreshToken");
			await _localStorageService.RemoveItemAsync("accessToken");

			var identity = new ClaimsIdentity();

			var user = new ClaimsPrincipal(identity);

			NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
		}

		private ClaimsIdentity GetClaimsIdentity(User user)
		{
			var claimsIdentity = new ClaimsIdentity();
			var username = string.Format("{0} {1}", user.User_Info.FirstName ==null ? string.Empty: user.User_Info.FirstName, user.User_Info.LastName == null ? string.Empty : user.User_Info.LastName);

			 
				claimsIdentity = new ClaimsIdentity(new[]
								{
									new Claim(ClaimTypes.Name, username ),
									new Claim(ClaimTypes.Email, user.EmailAddress==null ? string.Empty: user.EmailAddress),
									//new Claim(ClaimTypes.Role, <User Roles>),
									//new Claim("<Custom Policy name >", <Policy logic / function (user) >)
								}, "apiauth_type");
			 

			return claimsIdentity;
		}

		//private string <Policy logic / function >(User user)
		//{
		//     Custom logic
		//}
	}
}
