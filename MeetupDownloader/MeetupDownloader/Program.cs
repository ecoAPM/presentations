using System.Text.Json;

var data = await File.ReadAllTextAsync("../../data.json");
var meetups = JsonSerializer.Deserialize<Meetup[]>(data, JsonSerializerOptions.Web)!.OrderBy(m => m.Date).ToArray();
Console.WriteLine(meetups.Length + " meetups");

var topAttendees = meetups
	.SelectMany(m => m.Who)
	.GroupBy(n => n)
	.OrderByDescending(n => n.Count())
	.Take(25)
	.ToArray();

foreach (var a in topAttendees)
	Console.WriteLine($"{a.Key} \t {a.Count()}");

var myFirst = meetups
	.First(m => m.Who.Contains("Steve Desmond"));

Console.WriteLine($"Steve's first: {myFirst.Date}");

var locations = meetups
	.GroupBy(m => m.Location)
	.OrderBy(l => l.Min(m => m.Date))
	.ToArray();

foreach (var l in locations)
	Console.WriteLine($"{l.Key} \t {l.Count()} ({l.Min(m => m.Date)} - {l.Max(m => m.Date)})");

var mostLocations = locations
	.OrderByDescending(l => l.Count())
	.Take(3)
	.ToArray();

foreach (var l in mostLocations)
	Console.WriteLine($"{l.Key} \t {l.Count()}");

var longestLocations = locations
	.OrderByDescending(l => l.Max(m => m.Date) - l.Min(m => m.Date))
	.Take(3)
	.ToArray();

foreach (var l in longestLocations)
	Console.WriteLine($"{l.Key} \t {(l.Max(m => m.Date) - l.Min(m => m.Date)).TotalDays/365:G2}y");

var attendance = meetups
	.ToDictionary(m => m.Date, m => m.Attendees);

foreach (var m in attendance)
	Console.WriteLine($"{m.Key} \t {m.Value}");

internal record struct Meetup(DateTime Date, string Name, string Location, int Attendees, string[] Who);