using FurnitureFactoryManager.Models;

namespace FurnitureFactoryManager.Services;

public class ProjectStore
{
    private readonly List<Project> _projects = new();
    private readonly List<ProjectTask> _tasks = new();
    private int _nextTaskId = 1;
    private int _nextProjectId = 1;
    private readonly object _lock = new();

    public ProjectStore()
    {
        Seed();
    }

    public IReadOnlyList<Project> GetAllProjects()
    {
        lock (_lock)
        {
            return _projects
                .Select(project => new Project
                {
                    Id = project.Id,
                    Name = project.Name,
                    Description = project.Description
                })
                .ToList();
        }
    }

    public IReadOnlyCollection<ProjectTask> GetTasksByProject(int projectId)
    {
        lock (_lock)
        {
            return _tasks
                .Where(task => task.ProjectId == projectId)
                .Select(task => Clone(task))
                .ToList();
        }
    }

    public ProjectTask? UpdateStatus(int id, TaskStatus status)
    {
        lock (_lock)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return null;
            }

            task.Status = status;
            return Clone(task);
        }
    }

    public ProjectTask? AddTask(string title, string description, string assignedTo, TaskStatus status, int projectId)
    {
        lock (_lock)
        {
            var project = _projects.FirstOrDefault(p => p.Id == projectId);
            if (project == null)
            {
                return null;
            }

            var task = new ProjectTask
            {
                Id = _nextTaskId++,
                Title = title,
                Description = description,
                AssignedTo = assignedTo,
                Status = status,
                ProjectId = projectId,
                ProjectName = project.Name
            };

            _tasks.Add(task);
            return Clone(task);
        }
    }

    private void Seed()
    {
        var premiumSofa = CreateProject("Premium sofa", "Izrada luksuzne sofe za salon u Beogradu");
        var royalWardrobe = CreateProject("Royal garderober", "Proizvodnja garderobera po meri za kraljevsku rezidenciju");
        var officeProgram = CreateProject("Ofis program", "Serija radnih stolova za korporativnog klijenta");

        CreateTask("Izrada sofa okvira", "Priprema drveta i sastavljanje okvira", "Milica", TaskStatus.ToDo, premiumSofa);
        CreateTask("Tapaciranje naslona", "Postavljanje pjene i platna", "Darko", TaskStatus.InProgress, premiumSofa);
        CreateTask("Finalna kontrola", "Pregled završne obrade", "Ana", TaskStatus.Done, premiumSofa);

        CreateTask("Montaža garderobera", "Sastavljanje elemenata u radionici", "Jovan", TaskStatus.ToDo, royalWardrobe);
        CreateTask("Lakiranje frontova", "Dve ruke bezbojnog laka", "Ivana", TaskStatus.InProgress, royalWardrobe);
        CreateTask("Pakovanje za isporuku", "Zaštita ivica i ambalaža", "Luka", TaskStatus.ToDo, royalWardrobe);

        CreateTask("Sečenje ploča", "Priprema radnih ploča za montažnu liniju", "Stefan", TaskStatus.InProgress, officeProgram);
        CreateTask("Provera kvaliteta", "Kontrola dimenzija i ivica", "Sara", TaskStatus.ToDo, officeProgram);
    }

    private int CreateProject(string name, string description)
    {
        var project = new Project
        {
            Id = _nextProjectId++,
            Name = name,
            Description = description
        };

        _projects.Add(project);
        return project.Id;
    }

    private void CreateTask(string title, string description, string assignedTo, TaskStatus status, int projectId)
    {
        var projectName = _projects.First(p => p.Id == projectId).Name;
        _tasks.Add(new ProjectTask
        {
            Id = _nextTaskId++,
            Title = title,
            Description = description,
            AssignedTo = assignedTo,
            Status = status,
            ProjectId = projectId,
            ProjectName = projectName
        });
    }

    private static ProjectTask Clone(ProjectTask task)
    {
        return new ProjectTask
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            AssignedTo = task.AssignedTo,
            Status = task.Status,
            ProjectId = task.ProjectId,
            ProjectName = task.ProjectName
        };
    }
}
