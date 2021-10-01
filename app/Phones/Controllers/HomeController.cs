using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Phones.Models;
using Phones.Services;

namespace Phones.Controllers
{
	public class HomeController : Controller
	{
		private readonly IPhoneInfoService _phoneInfo;
		private readonly IPriceDisplayService _priceDisplay;
		private readonly IMemoryCache _cache;

		public HomeController(IPhoneInfoService phoneInfo, IPriceDisplayService priceDisplay, IMemoryCache cache)
		{
			_phoneInfo = phoneInfo;
			_priceDisplay = priceDisplay;
			_cache = cache;
		}

		public async Task<IActionResult> Index()
		{
			var names = await _phoneInfo.GetNames();
			return View(names);
		}

		public async Task<IActionResult> Info(string id)
		{
			var phone = await _cache.GetOrCreateAsync(id, async entry =>
			{
				entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
				return await GetPhoneViewModel(id);
			});

			return View(phone);
		}

		public IActionResult Image(string id)
		{
			var stream = new FileStream($"{id}.png", FileMode.Open);
			return File(stream, "image/png");
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
	}
}