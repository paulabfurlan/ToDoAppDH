using Microsoft.EntityFrameworkCore;
using ToDoApp.API.Model.V1.Domain;

namespace ToDoApp.API.Data
{
    public class ToDoAppDbContext: DbContext
	{
        public ToDoAppDbContext(DbContextOptions<ToDoAppDbContext> dbContextOptions) : base(dbContextOptions)
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<TaskTDApp> Tasks { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Seed data for an example user
			var users = new List<User>()
			{
				new User()
				{
					Id = Guid.Parse("b21d8b6d-3242-4c5e-8fa9-c032a60a91a4"),
					Name = "Paula",
					LastName = "Furlan",
					Email = "email@test.com"
				}
			};

			// Seed users to the Database
			modelBuilder.Entity<User>().HasData(users);

			// Seed data for example tasks
			/*var tasks = new List<Task>()
			{
				new Task()
				{
					Id = Guid.Parse("56f32a0b-8221-440e-831a-e88defcd070f"),
					Description = "Wash the gym outfits",
					CreatedAt = "11/07/2024",
					FinishedAt = null,
					Completed = false,
					User = users[0]
				},
				new Task()
				{
					Id = Guid.Parse("e1fe476f-4075-4310-872d-0ccd3774ed75"),
					Description = "Buy food for the week",
					CreatedAt = "11/01/2024",
					FinishedAt = "11/03/2024",
					Completed = true,
					User = users[0]
				},
				new Task()
				{
					Id = Guid.Parse("91916cc8-3a15-4208-861e-f53d2bfdfb58"),
					Description = "Finish the app API",
					CreatedAt = "11/24/2024",
					FinishedAt = null,
					Completed = false,
					User = users[0]
				}
			};

			// Seed tasks to the Database
			modelBuilder.Entity<Task>().HasData(tasks);*/
		}
	}
}
