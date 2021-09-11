using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Phones.Data;
using Phones.Services;
using Xunit;

namespace Phones.Tests
{
	public class PhoneInfoServiceTests : IDisposable
	{
		private readonly IDbConnection _db;

		public PhoneInfoServiceTests()
		{
			_db = new SqliteConnection("Data Source=:memory:");
			_db.Open();
			var initializer = new Initializer(_db);
			initializer.Run().GetAwaiter().GetResult();
		}

		[Fact]
		public async Task CanGetAllPhones()
		{
			//arrange
			var service = new PhoneInfoService(_db);

			//act
			var phones = await service.GetAll();

			//assert
			Assert.NotEmpty(phones);
		}

		[Fact]
		public async Task CanGetPhoneNames()
		{
			//arrange
			var service = new PhoneInfoService(_db);

			//act
			var names = await service.GetNames();

			//assert
			Assert.NotEmpty(names);
		}

		[Fact]
		public async Task CanGetInfoForPhone()
		{
			//arrange
			var service = new PhoneInfoService(_db);

			//act
			var info = await service.GetInfo("Moto G");

			//assert
			var infoArray = info.ToArray();
			Assert.NotEmpty(infoArray);
			Assert.All(infoArray, i => Assert.Equal("Moto G", i.Name));
		}

		public void Dispose()
			=> _db.Dispose();
	}
}