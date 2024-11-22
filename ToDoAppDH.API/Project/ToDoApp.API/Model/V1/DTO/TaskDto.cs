namespace ToDoApp.API.Model.V1.DTO
{
	public class TaskDto
	{
		public Guid Id { get; set; }
		public string Description { get; set; }
		public string CreatedAt { get; set; }
		public string? FinishedAt { get; set; }
		public bool Completed { get; set; }

		// Navigation properties
		public UserDto User { get; set; }
	}
}
