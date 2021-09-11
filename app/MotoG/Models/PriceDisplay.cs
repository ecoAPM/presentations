namespace MotoG.Models
{
	public record PriceDisplay
	{
		public PhoneInfo PhoneInfo { get; set; }
		public string LogoURL { get; set; }
		public decimal? Price { get; set; }
		public decimal? PercentOfAverage { get; set; }
	}
}