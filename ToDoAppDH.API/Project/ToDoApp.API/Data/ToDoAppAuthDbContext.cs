using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ToDoApp.API.Data
{
	public class ToDoAppAuthDbContext : IdentityDbContext
	{
		public ToDoAppAuthDbContext(DbContextOptions<ToDoAppAuthDbContext> options) : base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			var readerRoleId = "2caa5bda-8d59-484f-904d-76af4da108c3";
			var writerRoleId = "78678386-95ec-49c6-b158-c91bdddd22ca";

			var roles = new List<IdentityRole>
			{
				new IdentityRole
				{
					Id = readerRoleId,
					ConcurrencyStamp = readerRoleId,
					Name = "Reader",
					NormalizedName = "Reader".ToUpper()
				},
				new IdentityRole
				{
					Id = writerRoleId,
					ConcurrencyStamp = writerRoleId,
					Name = "Writer",
					NormalizedName = "Writer".ToUpper()
				}
			};

			builder.Entity<IdentityRole>().HasData(roles);
		}
	}
}
