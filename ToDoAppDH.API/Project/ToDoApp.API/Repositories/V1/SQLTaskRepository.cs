using Microsoft.EntityFrameworkCore;
using ToDoApp.API.Data;
using ToDoApp.API.Model.V1.Domain;

namespace ToDoApp.API.Repositories.V1
{
	public class SQLTaskRepository : ITaskRepository
	{
		private readonly ToDoAppDbContext dbContext;

		public SQLTaskRepository(ToDoAppDbContext dbContext)
		{
			this.dbContext = dbContext;
		}

		public async Task<List<TaskTDApp>> GetAllAsync(string? filterOn = null, string? filterQuery = null,
			string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
		{
			var tasks = dbContext.Tasks
								  .Include("User")
								  .AsQueryable();

			// Filtering
			if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
			{
				if (filterOn.Equals("Description", StringComparison.OrdinalIgnoreCase))
				{
					tasks = tasks.Where(x => x.Description.Contains(filterQuery));
				}
				else if (filterOn.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase))
				{
					tasks = tasks.Where(x => x.CreatedAt.Contains(filterQuery));
				}
				else if (filterOn.Equals("FinishedAt", StringComparison.OrdinalIgnoreCase))
				{
					tasks = tasks.Where(x => x.FinishedAt.Contains(filterQuery));
				}
			}

			// Sorting
			if (!string.IsNullOrWhiteSpace(sortBy))
			{
				if (sortBy.Equals("Description", StringComparison.OrdinalIgnoreCase))
				{
					tasks = isAscending ? tasks.OrderBy(x => x.Description) : tasks.OrderByDescending(x => x.Description);
				}
				else if (sortBy.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase))
				{
					tasks = isAscending ? tasks.OrderBy(x => x.CreatedAt) : tasks.OrderByDescending(x => x.CreatedAt);
				}
				else if (sortBy.Equals("FinishedAt", StringComparison.OrdinalIgnoreCase))
				{
					tasks = isAscending ? tasks.OrderBy(x => x.FinishedAt) : tasks.OrderByDescending(x => x.FinishedAt);
				}
			}

			// Pagination
			var skipResults = (pageNumber - 1) * pageSize;

			return await tasks.Skip(skipResults).Take(pageSize).ToListAsync();
		}

		public async Task<TaskTDApp?> GetByIdAsync(Guid id)
		{
			return await dbContext.Tasks
								   .Include("User")
								   .FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task<TaskTDApp> CreateAsync(TaskTDApp task)
		{
			await dbContext.Tasks.AddAsync(task);
			await dbContext.SaveChangesAsync();
			return task;
		}

		public async Task<TaskTDApp?> UpdateAsync(Guid id, TaskTDApp task)
		{
			var existingTask = await dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == id);

			if (existingTask == null)
			{
				return null;
			}

			existingTask.Description = task.Description;
			existingTask.CreatedAt = task.CreatedAt;
			existingTask.FinishedAt = task.FinishedAt;
			existingTask.Completed = task.Completed;
			existingTask.UserId = task.UserId;

			await dbContext.SaveChangesAsync();
			return existingTask;
		}

		public async Task<TaskTDApp?> DeleteAsync(Guid id)
		{
			var existingTask = await dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == id);

			if (existingTask == null)
			{
				return null;
			}

			dbContext.Tasks.Remove(existingTask);
			await dbContext.SaveChangesAsync();
			return existingTask;
		}
	}
}
