﻿namespace ToDoApp.API.Model.Domain
{
	public class User
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
	}
}
