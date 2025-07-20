namespace ProjectAssignmentPortal.Application.Models.Projects;

/// <summary>
///     Enumeration for External Link Type.
///     Represents the different types of external links.
///     Maps to PostgreSQL enum: external_link_type
/// </summary>
public enum ExternalLinkType
{
    Teams = 0,
    Slack = 1,
    Discord = 2,
    SharePoint = 3,
    Notion = 4,
    Wiki = 5,
    Other = 6,
}
