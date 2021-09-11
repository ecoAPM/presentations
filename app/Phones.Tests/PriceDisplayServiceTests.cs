using System;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Browser;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Io;
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
		public void ImageDataIsBase64Encoded()
		{
			//arrange
			var browser = Substitute.For<IBrowsingContext>();
			var service = new PriceDisplayService(browser);

			//act
			var imageData = service.GetImageData("Moto G");

			//assert
			Assert.NotEmpty(Convert.FromBase64String(imageData));
		}

		[Fact]
		public async Task CanGetLogoURLFromHTML()
		{
			//arrange
			var browser = Substitute.For<IBrowsingContext>();

			var link = new HtmlElement(Substitute.For<Document>(browser, new TextSource("")), "link");
			link.AddAttribute(new Attr("href", "/test.ico"));

			var dom = Substitute.For<IDocument>();
			dom.QuerySelector(Arg.Any<string>()).Returns(link);

			var handler = Substitute.For<INavigationHandler>();
			handler.SupportsProtocol(Arg.Any<string>()).Returns(true);
			handler.NavigateAsync(Arg.Any<DocumentRequest>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(dom));

			browser.GetServices<INavigationHandler>().Returns(new[] { handler });

			var service = new PriceDisplayService(browser);

			//act
			var logoURL = await service.GetLogoURL("http://localhost");

			//assert
			Assert.Equal("https://localhost/test.ico", logoURL);
		}

		[Fact]
		public async Task CanGetDefaultLogoURL()
		{
			//arrange
			var browser = Substitute.For<IBrowsingContext>();

			var dom = Substitute.For<IDocument>();
			dom.QuerySelector(Arg.Any<string>()).Returns(null as IElement);

			var handler = Substitute.For<INavigationHandler>();
			handler.SupportsProtocol(Arg.Any<string>()).Returns(true);
			handler.NavigateAsync(Arg.Any<DocumentRequest>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(dom));

			browser.GetServices<INavigationHandler>().Returns(new[] { handler });

			var service = new PriceDisplayService(browser);

			//act
			var logoURL = await service.GetLogoURL("http://localhost");

			//assert
			Assert.Equal("https://localhost/favicon.ico", logoURL);
		}

		[Fact]
		public async Task CanGetPriceViaRegex()
		{
			//arrange
			var browser = Substitute.For<IBrowsingContext>();

			var dom = Substitute.For<IDocument>();
			dom.Source.Returns(new TextSource($@"{{""Price"":""$123.45""}}"));

			var handler = Substitute.For<INavigationHandler>();
			handler.SupportsProtocol(Arg.Any<string>()).Returns(true);
			handler.NavigateAsync(Arg.Any<DocumentRequest>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(dom));

			browser.GetServices<INavigationHandler>().Returns(new[] { handler });

			var service = new PriceDisplayService(browser);

			var phone = new PhoneInfo
			{
				Store = "Motorola",
				URL = "http://localhost",
				PriceSelector = $@"{{""Price"":""\$([\d\.]+)""}}"
			};

			//act
			var price = await service.GetPrice(phone);

			//assert
			Assert.Equal(123.45m, price);
		}

		[Fact]
		public async Task CanGetPriceViaDOM()
		{
			//arrange
			var browser = Substitute.For<IBrowsingContext>();

			var element = new HtmlElement(Substitute.For<Document>(browser, new TextSource("")), "price")
			{
				TextContent = "$123.45"
			};

			var dom = Substitute.For<IDocument>();
			dom.QuerySelector(Arg.Any<string>()).Returns(element);

			var handler = Substitute.For<INavigationHandler>();
			handler.SupportsProtocol(Arg.Any<string>()).Returns(true);
			handler.NavigateAsync(Arg.Any<DocumentRequest>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(dom));

			browser.GetServices<INavigationHandler>().Returns(new[] { handler });

			var service = new PriceDisplayService(browser);

			var phone = new PhoneInfo
			{
				Store = "X",
				URL = "http://localhost",
				PriceSelector = "price.tag"
			};

			//act
			var price = await service.GetPrice(phone);

			//assert
			Assert.Equal(123.45m, price);
		}
	}
}