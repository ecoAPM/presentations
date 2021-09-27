using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Phones.Tests
{
	internal class StubHandler : HttpMessageHandler
	{
		private readonly string _response;

		public StubHandler(string response) => _response = response;

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var msg = new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent(_response)
			};
			return Task.FromResult(msg);
		}
	}
}