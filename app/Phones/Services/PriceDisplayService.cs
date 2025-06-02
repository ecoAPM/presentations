using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Phones.Models;

namespace Phones.Services
{
	public class PriceDisplayService : IPriceDisplayService
	{
		private readonly IBrowsingContext _browser;

		public PriceDisplayService(IBrowsingContext browser)
		{
			_browser = browser;
		}

		public async Task<PriceViewModel> GetPriceViewModel(PhoneInfo info)
		{
			var dom = await _browser.OpenAsync(info.URL);
			var logoURL = "https://" + new Url(info.URL).Host.Replace("api.", "") + "/favicon.ico";
			var price = GetPrice(dom, info);

			return new PriceViewModel
			{
				Store = info.Store,
				Link = info.URL,
				LogoURL = logoURL,
				Price = price
			};
		}

		public string GetImageData(string name)
		{
			var contents = File.ReadAllBytes($"{name}.png");
			return Convert.ToBase64String(contents);
		}

		private static decimal? GetPrice(IDocument dom, PhoneInfo info)
		{
			var findPrice = PriceFinder(info.Store);
			var text = findPrice(dom, info.PriceSelector);
			return decimal.TryParse(text, out var price)
				? price
				: null;
		}

		private static Func<IDocument, string, string> PriceFinder(string store)
		{
			return store switch
			{
				"BestBuy" => GetRegexPrice,
				"Motorola" => GetRegexPrice,
				"Newegg" => GetRegexPrice,
				_ => GetHTMLPrice
			};
		}

		private static string GetRegexPrice(IDocument dom, string selector)
		{
			var regex = new Regex(selector);
			var html = dom.Source.Text;
			var match = regex.Match(html);
			var capture = match.Groups.Values.Skip(1).FirstOrDefault();
			return capture?.Value;
		}

		private static string GetHTMLPrice(IParentNode dom, string selector)
			=> dom.QuerySelector(selector)?.TextContent.Trim().Split(" ")[0].Replace("$", "") ?? "";
	}
}