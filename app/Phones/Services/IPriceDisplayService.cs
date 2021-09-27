using System.Threading.Tasks;
using Phones.Models;

namespace Phones.Services
{
	public interface IPriceDisplayService
	{
		Task<string> GetImageData(string name);
		Task<PriceViewModel> GetPriceViewModel(PhoneInfo info);
	}
}