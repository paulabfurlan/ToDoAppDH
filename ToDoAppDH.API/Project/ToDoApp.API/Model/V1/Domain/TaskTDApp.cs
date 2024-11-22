namespace ToDoApp.API.Model.V1.Domain
{
    public class TaskTDApp
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string CreatedAt { get; set; }
        public string? FinishedAt { get; set; }
        public bool Completed { get; set; }
        public Guid UserId { get; set; }

        // Navigation properties
        public User User { get; set; }
    }
}
