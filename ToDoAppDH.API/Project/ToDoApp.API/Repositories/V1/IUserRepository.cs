﻿using ToDoApp.API.Model.V1.Domain;

namespace ToDoApp.API.Repositories.V1
{
	public interface IUserRepository
	{
		Task<List<User>> GetAllAsync(string? filterOn = null, string? filterQuery = null,
			string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000);
		Task<User?> GetByIdAsync(Guid id);
		Task<User> CreateAsync(User user);
		Task<User?> UpdateAsync(Guid id, User user);
		Task<User?> DeleteAsync(Guid id);
	}
}
