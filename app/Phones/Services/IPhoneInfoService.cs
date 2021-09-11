using System.Collections.Generic;
using System.Threading.Tasks;
using Phones.Models;

namespace Phones.Services
{
	public interface IPhoneInfoService
	{
		Task<IEnumerable<PhoneInfo>> GetAll();
		Task<IEnumerable<PhoneInfo>> GetInfo(string name);
	}
}