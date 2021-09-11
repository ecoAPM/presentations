using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using MotoG.Models;

namespace MotoG.Services
{
	public class PriceDisplayService : IPriceDisplayService
	{
		private readonly HttpClient _http;
		private readonly IBrowsingContext _browser;

		public PriceDisplayService(HttpClient http, IBrowsingContext browser)
		{
			_http = http;
			_browser = browser;
		}

		public async Task<string> GetLogoURL(PhoneInfo info)
		{
			var dom = await GetDocument(info);
			var element = dom.QuerySelector(@"link[rel~=""icon""]");
			var logoURL = element?.Attributes["href"]?.Value ?? "/favicon.ico";
			return wrap(info.URL, logoURL);
		}

		private async Task<IDocument> GetDocument(PhoneInfo info)
		{
			var html = await _http.GetStringAsync(info.URL);
			return await _browser.OpenAsync(r => r.Content(html));
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
			var dom = await GetDocument(info);
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
			var capture = match.Groups.Values.Skip(1).First();
			return capture.Value;
		}

		private static string GetHTMLPrice(IParentNode dom, string selector)
			=> dom.QuerySelector(selector)?.TextContent.Trim().Split(" ")[0].Replace("$", "") ?? "";
	}
}