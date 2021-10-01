using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Phones.Controllers;
using Phones.Models;
using Phones.Services;
using Xunit;

namespace Phones.Tests
{
	public class HomeControllerTests
	{
		[Fact]
		public async Task CanGetPhoneNamesForList()
		{
			//arrange
			var infoService = Substitute.For<IPhoneInfoService>();
			infoService.GetNames().Returns(new[] { "Phone 1", "Phone 2", "Phone 3" });

			var displayService = Substitute.For<IPriceDisplayService>();
			var cache = Substitute.For<IMemoryCache>();

			var controller = new HomeController(infoService, displayService, cache);

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
			var infoService = Substitute.For<IPhoneInfoService>();
			var info = new[]
			{
				new PhoneInfo { Name = "Phone 1", Store = "Store 1" },
				new PhoneInfo { Name = "Phone 1", Store = "Store 2" },
				new PhoneInfo { Name = "Phone 1", Store = "Store 3" }
			};
			infoService.GetInfo(Arg.Any<string>()).Returns(info);

			var displayService = Substitute.For<IPriceDisplayService>();
			displayService.GetPriceViewModel(Arg.Any<PhoneInfo>()).Returns(new PriceViewModel());

			var cache = Substitute.For<IMemoryCache>();

			var controller = new HomeController(infoService, displayService, cache);

			//act
			var response = await controller.Info("Phone 1") as ViewResult;

			//assert
			var phone = response?.Model as PhoneViewModel;
			Assert.Equal(3, phone?.Prices.Count());
		}
	}
}