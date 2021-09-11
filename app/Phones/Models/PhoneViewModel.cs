using System.Collections.Generic;

namespace Phones.Models
{
    public record PhoneViewModel
	{
		public string Name { get; set; }
		public string ImageData { get; set; }
		public IEnumerable<PriceViewModel> Prices { get; set; }
	}
}