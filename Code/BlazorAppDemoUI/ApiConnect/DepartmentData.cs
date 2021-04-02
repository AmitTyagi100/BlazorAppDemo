using BlazorAppDemo.Data;
using Blazored.LocalStorage;
using CrossCuttingEntities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorAppDemo.ApiConnect
{
	public class DepartmentData : IDepartmentData
	{

		public HttpClient _httpClient { get; }
		public AppSettings _appSettings { get; }

		public ILocalStorageService _localStorageService { get; }

		public DepartmentData(HttpClient httpClient, IOptions<AppSettings> appSettings, ILocalStorageService localStorageService)
		{
			_appSettings = appSettings.Value;

			httpClient.BaseAddress = new Uri(_appSettings.BlazorDemoAPIAddress);
			//httpClient.DefaultRequestHeaders.Add("User-Agent", "BlazorServer");
			_localStorageService = localStorageService;
			_httpClient = httpClient;
		}


		public async Task<List<Department>> GetDepartments()
		{
			var requestMessage = new HttpRequestMessage(HttpMethod.Get, "Departments");

			var token = await _localStorageService.GetItemAsync<string>("accessToken");
			requestMessage.Headers.Authorization
				= new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

			var response = await _httpClient.SendAsync(requestMessage);

			var responseStatusCode = response.StatusCode;
			var responseBody = await response.Content.ReadAsStringAsync();

			return await Task.FromResult(JsonConvert.DeserializeObject<List<Department>>(responseBody));
		}
	}
}
