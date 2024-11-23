using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
	public class TasksController : ControllerBase
	{
		private readonly ITaskRepository taskRepository;
		private readonly IMapper mapper;
		private readonly ILogger<TasksController> logger;

		public TasksController(ITaskRepository taskRepository, IMapper mapper, ILogger<TasksController> logger)
		{
			this.taskRepository = taskRepository;
			this.mapper = mapper;
			this.logger = logger;
		}

		// GET All Tasks
		[MapToApiVersion("1.0")]
		[HttpGet]
		[Authorize]
		public async Task<IActionResult> GetAllV1([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
			[FromQuery] string? sortBy, [FromQuery] bool? isAscending,
			[FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
		{
			// Get data from Databse - Domain Models
			var tasksDomain = await taskRepository.GetAllAsync(filterOn, filterQuery, sortBy, isAscending ?? true,
				pageNumber, pageSize);

			// Map Domain Models to DTOs
			var tasksDto = mapper.Map<List<TaskDto>>(tasksDomain);

			// Return DTOs
			logger.LogInformation($"GetAllTasks returned: {JsonSerializer.Serialize(tasksDomain)}");
			return Ok(tasksDto);
		}

		// GET Task by ID
		[MapToApiVersion("1.0")]
		[HttpGet]
		[Route("{id:Guid}")]
		[Authorize]
		public async Task<IActionResult> GetByIdV1([FromRoute] Guid id)
		{
			// Get Task Domain Model from Database
			var taskDomain = await taskRepository.GetByIdAsync(id);

			if (taskDomain == null)
			{
				logger.LogError($"GetTaskById didn't find task with id: {id}");
				return NotFound();
			}

			// Map/Convert Task Domain Model to Task DTO
			var taskDto = mapper.Map<TaskDto>(taskDomain);

			// Return DTO back to client
			logger.LogInformation($"GetTaskById returned: {JsonSerializer.Serialize(taskDomain)}");
			return Ok(taskDto);
		}

		// POST To Create new Task
		[MapToApiVersion("1.0")]
		[HttpPost]
		[ValidateModel]
		[Authorize]
		public async Task<IActionResult> CreateV1([FromBody] AddTaskRequestDto addTaskRequestDto)
		{
			// Map or Convert DTO to Domain Model
			var taskDomainModel = mapper.Map<TaskTDApp>(addTaskRequestDto);

			// Use Domain Model to create Task
			taskDomainModel = await taskRepository.CreateAsync(taskDomainModel);

			// Map Domain Model back to DTO
			var taskDto = mapper.Map<TaskDto>(taskDomainModel);

			logger.LogInformation($"CreateTask {JsonSerializer.Serialize(taskDomainModel)} was requested");
			return CreatedAtAction(nameof(GetByIdV1), new { id = taskDto.Id }, taskDto);
		}

		// Update Task
		[MapToApiVersion("1.0")]
		[HttpPut]
		[Route("{id:Guid}")]
		[ValidateModel]
		[Authorize]
		public async Task<IActionResult> UpdateV1([FromRoute] Guid id, [FromBody] UpdateTaskRequestDto updateTaskRequestDto)
		{
			// Map DTO to Domain Model
			var taskDomainModel = mapper.Map<TaskTDApp>(updateTaskRequestDto);

			// Update the task if it exists
			taskDomainModel = await taskRepository.UpdateAsync(id, taskDomainModel);

			if (taskDomainModel == null)
			{
				logger.LogError($"UpdateTask could not update task with id: {id}");
				return NotFound();
			}

			// Convert Domain Model to DTO
			var taskDto = mapper.Map<TaskDto>(taskDomainModel);

			logger.LogInformation($"UpdateTask updated the task informations to {JsonSerializer.Serialize(taskDto)}");
			return Ok(taskDto);
		}

		// Delete Task
		[MapToApiVersion("1.0")]
		[HttpDelete]
		[Route("{id:guid}")]
		[Authorize]
		public async Task<IActionResult> DeleteV1([FromRoute] Guid id)
		{
			// Delete task if it exists
			var taskDomainModel = await taskRepository.DeleteAsync(id);

			if (taskDomainModel == null)
			{
				logger.LogError($"DeleteTask could not delete task with id: {id}");
				return NotFound();
			}

			// Convert Domain Model to DTO
			var taskDto = mapper.Map<TaskDto>(taskDomainModel);

			// Return the deleted Task back
			logger.LogInformation($"DeleteTask deleted the task: {JsonSerializer.Serialize(taskDto)}");
			return Ok(taskDto);
		}
	}
}
