using ProjectAssignmentPortal.Application.Models.Companies;
using ProjectAssignmentPortal.Application.Models.Countries;
using ProjectAssignmentPortal.Application.Models.Organisations;
using ProjectAssignmentPortal.Application.Models.Projects.Enums;
using ProjectAssignmentPortal.Application.Models.Workers;

namespace ProjectAssignmentPortal.Application.Models.Projects;

/// <summary>
///     Value object for project identity information.
/// </summary>
public record ProjectIdentity(string Id, string Number, string Name, ProjectType Type, ProjectStatus Status = ProjectStatus.Business);
/// <summary>
///     Value object for basic project information.
/// </summary>
public record ProjectBasicInfo(
    string? Site = null,
    string? Comment = null,
    string? Description = null,
    Worker? Manager = null,
    Worker? Leader = null,
    Worker? SupportManager = null,
    Worker? QualityManager = null,
    List<ProjectCompany>? Companies = null);
/// <summary>
///     Value object for project dates.
/// </summary>
public record ProjectDates(
    DateTime CreatedAt,
    DateTime? UpdatedAt = null,
    DateOnly? StartDate = null,
    DateOnly? EndDate = null,
    DateOnly? FinancialStartDate = null,
    DateOnly? FinancialClosingDate = null,
    DateOnly? CreationDateInReflex = null);
/// <summary>
///     Value object for project people/workers.
/// </summary>
public record ProjectWorkers(Worker? Manager = null, Worker? SalesManager = null, Worker? Creator = null);
/// <summary>
///     Value object for project organization information.
/// </summary>
public record ProjectOrganization(
    Organisation? BusinessUnit = null,
    Organisation? Branch = null,
    Organisation? Division = null,
    Organisation? Department = null,
    Country? Country = null,
    string? ActivitySector = null,
    string? MarketSector = null,
    List<(string Id, string Code, string Label)>? Locations = null);
/// <summary>
///     Data Transfer Object for Organisation information with localized names.
/// </summary>
public record OrganisationInfo(string Id, string NameFr, string? NameEn = null, string? Code = null, string Type = "");
/// <summary>
///     Data Transfer Object for Location information.
/// </summary>
public record LocationInfo(string Id, string Code, string Label);
/// <summary>
///     Data Transfer Object for Worker information in API responses.
/// </summary>
public record WorkerInfo(string Id, string Name, string? FirstName = null, string? Email = null, string? JobTitle = null, string? OrganisationName = null);
/// <summary>
///     External link information for projects
/// </summary>
public record ProjectExternalLinkInfo(string Id, string Type, string Label, string Url);
/// <summary>
///     Data Transfer Object for Project response.
/// </summary>
public record ProjectResponse(
    string Id,
    string Number,
    string Name,
    string Status,
    string Type,
    string? Site,
    DateOnly? StartDate,
    DateOnly? EndDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt = null,
    string? ReflexNumber = null,
    string? MissionNumber = null,
    string? CreatedBy = null,
    WorkerInfo? Manager = null,
    WorkerInfo? SalesManager = null,
    OrganisationInfo? BusinessUnit = null,
    OrganisationInfo? Branch = null,
    OrganisationInfo? Division = null,
    OrganisationInfo? Department = null,
    string? DirectoryUrl = null,
    string? TeamsUrl = null);
/// <summary>
///     Detailed Project response for single project vi
///     Extends basic project information with additional details.
/// </summary>
public record ProjectResponseDetails(
    string Id,
    string Number,
    string Name,
    string Status,
    string Type,
    string? Site,
    DateOnly? StartDate,
    DateOnly? EndDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt = null,
    string? ReflexNumber = null,
    string? MissionNumber = null,

    // Enhanced worker details (instead of just names)
    WorkerInfo? CreatedByInfo = null,
    WorkerInfo? Manager = null,
    WorkerInfo? SalesManager = null,

    // Organization details (same as ProjectResponse)
    OrganisationInfo? BusinessUnit = null,
    OrganisationInfo? Branch = null,
    OrganisationInfo? Division = null,
    OrganisationInfo? Department = null,
    List<CompanyInfo>? Companies = null,
    string? DirectoryUrl = null,
    string? TeamsUrl = null,

    // Additional detail information
    string? Comment = null,
    List<ProjectExternalLinkInfo>? ExternalLinks = null,

    // Location information
    List<LocationInfo>? Locations = null);
public record ClientInfo(string Id, string Name);
public record CompanyInfo(string Id, string Code, string Name, string Type, string? Role = null);
/// <summary>
///     Data Transfer Object for organisation filter with hierarchy information.
/// </summary>
public record OrganisationFilterDto(string Id, string Name, string Type, string? Code = null, string? ParentId = null, string? ParentName = null);
/// <summary>
///     Data Transfer Object for date range filter information.
/// </summary>
public record DateRangeDto(DateOnly? MinDate, DateOnly? MaxDate);
/// <summary>
///     Data Transfer Object for manager filter information.
/// </summary>
public record ManagerFilterDto(string Id, string Name, string? FirstName = null, string? Email = null);
