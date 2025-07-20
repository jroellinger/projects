using ProjectAssignmentPortal.Application.Common;
using ProjectAssignmentPortal.Application.Models.Workers;

namespace ProjectAssignmentPortal.Application.Models.Projects;

/// <summary>
///     Extension methods for mapping between Domain entities and DTOs.
///     Keeps mapping logic centralized and testable.
/// </summary>
public static class ProjectMapping
{
    /// <summary>
    ///     Maps a domain entity to a DTO.
    /// </summary>
    public static Result<ProjectResponse> ToProjectResponse(this Project project)
    {
        if (project is null)
        {
            return Result<ProjectResponse>.Failure("Project cannot be null");
        }

        var response = new ProjectResponse(project.Id,
                                           project.Number,
                                           project.Name,
                                           project.Status.ToString(),
                                           project.Type.ToString(),
                                           project.Site,
                                           project.StartDate,
                                           project.EndDate,
                                           project.CreatedAt,
                                           project.UpdatedAt,
                                           project.ReflexNumber,
                                           project.MissionNumber,
                                           project.CreatedBy?.FullName,
                                           project.Manager != null
                                               ? new WorkerInfo(project.Manager.Id, project.Manager.FullName, project.Manager.FirstName, project.Manager.Email)
                                               : null,
                                           project.SalesManager != null
                                               ? new WorkerInfo(project.SalesManager.Id,
                                                                project.SalesManager.FullName,
                                                                project.SalesManager.FirstName,
                                                                project.SalesManager.Email)
                                               : null,
                                           project.BusinessUnit != null
                                               ? new OrganisationInfo(project.BusinessUnit.Id,
                                                                      project.BusinessUnit.NameFr,
                                                                      project.BusinessUnit.NameEn,
                                                                      project.BusinessUnit.Code,
                                                                      project.BusinessUnit.Type.ToString())
                                               : null,
                                           project.Branch != null
                                               ? new OrganisationInfo(project.Branch.Id,
                                                                      project.Branch.NameFr,
                                                                      project.Branch.NameEn,
                                                                      project.Branch.Code,
                                                                      project.Branch.Type.ToString())
                                               : null,
                                           project.Division != null
                                               ? new OrganisationInfo(project.Division.Id,
                                                                      project.Division.NameFr,
                                                                      project.Division.NameEn,
                                                                      project.Division.Code,
                                                                      project.Division.Type.ToString())
                                               : null,
                                           project.Department != null
                                               ? new OrganisationInfo(project.Department.Id,
                                                                      project.Department.NameFr,
                                                                      project.Department.NameEn,
                                                                      project.Department.Code,
                                                                      project.Department.Type.ToString())
                                               : null,
                                           project.DirectoryUrl,
                                           project.TeamsUrl);

        return Result<ProjectResponse>.Success(response);
    }

    /// <summary>
    ///     Maps a collection of domain entities to DTOs.
    /// </summary>
    public static Result<IReadOnlyList<ProjectResponse>> ToProjectResponse(this IEnumerable<Project> projects)
    {
        if (projects is null)
        {
            return Result<IReadOnlyList<ProjectResponse>>.Failure("Projects collection cannot be null");
        }

        var results = new List<ProjectResponse>();

        foreach (var project in projects)
        {
            var result = project.ToProjectResponse();

            if (!result.IsSuccess)
            {
                return Result<IReadOnlyList<ProjectResponse>>.Failure(result.Error);
            }

            results.Add(result.Value);
        }

        return Result<IReadOnlyList<ProjectResponse>>.Success(results.AsReadOnly());
    }

    /// <summary>
    ///     Maps a domain entity to a detailed DTO with complete worker information.
    /// </summary>
    public static Result<ProjectResponseDetails> ToProjectResponseDetails(this Project project)
    {
        if (project is null)
        {
            return Result<ProjectResponseDetails>.Failure("Project cannot be null");
        }

        // Map the external links if available
        var externalLinks = project.ExternalLinks.Select(link => new ProjectExternalLinkInfo(link.Id, // Already a Guid
                                                                                             link.Type.ToString(),
                                                                                             link.Label,
                                                                                             link.Url))
                                   .ToList();

        // Map locations from domain model
        var locationInfos = project.Locations?.Select(loc => new LocationInfo(loc.Id, loc.Code, loc.Label))
                                   .ToList();

        var response = new ProjectResponseDetails(
            // Base project information
            project.Id,
            project.Number,
            project.Name,
            project.Status.ToString(),
            project.Type.ToString(),
            project.Site,
            project.StartDate,
            project.EndDate,
            project.CreatedAt,
            project.UpdatedAt,
            project.ReflexNumber,
            project.MissionNumber,
            // Enhanced worker details
            project.CreatedBy?.ToDto(),
            project.Manager?.ToDto(),
            project.SalesManager?.ToDto(),

            // Organization information
            project.BusinessUnit != null
                ? new OrganisationInfo(project.BusinessUnit.Id,
                                       project.BusinessUnit.NameFr,
                                       project.BusinessUnit.NameEn,
                                       project.BusinessUnit.Code,
                                       project.BusinessUnit.Type.ToString())
                : null,
            project.Branch != null
                ? new OrganisationInfo(project.Branch.Id, project.Branch.NameFr, project.Branch.NameEn, project.Branch.Code, project.Branch.Type.ToString())
                : null,
            project.Division != null
                ? new OrganisationInfo(project.Division.Id,
                                       project.Division.NameFr,
                                       project.Division.NameEn,
                                       project.Division.Code,
                                       project.Division.Type.ToString())
                : null,
            project.Department != null
                ? new OrganisationInfo(project.Department.Id,
                                       project.Department.NameFr,
                                       project.Department.NameEn,
                                       project.Department.Code,
                                       project.Department.Type.ToString())
                : null,
            project.Companies?.Select(pc => new CompanyInfo(pc.Company.Id, pc.Company.Code, pc.Company.Name, pc.Company.Type))
                   .ToList(),
            // URLs
            project.DirectoryUrl,
            project.TeamsUrl,

            // Additional detail information
            project.Comment,
            externalLinks.Count > 0 ? externalLinks : null,

            // Location information
            locationInfos?.Count > 0 ? locationInfos : null);

        return Result<ProjectResponseDetails>.Success(response);
    }
}
