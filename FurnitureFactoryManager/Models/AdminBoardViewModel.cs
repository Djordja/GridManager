namespace FurnitureFactoryManager.Models;

public class AdminBoardViewModel
{
    public IReadOnlyList<Project> Projects { get; set; } = Array.Empty<Project>();
    public int SelectedProjectId { get; set; }
    public IReadOnlyCollection<ProjectTask> Tasks { get; set; } = Array.Empty<ProjectTask>();
    public NewTaskInput NewTask { get; set; } = new();

    public class NewTaskInput
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string AssignedTo { get; set; } = string.Empty;

        public TaskStatus Status { get; set; } = TaskStatus.ToDo;
        public int ProjectId { get; set; }
    }
}
