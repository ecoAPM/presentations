using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Text;
using NSubstitute;
using Phones.Models;
using Phones.Services;
using Xunit;
using IDocument = AngleSharp.Dom.IDocument;

namespace Phones.Tests
{
	public class PriceDisplayServiceTests
	{
		[Fact]
		public async Task ImageDataIsBase64Encoded()
		{
			//arrange
			var http = Substitute.For<IHttpClientFactory>();
			var browser = Substitute.For<IBrowsingContext>();
			var service = new PriceDisplayService(http, browser);

			//act
			var imageData = await service.GetImageData("Moto G");

			//assert
			Assert.NotEmpty(Convert.FromBase64String(imageData));
		}

		[Fact]
		public async Task CanGetDefaultLogoURL()
		{
			//arrange
			var handler = new StubHandler("");

			var http = Substitute.For<IHttpClientFactory>();
			http.CreateClient().Returns(new HttpClient(handler));

			var browser = Substitute.For<IBrowsingContext>();

			var factory = Substitute.For<IDocumentFactory>();
			browser.GetServices<IDocumentFactory>().Returns(new [] { factory });

			var service = new PriceDisplayService(http, browser);

			var phone = new PhoneInfo
			{
				URL = "http://localhost"
			};

			//act
			var vm = await service.GetPriceViewModel(phone);

			//assert
			Assert.Equal("https://localhost/favicon.ico", vm.LogoURL);
		}

		[Fact]
		public async Task CanGetPriceViaRegex()
		{
			//arrange
			var handler = new StubHandler($@"{{""Price"":""$123.45""}}");

			var http = Substitute.For<IHttpClientFactory>();
			http.CreateClient().Returns(new HttpClient(handler));

			var browser = Substitute.For<IBrowsingContext>();

			var factory = Substitute.For<IDocumentFactory>();
			browser.GetServices<IDocumentFactory>().Returns(new [] { factory });

			var service = new PriceDisplayService(http, browser);

			var phone = new PhoneInfo
			{
				Store = "Motorola",
				URL = "http://localhost",
				PriceSelector = $@"{{""Price"":""\$([\d\.]+)""}}"
			};

			//act
			var vm = await service.GetPriceViewModel(phone);

			//assert
			Assert.Equal(123.45m, vm.Price);
		}

		[Fact]
		public async Task CanGetPriceViaDOM()
		{
			//arrange
			var handler = new StubHandler("");

			var http = Substitute.For<IHttpClientFactory>();
			http.CreateClient().Returns(new HttpClient(handler));

			var browser = Substitute.For<IBrowsingContext>();

			var element = new HtmlElement(Substitute.For<Document>(browser, new TextSource("")), "price")
			{
				TextContent = "$123.45"
			};

			var dom = Substitute.For<IDocument>();
			dom.QuerySelector(Arg.Any<string>()).Returns(element);

			var factory = Substitute.For<IDocumentFactory>();
			factory.CreateAsync(Arg.Any<IBrowsingContext>(), Arg.Any<CreateDocumentOptions>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(dom));
			browser.GetServices<IDocumentFactory>().Returns(new [] { factory });

			var service = new PriceDisplayService(http, browser);

			var phone = new PhoneInfo
			{
				Store = "X",
				URL = "http://localhost",
				PriceSelector = "price.tag"
			};

			//act
			var vm = await service.GetPriceViewModel(phone);

			//assert
			Assert.Equal(123.45m, vm.Price);
		}
	}
}