using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using ToDoApp.API.Model.V1.DTO;
using ToDoApp.API.Repositories.V1;

namespace ToDoApp.API.Controllers
{
	[ApiController]
	[ApiVersion("1.0")]
	[Route("api/v{version:apiVersion}/[controller]")]
	public class UsersController : ControllerBase
	{
		private readonly IUserRepository userRepository;
		private readonly IMapper mapper;

		public UsersController(IUserRepository userRepository, IMapper mapper)
        {
			this.userRepository = userRepository;
			this.mapper = mapper;
		}

		// GET All Users
		[MapToApiVersion("1.0")]
		[HttpGet(Name = "GetAllUsersV1")]
		public async Task<IActionResult> GetAllV1([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
			[FromQuery] string? sortBy, [FromQuery] bool? isAscending,
			[FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
		{
			// Get data from Databse - Domain Models
			var usersDomain = await userRepository.GetAllAsync(filterOn, filterQuery, sortBy, isAscending ?? true,
				pageNumber, pageSize);

			// Map Domain Models to DTOs
			var usersDto = mapper.Map<List<UserDto>>(usersDomain);

			// Return DTOs
			return Ok(usersDto);
		}
    }
}
