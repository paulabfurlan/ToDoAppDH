using ToDoApp.API.Model.V1.Domain;

namespace ToDoApp.API.Repositories.V1
{
	public interface ITaskRepository
	{
		Task<List<TaskTDApp>> GetAllAsync(string? filterOn = null, string? filterQuery = null,
	string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000);
		Task<TaskTDApp?> GetByIdAsync(Guid id);
		Task<TaskTDApp> CreateAsync(TaskTDApp task);
		Task<TaskTDApp?> UpdateAsync(Guid id, TaskTDApp task);
		Task<TaskTDApp?> DeleteAsync(Guid id);
	}
}
