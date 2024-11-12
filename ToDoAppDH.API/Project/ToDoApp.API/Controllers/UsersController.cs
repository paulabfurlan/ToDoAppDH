using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using ToDoApp.API.CustomActionFilters;
using ToDoApp.API.Model.V1.Domain;
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
		private readonly ILogger<UsersController> logger;

		public UsersController(IUserRepository userRepository, IMapper mapper, ILogger<UsersController> logger)
        {
			this.userRepository = userRepository;
			this.mapper = mapper;
			this.logger = logger;
		}

		// GET All Users
		[MapToApiVersion("1.0")]
		[HttpGet]
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
			logger.LogInformation($"GetAllUsers returned: {JsonSerializer.Serialize(usersDomain)}");
			return Ok(usersDto);
		}

		// GET User by ID
		[MapToApiVersion("1.0")]
		[HttpGet]
		[Route("{id:Guid}")]
		public async Task<IActionResult> GetByIdV1([FromRoute] Guid id)
		{
			// Get User Domain Model from Database
			var userDomain = await userRepository.GetByIdAsync(id);

			if (userDomain == null)
			{
				logger.LogError($"GetUserById didn't find user with id: {id}");
				return NotFound();
			}

			// Map/Convert Region Domain Model to Region DTO
			var userDto = mapper.Map<UserDto>(userDomain);

			// Return DTO back to client
			logger.LogInformation($"GetUserById returned: {JsonSerializer.Serialize(userDomain)}");
			return Ok(userDto);
		}

		// POST To Create new User
		[HttpPost]
		[ValidateModel]
		public async Task<IActionResult> CreateV1([FromBody] AddUserRequestDto addUserRequestDto)
		{
			// Map or Convert DTO to Domain Model
			var userDomainModel = mapper.Map<User>(addUserRequestDto);

			// Use Domain Model to create User
			userDomainModel = await userRepository.CreateAsync(userDomainModel);

			// Map Domain Model back to DTO
			var userDto = mapper.Map<UserDto>(userDomainModel);

			logger.LogInformation($"CreateUser {JsonSerializer.Serialize(userDomainModel)} was requested");
			return CreatedAtAction(nameof(GetByIdV1), new { id = userDto.Id }, userDto);
		}

		// Update User
		[HttpPut]
		[Route("{id:Guid}")]
		[ValidateModel]
		public async Task<IActionResult> UpdateV1([FromRoute] Guid id, [FromBody] UpdateUserRequestDto updateUserRequestDto)
		{
			// Map DTO to Domain Model
			var userDomainModel = mapper.Map<User>(updateUserRequestDto);

			// Update the user if it exists
			userDomainModel = await userRepository.UpdateAsync(id, userDomainModel);

			if (userDomainModel == null)
			{
				logger.LogError($"UpdateUser could not update user with id: {id}");
				return NotFound();
			}

			// Convert Domain Model to DTO
			var userDto = mapper.Map<UserDto>(userDomainModel);

			logger.LogInformation($"UpdateUser updated the user informations to {JsonSerializer.Serialize(userDto)}");
			return Ok(userDto);
		}

		// Delete User
		[HttpDelete]
		[Route("{id:guid}")]
		public async Task<IActionResult> DeleteV1([FromRoute] Guid id)
		{
			// Delete user if it exists
			var userDomainModel = await userRepository.DeleteAsync(id);

			if (userDomainModel == null)
			{
				logger.LogError($"DeleteUser could not delete user with id: {id}");
				return NotFound();
			}

			// Convert Domain Model to DTO
			var userDto = mapper.Map<UserDto>(userDomainModel);

			// Return the deleted User back
			logger.LogInformation($"DeleteUser deleted the user: {JsonSerializer.Serialize(userDto)}");
			return Ok(userDto);
		}
	}
}
