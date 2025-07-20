namespace ProjectAssignmentPortal.Application.Models.Projects.Enums;

/// <summary>
///     Enumeration for Project Status.
///     Represents the different states a project can be in.
///     Maps to PostgreSQL enum: statut_project
/// </summary>
public enum ProjectStatus
{
    Business = 0,
    BidLost = 1,
    BidProposal = 2,
}
