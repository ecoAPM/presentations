public class WebPeepController
{
	private readonly DB _db;

	public WebPeepController(DB db)
		=> _db = db;

	public WebPerson[] List()
		=> _db.WebPeople.ToArray();

	public WebPerson Info(int id)
		=> _db.WebPeople.Find(id)!;

	public WebPerson Add(WebPerson data)
	{
		_db.WebPeople.Add(data);
		_db.SaveChanges();
		return data;
	}

	public WebPerson[] WithFav(string lang)
	{
		var peeps = _db.WebPeople.Where(p => p.FavLang == lang);
	}

	public string SayHello(int id)
	{
		var person = _db.WebPeople.Find(id);
		if (person is null)
			throw new NullReferenceException("person not found!");

		return $"Hello, {person.Name}!";
	}
}