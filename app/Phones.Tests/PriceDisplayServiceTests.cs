using System;
using AngleSharp;
using Microsoft.Extensions.Caching.Memory;
using Phones.Services;
using Rocks;
using Xunit;

namespace Phones.Tests
{
	public class PriceDisplayServiceTests
	{
		[Fact]
		public void ImageDataIsBase64Encoded()
		{
			//arrange
			var browser = Rock.Create<IBrowsingContext>();
			var cache = Rock.Create<IMemoryCache>();
			var service = new PriceDisplayService(browser.Instance(), cache.Instance());

			//act
			var imageData = service.GetImageData("Moto G");

			//assert
			Assert.NotEmpty(Convert.FromBase64String(imageData));
		}
	}
}