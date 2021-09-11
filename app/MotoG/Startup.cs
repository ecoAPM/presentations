using System.Data;
using System.Net.Http;
using AngleSharp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MotoG.Data;
using MotoG.Services;
using Npgsql;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace MotoG
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
			=> Configuration = configuration;

		private IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllersWithViews();

			services.AddTransient<IDbConnection>(_ => new NpgsqlConnection(Configuration.GetConnectionString("DB")));

			services.AddScoped(_ => AngleSharp.Configuration.Default.WithDefaultLoader());
			services.AddScoped<IBrowsingContext, BrowsingContext>();
			services.AddScoped<IPhoneInfoService, PhoneInfoService>();
			services.AddScoped<IPriceDisplayService, PriceDisplayService>();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseDeveloperExceptionPage();
			app.UseStaticFiles();
			app.UseRouting();
			app.UseEndpoints(e => e.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}"));

			var db = app.ApplicationServices.GetService<IDbConnection>();
			new Initializer(db).Run().GetAwaiter().GetResult();
		}
	}
}