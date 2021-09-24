using System.Threading.Tasks;
using Phones.Models;

namespace Phones.Services
{
	public interface IPriceDisplayService
	{
		string GetImageData(string name);
		Task<PriceViewModel> GetPriceViewModel(PhoneInfo info);
	}
}