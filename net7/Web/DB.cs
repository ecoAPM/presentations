using Microsoft.EntityFrameworkCore;

public class DB : DbContext
{
	public DbSet<WebPerson> WebPeople { get; set; }

	public DB(DbContextOptions<DB> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<WebPerson>()
			.ToTable("WebPeople")
			.HasKey(p => p.ID);

		base.OnModelCreating(modelBuilder);
	}
}