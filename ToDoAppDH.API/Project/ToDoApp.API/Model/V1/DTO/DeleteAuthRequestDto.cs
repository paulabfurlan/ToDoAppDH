using System.ComponentModel.DataAnnotations;

namespace ToDoApp.API.Model.V1.DTO
{
	public class DeleteAuthRequestDto
	{
		[Required]
		[DataType(DataType.EmailAddress)]
		public string Username { get; set; }
	}
}
