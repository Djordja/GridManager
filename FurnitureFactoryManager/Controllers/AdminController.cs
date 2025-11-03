using FurnitureFactoryManager.Models;
using FurnitureFactoryManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureFactoryManager.Controllers;

public class AdminController : Controller
{
    private readonly ProjectStore _store;

    public AdminController(ProjectStore store)
    {
        _store = store;
    }

    public IActionResult Index(int? projectId)
    {
        var projects = _store.GetAllProjects();
        if (projects.Count == 0)
        {
            return View(new AdminBoardViewModel());
        }

        var selectedProjectId = projectId.HasValue && projects.Any(p => p.Id == projectId.Value)
            ? projectId.Value
            : projects[0].Id;

        var tasks = _store.GetTasksByProject(selectedProjectId);

        var viewModel = new AdminBoardViewModel
        {
            Projects = projects,
            SelectedProjectId = selectedProjectId,
            Tasks = tasks,
            NewTask = new AdminBoardViewModel.NewTaskInput
            {
                ProjectId = selectedProjectId,
                Status = TaskStatus.ToDo
            }
        };

        if (TempData.ContainsKey("FormErrors"))
        {
            ViewBag.FormErrors = TempData["FormErrors"] as string;
        }

        if (TempData.ContainsKey("FormSuccess"))
        {
            ViewBag.FormSuccess = TempData["FormSuccess"] as string;
        }

        if (TempData.TryGetValue("NewTask.Title", out var storedTitle))
        {
            viewModel.NewTask.Title = storedTitle?.ToString() ?? string.Empty;
        }

        if (TempData.TryGetValue("NewTask.Description", out var storedDescription))
        {
            viewModel.NewTask.Description = storedDescription?.ToString() ?? string.Empty;
        }

        if (TempData.TryGetValue("NewTask.AssignedTo", out var storedAssignee))
        {
            viewModel.NewTask.AssignedTo = storedAssignee?.ToString() ?? string.Empty;
        }

        if (TempData.TryGetValue("NewTask.Status", out var storedStatus)
            && Enum.TryParse<TaskStatus>(storedStatus?.ToString(), out var parsedStatus))
        {
            viewModel.NewTask.Status = parsedStatus;
        }

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddTask(AdminBoardViewModel.NewTaskInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Title) || string.IsNullOrWhiteSpace(input.AssignedTo))
        {
            TempData["FormErrors"] = "Popunite obavezna polja: naziv zadatka i ime zaposlenog.";
            TempData["NewTask.Title"] = input.Title ?? string.Empty;
            TempData["NewTask.Description"] = input.Description ?? string.Empty;
            TempData["NewTask.AssignedTo"] = input.AssignedTo ?? string.Empty;
            TempData["NewTask.Status"] = input.Status.ToString();
            return RedirectToAction(nameof(Index), new { projectId = input.ProjectId });
        }

        var title = input.Title.Trim();
        var assignedTo = input.AssignedTo.Trim();
        var description = input.Description?.Trim() ?? string.Empty;

        var created = _store.AddTask(title, description, assignedTo, input.Status, input.ProjectId);
        if (created == null)
        {
            TempData["FormErrors"] = "Odabrani projekat ne postoji.";
            TempData["NewTask.Title"] = input.Title ?? string.Empty;
            TempData["NewTask.Description"] = input.Description ?? string.Empty;
            TempData["NewTask.AssignedTo"] = input.AssignedTo ?? string.Empty;
            TempData["NewTask.Status"] = input.Status.ToString();
            return RedirectToAction(nameof(Index));
        }

        TempData["FormSuccess"] = "Zadatak je uspešno dodat na tablu.";
        return RedirectToAction(nameof(Index), new { projectId = created.ProjectId });
    }

    [HttpPost]
    public IActionResult UpdateStatus([FromBody] UpdateStatusRequest request)
    {
        if (!Enum.IsDefined(typeof(TaskStatus), request.Status))
        {
            return BadRequest("Nepoznat status");
        }

        var status = Enum.Parse<TaskStatus>(request.Status);
        var updated = _store.UpdateStatus(request.TaskId, status);

        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    public class UpdateStatusRequest
    {
        public int TaskId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
