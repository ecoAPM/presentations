﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using Microsoft.Extensions.Caching.Memory;
using Phones.Models;

namespace Phones.Services
{
	public class PriceDisplayService : IPriceDisplayService
	{
		private readonly HttpClient _http;
		private readonly IBrowsingContext _browser;
		private readonly IMemoryCache _cache;

		public PriceDisplayService(IHttpClientFactory http, IBrowsingContext browser, IMemoryCache cache)
		{
			_http = http.CreateClient();
			_browser = browser;
			_cache = cache;
		}

		public async Task<PriceViewModel> GetPriceViewModel(PhoneInfo info)
			=> await _cache.GetOrCreateAsync(info.Name + info.Store,
				async entry =>
				{
					entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);

					var response = _http.GetAsync(info.URL);
					var logoURL = "https://" + new Url(info.URL).Host.Replace("api.", "") + "/favicon.ico";
					var price = GetPrice(await response, info);

					return new PriceViewModel
					{
						Store = info.Store,
						Link = info.URL,
						LogoURL = logoURL,
						Price = await price
					};
				});

		private readonly IDictionary<string, string> _images = new ConcurrentDictionary<string, string>();

		public async Task<string> GetImageData(string name)
		{
			if (!_images.ContainsKey(name))
			{
				var contents = await File.ReadAllBytesAsync($"{name}.png");
				var base64 = Convert.ToBase64String(contents);
				_images.Add(name, base64);
			}

			return _images[name];
		}

		private async Task<decimal?> GetPrice(HttpResponseMessage response, PhoneInfo info)
		{
			var priceFinder = PriceFinder(info.Store);
			var text = await priceFinder(response, info.PriceSelector);
			return decimal.TryParse(text, out var price)
				? price
				: null;
		}

		private Func<HttpResponseMessage, string, Task<string>> PriceFinder(string store)
		{
			return store switch
			{
				"BestBuy" => GetRegexPrice,
				"Motorola" => GetRegexPrice,
				"Newegg" => GetRegexPrice,
				_ => GetHTMLPrice
			};
		}

		private static async Task<string> GetRegexPrice(HttpResponseMessage response, string selector)
		{
			var html = response.Content.ReadAsStringAsync();
			var regex = new Regex(selector);
			var match = regex.Match(await html);
			var capture = match.Groups.Values.Skip(1).FirstOrDefault();
			return capture?.Value;
		}

		private async Task<string> GetHTMLPrice(HttpResponseMessage response, string selector)
		{
			async void Request(VirtualResponse r) => r.Content(await response.Content.ReadAsStreamAsync());
			var dom = await _browser.OpenAsync(Request);
			return dom.QuerySelector(selector)?.TextContent.Trim().Split(" ")[0].Replace("$", "") ?? "";
	}
	}
}