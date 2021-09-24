using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Phones.Controllers;
using Phones.Models;
using Phones.Services;
using Rocks;
using Xunit;

namespace Phones.Tests
{
	public class HomeControllerTests
	{
		[Fact]
		public async Task CanGetPhoneNamesForList()
		{
			//arrange
			var infoService = Rock.Create<IPhoneInfoService>();
			infoService.Methods().GetNames().Returns(Task.FromResult(new[] { "Phone 1", "Phone 2", "Phone 3" }.AsEnumerable()));

			var displayService = Rock.Make<IPriceDisplayService>();

			var controller = new HomeController(infoService.Instance(), displayService.Instance());

			//act
			var response = await controller.Index() as ViewResult;

			//assert
			var names = response?.Model as IEnumerable<string>;
			Assert.Equal(3, names?.Count());
		}

		[Fact]
		public async Task CanGetPriceInfo()
		{
			//arrange
			var infoService = Rock.Create<IPhoneInfoService>();
			var info = new[]
			{
				new PhoneInfo { Name = "Phone 1", Store = "Store 1" },
				new PhoneInfo { Name = "Phone 1", Store = "Store 2" },
				new PhoneInfo { Name = "Phone 1", Store = "Store 3" }
			};
			infoService.Methods().GetInfo(Arg.Any<string>()).Returns(Task.FromResult(info.AsEnumerable()));

			var displayService = Rock.Create<IPriceDisplayService>();
			displayService.Methods().GetPriceViewModel(Arg.Any<PhoneInfo>()).Returns(Task.FromResult(new PriceViewModel()));
			displayService.Methods().GetImageData(Arg.Any<string>()).Returns("");

			var controller = new HomeController(infoService.Instance(), displayService.Instance());

			//act
			var response = await controller.Info("Phone 1") as ViewResult;

			//assert
			var phone = response?.Model as PhoneViewModel;
			Assert.Equal(3, phone?.Prices.Count());
		}
	}
}