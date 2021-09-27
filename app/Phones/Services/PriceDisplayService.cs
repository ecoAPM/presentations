using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Phones.Models;

namespace Phones.Services
{
	public class PriceDisplayService : IPriceDisplayService
	{
		private readonly IHttpClientFactory _http;
		private readonly IBrowsingContext _browser;

		public PriceDisplayService(IHttpClientFactory http, IBrowsingContext browser)
		{
			_http = http;
			_browser = browser;
		}

		public async Task<PriceViewModel> GetPriceViewModel(PhoneInfo info)
		{
			var client = _http.CreateClient();
			var response = await client.GetAsync(info.URL);
			var logoURL = "https://" + new Url(info.URL).Host.Replace("api.", "") + "/favicon.ico";
			var html = await response.Content.ReadAsStringAsync();
			var price = GetPrice(info, html);

			return new PriceViewModel
			{
				Store = info.Store,
				Link = info.URL,
				LogoURL = logoURL,
				Price = await price
			};
		}

		public string GetImageData(string name)
		{
			var contents = File.ReadAllBytes($"{name}.png");
			return Convert.ToBase64String(contents);
		}

		private async Task<decimal?> GetPrice(PhoneInfo info, string html)
		{
			var priceFinder = PriceFinder(info.Store);
			var text = await priceFinder(html, info.PriceSelector);
			return decimal.TryParse(text, out var price)
				? price
				: null;
		}

		private Func<string, string, Task<string>> PriceFinder(string store)
		{
			return store switch
			{
				"BestBuy" => GetRegexPrice,
				"Motorola" => GetRegexPrice,
				"Newegg" => GetRegexPrice,
				_ => GetHTMLPrice
			};
		}

		private static Task<string> GetRegexPrice(string html, string selector)
		{
			var regex = new Regex(selector);
			var match = regex.Match(html);
			var capture = match.Groups.Values.Skip(1).FirstOrDefault();
			return Task.FromResult(capture?.Value);
		}

		private async Task<string> GetHTMLPrice(string html, string selector)
		{
			var dom = await _browser.OpenAsync(r => r.Content(html));
			return dom.QuerySelector(selector)?.TextContent.Trim().Split(" ")[0].Replace("$", "") ?? "";
		}
	}
}