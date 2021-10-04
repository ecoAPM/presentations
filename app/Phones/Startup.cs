using System.Data;
using AngleSharp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Phones.Data;
using Phones.Services;
using Npgsql;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Phones
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
			=> Configuration = configuration;

		private IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllersWithViews();
			services.AddHttpClient();
			services.AddMemoryCache();

			services.AddTransient<IDbConnection>(_ => new NpgsqlConnection(Configuration.GetConnectionString("DB")));
			services.AddTransient<IPhoneInfoService, PhoneInfoService>();

			services.AddSingleton(_ => AngleSharp.Configuration.Default.WithDefaultLoader());
			services.AddSingleton<IBrowsingContext, BrowsingContext>();
			services.AddSingleton<IPriceDisplayService, PriceDisplayService>();
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