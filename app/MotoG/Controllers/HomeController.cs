using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MotoG.Models;
using MotoG.Services;

namespace MotoG.Controllers
{
	public class HomeController : Controller
	{
		private readonly IPhoneInfoService _phoneInfo;
		private readonly IPriceInfoService _priceInfo;

		public HomeController(IPhoneInfoService phoneInfo, IPriceInfoService priceInfo)
		{
			_phoneInfo = phoneInfo;
			_priceInfo = priceInfo;
		}

		public async Task<IActionResult> Index()
		{
			var allPhones = await _phoneInfo.GetAll();
			return View(allPhones);
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
					Logo = await _priceInfo.GetLogo(p),
					Price = await _priceInfo.GetPrice(p)
				};
				displayInfo.Add(pd);
			}

			return View(displayInfo);
		}
	}
}