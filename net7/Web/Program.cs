using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);
		builder.Services.AddDbContext<DB>(o => o.UseSqlite("data source=test.sqlite"));
		builder.Services.AddScoped<WebPeepController>();

		var app = builder.Build();

		app.MapGet("/", (WebPeepController controller) => controller.List());
		app.MapGet("/{id}", (WebPeepController controller, int id) => controller.Info(id));
		app.MapGet("/hi/{id}", (WebPeepController controller, int id) => controller.SayHello(id));
		app.MapPost("/", (WebPeepController controller, [FromBody] WebPerson peep) => controller.Add(peep));

		app.Run();
	}
}