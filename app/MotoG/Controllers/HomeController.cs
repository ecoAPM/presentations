using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MotoG.Models;
using MotoG.Services;

namespace MotoG.Controllers
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
			var phones = await _phoneInfo.GetAll();
			return View(phones);
		}

		public async Task<IActionResult> Info(string id)
		{
			var phoneInfo = await _phoneInfo.GetInfo(id);
			var displayInfo = new List<PriceDisplay>();
			foreach (var p in phoneInfo)
			{
				var pd = new PriceDisplay
				{
					PhoneInfo = p,
					LogoURL = await _priceDisplay.GetLogoURL(p),
					Price = await _priceDisplay.GetPrice(p)
				};
				displayInfo.Add(pd);
			}

			foreach (var info in displayInfo)
			{
				var average = displayInfo.Average(i => i.Price);
				info.PercentOfAverage = info.Price / average;
			}

			return View(displayInfo);
		}
	}
}