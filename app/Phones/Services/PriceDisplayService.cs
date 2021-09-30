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
			var dom = await GetDocument(info.URL);
			var logoURL = GetLogoURL(dom, info);
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

		private static string GetLogoURL(IDocument dom, PhoneInfo info)
		{
			var element = dom.QuerySelector(@"link[rel~=""icon""]");
			var logoURL = element?.Attributes["href"]?.Value ?? "/favicon.ico";
			return wrap(info.URL, logoURL);
		}

		private async Task<IDocument> GetDocument(string url)
		{
			return await _browser.OpenAsync(url);
		}

		private static string wrap(string page, string img) => img.Contains("//") ? img : "https://" + new Url(page).Host + img;

		private static decimal? GetPrice(IDocument dom, PhoneInfo info)
		{
			var text = GetPriceText(dom, info);
			return decimal.TryParse(text, out var price)
				? price
				: null;
		}

		private static string GetPriceText(IDocument dom, PhoneInfo info)
		{
			var findPrice = PriceFinder(info.Store);
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