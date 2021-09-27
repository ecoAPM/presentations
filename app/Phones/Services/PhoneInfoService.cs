using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Phones.Models;

namespace Phones.Services
{
	public class PhoneInfoService : IPhoneInfoService
	{
		private readonly IDbConnection _db;
		private readonly IMemoryCache _cache;

		public PhoneInfoService(IDbConnection db, IMemoryCache cache)
		{
			_db = db;
			_cache = cache;
		}

		public async Task<IEnumerable<PhoneInfo>> GetAll()
			=> await _cache.GetOrCreateAsync("All",
				async entry =>
				{
					entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
					return await _db.QueryAsync<PhoneInfo>("SELECT * FROM PhoneInfo");
				});

		public async Task<IEnumerable<PhoneInfo>> GetInfo(string name)
			=> await _cache.GetOrCreateAsync(name,
				async entry =>
				{
					entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
					return await _db.QueryAsync<PhoneInfo>("SELECT * FROM PhoneInfo WHERE Name = @Name", new { Name = name });
				});

		public async Task<IEnumerable<string>> GetNames()
			=> await _cache.GetOrCreateAsync("Names",
				async entry =>
				{
					entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
					return await _db.QueryAsync<string>("SELECT DISTINCT Name, LENGTH(Name) FROM PhoneInfo ORDER BY LENGTH(Name), Name");
				});
	}
}