using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace MotoG.Data
{
	public class Initializer
	{
		private readonly IDbConnection _db;

		public Initializer(IDbConnection db)
			=> _db = db;

		public async Task Run()
		{
			var alreadyDone = await _db.QuerySingleAsync<bool>($@"SELECT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{"PhoneInfo".ToLowerInvariant()}')");
			if (alreadyDone)
				return;

			var query = new StringBuilder($@"CREATE TABLE PhoneInfo
			(
				ID SMALLINT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
				Name VARCHAR(20),
				Store VARCHAR(20),
				URL VARCHAR(100),
				PriceSelector VARCHAR(100)
			);

			INSERT INTO PhoneInfo (Name, Store, URL, PriceSelector) VALUES
			('Moto G', 'Amazon', 'https://www.amazon.com/dp/B08TMTBK6G', '#priceblock_ourprice')
			, ('Moto G', 'BestBuy', 'https://www.bestbuy.com/site/6441176.p', '[data-track=""adprice-unactivated""] span span')
			, ('Moto G', 'Motorola', 'https://www.motorola.com/us/smartphones-moto-g-play/p?skuId=536', 'property=""product:price:amount"" content=""([\d\.]+)""')
			, ('Moto G', 'NewEgg', 'https://www.newegg.com/p/N82E16875209674', '""FinalPrice"":[ ]?([\d\.]+),')");

			for (var x = 1; x < 100; x++)
			{
				for (var y = 1; y < 10; y++)
				{
					query.AppendLine($@", ('Fake Phone {x}', 'Fake Store {y}', 'http://localhost', '#price')");
				}
			}

			query.AppendLine(";");
			await _db.ExecuteAsync(query.ToString());
		}
	}
}