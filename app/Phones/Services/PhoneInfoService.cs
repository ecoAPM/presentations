using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Phones.Models;

namespace Phones.Services
{
	public class PhoneInfoService : IPhoneInfoService
	{
		private readonly IDbConnection _db;

		public PhoneInfoService(IDbConnection db)
			=> _db = db;

		public async Task<IEnumerable<PhoneInfo>> GetAll()
			=> await _db.QueryAsync<PhoneInfo>("SELECT * FROM PhoneInfo");

		public async Task<IEnumerable<PhoneInfo>> GetInfo(string name)
			=> await _db.QueryAsync<PhoneInfo>("SELECT * FROM PhoneInfo WHERE Name = @Name", new { Name = name });

		public async Task<IEnumerable<string>> GetNames()
			=> await _db.QueryAsync<string>("SELECT DISTINCT Name, LENGTH(Name) FROM PhoneInfo ORDER BY LENGTH(Name), Name");
	}
}