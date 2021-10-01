using System.Threading.Tasks;
using Phones.Models;

namespace Phones.Services
{
	public interface IPriceDisplayService
	{
		Task<PriceViewModel> GetPriceViewModel(PhoneInfo info);
	}
}