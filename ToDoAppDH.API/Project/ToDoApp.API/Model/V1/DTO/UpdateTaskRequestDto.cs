using System.ComponentModel.DataAnnotations;

namespace ToDoApp.API.Model.V1.DTO
{
	public class UpdateTaskRequestDto
	{
		[Required]
		[MaxLength(100, ErrorMessage = "Description has to be a maximum of 100 characters")]
		public string Description { get; set; }

		[Required]
		public string CreatedAt { get; set; }

		public string? FinishedAt { get; set; }

		[Required]
		public bool Completed { get; set; }

		[Required]
		public Guid UserId { get; set; }
	}
}
