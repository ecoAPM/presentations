namespace Phones.Models
{
	public record PhoneInfo
	{
		public string Name { get; set; }
		public string Store { get; set; }
		public string URL { get; set; }
		public string PriceSelector { get; set; }
	}
}