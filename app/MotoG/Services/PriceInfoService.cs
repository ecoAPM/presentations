using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using MotoG.Models;

namespace MotoG.Services
{
	public class PriceInfoService : IPriceInfoService
	{
		private readonly IBrowsingContext _browser;

		public PriceInfoService(IBrowsingContext browser) => _browser = browser;

		public async Task<string> GetLogo(PhoneInfo info)
		{
			var dom = await _browser.OpenAsync(info.URL);
			var element = dom.QuerySelector(@"link[rel~=""icon""]");
			var logoURL = element?.Attributes["href"]?.Value ?? defaultIconURL(info.URL);
			var logo = await _browser.OpenAsync(logoURL);
			if (logo is null)
				return null;

			var bytes = Encoding.Default.GetBytes(logo.Source.Text);
			return Convert.ToBase64String(bytes);
		}

		private static string defaultIconURL(string url) => "https://" + new Url(url).Host + "/favicon.ico";

		public async Task<decimal?> GetPrice(PhoneInfo info)
		{
			var text = await GetPriceText(info);
			return decimal.TryParse(text, out var price)
				? price
				: null;
		}

		private async Task<string> GetPriceText(PhoneInfo info)
		{
			var dom = await _browser.OpenAsync(info.URL);
			return info.Store switch
			{
				"Motorola" => GetRegexPrice(dom, info.PriceSelector),
				"NewEgg" => GetRegexPrice(dom, info.PriceSelector),
				_ => GetHTMLPrice(dom, info.PriceSelector)
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
			=> dom.QuerySelector(selector)?.TextContent.Split(" ")[0].Replace("$", "") ?? "";
	}
}