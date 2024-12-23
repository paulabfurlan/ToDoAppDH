﻿using static System.Runtime.InteropServices.JavaScript.JSType;
using ToDoApp.API.Model.V1.DTO;
using AutoMapper;
using ToDoApp.API.Model.V1.Domain;

namespace ToDoApp.API.Mappings.V1
{
	public class AutoMapperProfiles : Profile
	{
		public AutoMapperProfiles()
		{
			CreateMap<User, UserDto>().ReverseMap();
			CreateMap<User, AddUserRequestDto>().ReverseMap();
			CreateMap<User, UpdateUserRequestDto>().ReverseMap();
			CreateMap<TaskTDApp, TaskDto>().ReverseMap();
			CreateMap<TaskTDApp, AddTaskRequestDto>().ReverseMap();
			CreateMap<TaskTDApp, UpdateTaskRequestDto>().ReverseMap();
		}
	}
}
