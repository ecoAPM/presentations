using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MotoG.Models;

namespace MotoG.Services
{
	public class PhoneInfoService : IPhoneInfoService
	{
		private readonly IDbConnection _db;

		public PhoneInfoService(IDbConnection db)
			=> _db = db;

		public async Task<IEnumerable<PhoneInfo>> GetAll()
			=> await _db.QueryAsync<PhoneInfo>("SELECT * FROM PhoneInfo");

		public async Task<IEnumerable<PhoneInfo>> GetInfo(string name)
		{
			var phones = await GetAll();
			return phones.Where(p => p.Name == name);
		}
	}
}