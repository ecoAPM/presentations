using System.Threading.Tasks;
using MotoG.Models;

namespace MotoG.Services
{
	public interface IPriceDisplayService
	{
		Task<string> GetLogoURL(PhoneInfo info);
		Task<decimal?> GetPrice(PhoneInfo info);
	}
}