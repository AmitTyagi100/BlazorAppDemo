using BlazorAppDemo.Data;
using BlazorAppDemo.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;



/// //


//using EmbeddedBlazorContent;
using System.Net.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using BlazorAppDemo.ApiConnect;
using CrossCuttingEntities;

namespace BlazorAppDemo
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			services.AddLocalization(Loc => Loc.ResourcesPath = "UiLocalization");

			services.AddRazorPages();
			services.AddServerSideBlazor();
			 

			var appSettingSection = Configuration.GetSection("AppSetting");
			services.Configure<AppSettings>(appSettingSection);

			services.AddTransient<ValidateHeaderHandler>();

			services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

			services.AddHttpClient<IAPIServices<User>, APIServices<User>>()
					.AddHttpMessageHandler<ValidateHeaderHandler>();

			services.AddHttpClient<IAPIServices<UserInfo>, APIServices<UserInfo>>()
				   .AddHttpMessageHandler<ValidateHeaderHandler>();


			services.AddBlazoredLocalStorage();

			services.AddHttpClient<IUserData, UserData>();

			services.AddHttpClient<IDepartmentData, DepartmentData>();

			services.AddSingleton<HttpClient>();
		}


		private RequestLocalizationOptions GetCultureOptions()
		{
			var cultures = Configuration.GetSection("Cultures")
				.GetChildren().ToDictionary(x => x.Key, x => x.Value);

			var supportedCultures = cultures.Keys.ToArray();

			var localizationOptions = new RequestLocalizationOptions()
				.AddSupportedCultures(supportedCultures)
				.AddSupportedUICultures(supportedCultures);

			return localizationOptions;
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRequestLocalization(GetCultureOptions());

			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapBlazorHub();
				endpoints.MapFallbackToPage("/_Host");
			});
		}
	}
}
