using ProjectAssignmentPortal.Application.Models.Projects.Enums;
using ProjectAssignmentPortal.Infrastructure.ProjectExternalLink;

namespace ProjectAssignmentPortal.Infrastructure.Project;

/// <summary>
///     Database model for Project entity.
///     Maps to PostgreSQL table: project
/// </summary>
public class ProjectDb
{
    // Project identifiers
    public string? Arborescence { get; set; }
    public string? BranchId { get; set; }

    // Organization references
    public string? BusinessUnitId { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = default!;

    // Person references
    public string? CreatedById { get; set; }
    public string? DepartmentId { get; set; }

    // URLs
    public string? DirectoryUrl { get; set; }
    public string? DivisionId { get; set; }
    public DateOnly? EndDate { get; set; }

    // Navigation properties
    public List<ProjectExternalLinkDb> ExternalLinks { get; set; } = new();
    // Identity
    public required string Id { get; set; }
    public string? ManagerId { get; set; }
    public string? MissionNumber { get; set; }

    // Core properties
    public string Name { get; set; } = default!;
    public string Number { get; set; } = default!;
    public string? ReflexNumber { get; set; }
    public string? SalesManagerId { get; set; }
    public string? Site { get; set; }

    // Dates
    public DateOnly? StartDate { get; set; }
    public ProjectStatus Status { get; set; } = default!;
    public string? TeamsUrl { get; set; }
    public ProjectType Type { get; set; } = default!;
    public DateTime? UpdatedAt { get; set; }
}
