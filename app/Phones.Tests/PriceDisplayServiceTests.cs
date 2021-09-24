﻿using System;
using AngleSharp;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Phones.Services;
using Xunit;

namespace Phones.Tests
{
	public class PriceDisplayServiceTests
	{
		[Fact]
		public void ImageDataIsBase64Encoded()
		{
			//arrange
			var browser = Substitute.For<IBrowsingContext>();
			var cache = Substitute.For<IMemoryCache>();
			var service = new PriceDisplayService(browser, cache);

			//act
			var imageData = service.GetImageData("Moto G");

			//assert
			Assert.NotEmpty(Convert.FromBase64String(imageData));
		}
	}
}