namespace MotoG.Models
{
	public record PriceDisplay
	{
		public PhoneInfo PhoneInfo { get; set; }
		public string Logo { get; set; }
		public decimal? Price { get; set; }
	}
}