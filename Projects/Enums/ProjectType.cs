namespace ProjectAssignmentPortal.Application.Models.Projects.Enums;

/// <summary>
///     Enumeration for Project Type.
///     Represents the different types of projects.
///     Maps to PostgreSQL enum: type_project
/// </summary>
public enum ProjectType
{
    Travel = 0,
    Project = 1,
    Audit = 2,
    Support = 3,
    Other = 4,
}
