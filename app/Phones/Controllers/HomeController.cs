﻿using System.Linq;
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
			var phoneInfo = await _phoneInfo.GetInfo(id);

			var priceTasks = phoneInfo.Select(GetPriceViewModel);
			var prices = await Task.WhenAll(priceTasks);

			foreach (var info in prices)
			{
				var average = prices.Average(i => i.Price);
				info.PercentOfAverage = info.Price / average;
			}

			var phone = new PhoneViewModel
			{
				Name = id,
				ImageData = _priceDisplay.GetImageData(id),
				Prices = prices
			};

			return View(phone);
		}

		private async Task<PriceViewModel> GetPriceViewModel(PhoneInfo p)
		{
			var logoURL = _priceDisplay.GetLogoURL(p.URL);
			var price = _priceDisplay.GetPrice(p);
			
			return new PriceViewModel
			{
				Store = p.Store,
				Link = p.URL,
				LogoURL = await logoURL,
				Price = await price
			};
		}
	}
}