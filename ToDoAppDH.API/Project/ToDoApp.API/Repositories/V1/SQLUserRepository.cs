using Microsoft.EntityFrameworkCore;
using ToDoApp.API.Data;
using ToDoApp.API.Model.V1.Domain;

namespace ToDoApp.API.Repositories.V1
{
	public class SQLUserRepository : IUserRepository
	{
		private readonly ToDoAppDbContext dbContext;

		public SQLUserRepository(ToDoAppDbContext dbContext)
        {
			this.dbContext = dbContext;
		}

		public async Task<List<User>> GetAllAsync(string? filterOn = null, string? filterQuery = null,
			string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
		{
			var users = dbContext.Users.AsQueryable();

			// Filtering
			if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
			{
				if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
				{
					users = users.Where(x => x.Name.Contains(filterQuery));
				}
				else if (filterOn.Equals("LastName", StringComparison.OrdinalIgnoreCase))
				{
					users = users.Where(x => x.LastName.Contains(filterQuery));
				}
				else if (filterOn.Equals("Email", StringComparison.OrdinalIgnoreCase))
				{
					users = users.Where(x => x.Email.Contains(filterQuery));
				}
			}

			// Sorting
			if (!string.IsNullOrWhiteSpace(sortBy))
			{
				if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
				{
					users = isAscending ? users.OrderBy(x => x.Name) : users.OrderByDescending(x => x.Name);
				}
				else if (sortBy.Equals("LastName", StringComparison.OrdinalIgnoreCase))
				{
					users = isAscending ? users.OrderBy(x => x.LastName) : users.OrderByDescending(x => x.LastName);
				}
				else if (sortBy.Equals("Email", StringComparison.OrdinalIgnoreCase))
				{
					users = isAscending ? users.OrderBy(x => x.Email) : users.OrderByDescending(x => x.Email);
				}
			}

			// Pagination
			var skipResults = (pageNumber - 1) * pageSize;

			return await users.Skip(skipResults).Take(pageSize).ToListAsync();
		}

		public async Task<User?> GetByIdAsync(Guid id)
		{
			return await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task<User> CreateAsync(User user)
		{
			await dbContext.Users.AddAsync(user);
			await dbContext.SaveChangesAsync();
			return user;
		}

		public async Task<User?> UpdateAsync(Guid id, User user)
		{
			var existingUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);

			if (existingUser == null)
			{
				return null;
			}

			existingUser.Name = user.Name;
			existingUser.LastName = user.LastName;
			existingUser.Email = user.Email;

			await dbContext.SaveChangesAsync();
			return existingUser;
		}

		public async Task<User?> DeleteAsync(Guid id)
		{
			var existingUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);

			if (existingUser == null)
			{
				return null;
			}

			dbContext.Users.Remove(existingUser);
			await dbContext.SaveChangesAsync();
			return existingUser;
		}
	}
}
