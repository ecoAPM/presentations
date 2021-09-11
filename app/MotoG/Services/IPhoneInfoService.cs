using System.Collections.Generic;
using System.Threading.Tasks;
using MotoG.Models;

namespace MotoG.Services
{
	public interface IPhoneInfoService
	{
		Task<IEnumerable<PhoneInfo>> GetAll();
		Task<IEnumerable<PhoneInfo>> GetInfo(string name);
	}
}