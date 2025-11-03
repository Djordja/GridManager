namespace FurnitureFactoryManager.Models;

public class ProjectTask
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AssignedTo { get; set; } = string.Empty;
    public TaskStatus Status { get; set; } = TaskStatus.ToDo;
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}
