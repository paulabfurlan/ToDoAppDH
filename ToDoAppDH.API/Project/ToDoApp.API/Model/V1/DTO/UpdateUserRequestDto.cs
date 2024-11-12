using System.ComponentModel.DataAnnotations;

namespace ToDoApp.API.Model.V1.DTO
{
	public class UpdateUserRequestDto
	{
		[Required]
		[MinLength(2, ErrorMessage = "Name has to be a minimum of 2 characters")]
		public string Name { get; set; }
		[Required]
		[MinLength(2, ErrorMessage = "LastName has to be a minimum of 2 characters")]
		public string LastName { get; set; }
		[Required]
		[RegularExpression(".+\\@.+\\..+", ErrorMessage = "The e-mail is not valid")]
		public string Email { get; set; }
	}
}
