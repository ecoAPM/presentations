namespace Phones.Models
{
	public record PriceViewModel
	{
		public string Store { get; set; }
		public string LogoURL { get; set; }
		public string Link { get; set; }
		public decimal? Price { get; set; }
		public decimal? PercentOfAverage { get; set; }
	}
}