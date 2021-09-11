using System.Threading.Tasks;
using MotoG.Models;

namespace MotoG.Services
{
	public interface IPriceInfoService
	{
		Task<string> GetLogo(PhoneInfo info);
		Task<decimal?> GetPrice(PhoneInfo info);
	}
}