using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Phones.Models;
using Phones.Services;

namespace Phones.Controllers
{
	public class HomeController : Controller
	{
		private readonly IPhoneInfoService _phoneInfo;
		private readonly IPriceDisplayService _priceDisplay;

		public HomeController(IPhoneInfoService phoneInfo, IPriceDisplayService priceDisplay)
		{
			_phoneInfo = phoneInfo;
			_priceDisplay = priceDisplay;
		}

		public async Task<IActionResult> Index()
		{
			var names = await _phoneInfo.GetNames();
			return View(names);
		}

		public async Task<IActionResult> Info(string id)
		{
			var phone = await GetPhoneViewModel(id);
			return View(phone);
		}

		private async Task<PhoneViewModel> GetPhoneViewModel(string id)
		{
			var phoneInfo = await _phoneInfo.GetInfo(id);

			var priceTasks = phoneInfo.Select(p => _priceDisplay.GetPriceViewModel(p));
			var prices = await Task.WhenAll(priceTasks);

			foreach (var info in prices)
			{
				var average = prices.Average(i => i.Price);
				info.PercentOfAverage = info.Price / average;
			}

			return new PhoneViewModel
			{
				Name = id,
				Prices = prices
			};
		}

		public IActionResult Image(string id)
		{
			var stream = new FileStream($"{id}.png", FileMode.Open);
			return File(stream, "image/png");
		}
	}
}