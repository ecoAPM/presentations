using System.Data;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Phones.Data
{
	public class Initializer
	{
		private readonly IDbConnection _db;

		public Initializer(IDbConnection db)
			=> _db = db;

		public async Task Run()
		{
			var alreadyDone = await _db.QuerySingleAsync<bool>($@"SELECT EXISTS ({TableQuery(_db)})");
			if (alreadyDone)
				return;

			var query = new StringBuilder($@"CREATE TABLE PhoneInfo
			(
				ID SMALLINT PRIMARY KEY {Identity(_db)},
				Name VARCHAR(20),
				Store VARCHAR(20),
				URL VARCHAR(200),
				PriceSelector VARCHAR(200)
			);

			INSERT INTO PhoneInfo (Name, Store, URL, PriceSelector) VALUES
			('Moto G', 'Amazon', 'https://www.amazon.com/dp/B08NWGBHG3', '#priceblock_ourprice')
			, ('Moto G', 'BestBuy', 'https://www.bestbuy.com/site/motorola-moto-g-play-2021-32gb-memory-unlocked-misty-blue/6441176.p?skuId=6441176', '\\""currentPrice\\"":[ ]?([\d\.]+),')
			, ('Moto G', 'Newegg', 'https://www.newegg.com/p/N82E16875209674', '""FinalPrice"":[ ]?([\d\.]+),')
			, ('Moto G', 'Motorola', 'https://www.motorola.com/us/smartphones-moto-g-play/p?skuId=536', 'property=""product:price:amount"" content=""([\d\.]+)""')
			, ('Nokia 5.4', 'Amazon', 'https://www.amazon.com/dp/B08STXNT4K', '#priceblock_ourprice')
			, ('Nokia 5.4', 'BestBuy', 'https://www.bestbuy.com/site/nokia-5-4-128gb-unlocked-polar-night/6453856.p?skuId=6453856', '\\""currentPrice\\"":[ ]?([\d\.]+),')
			, ('Nokia 5.4', 'Newegg', 'https://www.newegg.com/nokia-5-4-6-39-4g-lte-polar-night/p/N82E16875205602', '""FinalPrice"":[ ]?([\d\.]+),')
			, ('Nokia 5.4', 'Nokia', 'https://www.nokia.com/phones/en_us/nokia-5-4?sku=HQ5020M515000', 'div.price.lg')
			");

			for (var x = 10; x <= 100; x++)
			{
				for (var y = 1; y < 10; y++)
				{
					query.AppendLine($@", ('jkPhone {x}', 'Fake Store {y}', 'http://localhost', '#price')");
				}
			}

			query.AppendLine(";");
			await _db.ExecuteAsync(query.ToString());
		}

		private static string TableQuery(IDbConnection db)
			=> db is SqliteConnection
				? $@"SELECT * FROM sqlite_master WHERE type='table' AND name = '{"PhoneInfo".ToLowerInvariant()}'"
				: $@"SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{"PhoneInfo".ToLowerInvariant()}'";

		private static string Identity(IDbConnection db)
			=> db is SqliteConnection
				? ""
				: "GENERATED ALWAYS AS IDENTITY";
	}
}