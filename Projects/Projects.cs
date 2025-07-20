using ProjectAssignmentPortal.Application.Common;

namespace ProjectAssignmentPortal.Application.Models.Projects;

/// <summary>
///     First-class collection for Project entities following Object Calisthenics principles.
///     Encapsulates business rules and provides behavior-rich operations on collections of projects.
///     Prevents exposure of raw collections and provides domain-specific operations.
/// </summary>
public sealed class Projects
{
    private readonly List<Project> _projects;
    public int Count => _projects.Count;

    private Projects() => _projects = [];

    public Result<Projects> Add(Project project)
    {
        if (project == null)
        {
            return Result<Projects>.Failure("Cannot add null project");
        }

        _projects.Add(project);

        return Result<Projects>.Success(this);
    }

    public static Result<Projects> Create() => Result<Projects>.Success(new Projects());

    public IReadOnlyList<Project> GetProjects() => _projects.AsReadOnly();
}
