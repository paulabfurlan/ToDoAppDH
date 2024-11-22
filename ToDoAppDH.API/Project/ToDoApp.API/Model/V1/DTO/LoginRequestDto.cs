using System.ComponentModel.DataAnnotations;

namespace ToDoApp.API.Model.V1.DTO
{
    public class LoginRequestDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public double Expiration { get; set; }
    }
}
