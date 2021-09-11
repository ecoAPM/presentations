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

		public string GetImageData(string name)
		{
			var contents = File.ReadAllBytes($"{name}.png");
			return Convert.ToBase64String(contents);
		}

		public async Task<string> GetLogoURL(string url)
		{
			var dom = await GetDocument(url);
			var element = dom.QuerySelector(@"link[rel~=""icon""]");
			var logoURL = element?.Attributes["href"]?.Value ?? "/favicon.ico";
			return wrap(url, logoURL);
		}

		private async Task<IDocument> GetDocument(string url)
		{
			return await _browser.OpenAsync(url);
		}

		private static string wrap(string page, string img) => img.Contains("//") ? img : "https://" + new Url(page).Host + img;

		public async Task<decimal?> GetPrice(PhoneInfo info)
		{
			var text = await GetPriceText(info);
			return decimal.TryParse(text, out var price)
				? price
				: null;
		}

		private async Task<string> GetPriceText(PhoneInfo info)
		{
			var findPrice = PriceFinder(info.Store);
			var dom = await GetDocument(info.URL);
			return findPrice(dom, info.PriceSelector);
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