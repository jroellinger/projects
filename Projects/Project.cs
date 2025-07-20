using ProjectAssignmentPortal.Application.Common;
using ProjectAssignmentPortal.Application.Models.Companies;
using ProjectAssignmentPortal.Application.Models.Countries;
using ProjectAssignmentPortal.Application.Models.Organisations;
using ProjectAssignmentPortal.Application.Models.ProfitCenters;
using ProjectAssignmentPortal.Application.Models.Projects.Enums;
using ProjectAssignmentPortal.Application.Models.Projects.ProjectExternalLinks;
using ProjectAssignmentPortal.Application.Models.ServiceTypes;
using ProjectAssignmentPortal.Application.Models.Workers;

namespace ProjectAssignmentPortal.Application.Models.Projects;

/// <summary>
///     Project aggregate root following Domain-Driven Design principles.
///     Uses private constructor with factory methods and encapsulates business logic.
///     Maps to PostgreSQL table: project
/// </summary>
public sealed class Project
{
    private readonly List<ProjectCompany> _companies = new();
    private readonly List<ProjectExternalLink> _externalLinks = new();
    private readonly List<(string Id, string Code, string Label)> _locations = new();
    public Organisation? Branch { get; }
    public Organisation? BusinessUnit { get; }
    public string? Comment { get; }

    // Collection navigation property for companies
    public IReadOnlyCollection<ProjectCompany> Companies => _companies.AsReadOnly();
    public Country? Country { get; }
    public DateTime CreatedAt { get; }
    public Worker? CreatedBy { get; }
    public DateOnly? CreationDateInReflex { get; }
    public Organisation? Department { get; }
    public string? DirectoryUrl { get; }
    public Organisation? Division { get; }
    public DateOnly? EndDate { get; }

    // Collection navigation property for external links
    public IReadOnlyCollection<ProjectExternalLink> ExternalLinks => _externalLinks.AsReadOnly();
    public DateOnly? FinancialClosingDate { get; }
    public DateOnly? FinancialStartDate { get; }
    public string Id { get; }
    public IReadOnlyList<(string Id, string Code, string Label)> Locations => _locations.AsReadOnly();
    public Worker? Manager { get; init; }
    public string? MissionNumber { get; }
    public string Name { get; }
    public string Number { get; }
    public ProfitCenter? ProfitCenter { get; }
    public Classification? ProjectClassification { get; }
    public string? ReflexNumber { get; }
    public Worker? SalesManager { get; }
    public ServiceType? ServiceType { get; }
    public string? Site { get; }
    public DateOnly? StartDate { get; init; }
    public ProjectStatus Status { get; init; }
    public string? TeamsUrl { get; private set; }
    public string[]? Tools { get; private set; }
    public ProjectType Type { get; }
    public DateTime? UpdatedAt { get; }

    /// <summary>
    ///     Internal constructor for both new projects and database reconstruction.
    ///     Uses Value Objects to reduce parameter count and improve maintainability.
    ///     Used directly by ProjectBuilder.
    /// </summary>
    internal Project(ProjectIdentity identity, ProjectBasicInfo basicInfo, ProjectDates dates, ProjectWorkers workers, ProjectOrganization organization)
    {
        Id = identity.Id;
        Number = identity.Number;
        Name = identity.Name;
        Type = identity.Type;
        Status = identity.Status;

        CreatedAt = dates.CreatedAt;
        UpdatedAt = dates.UpdatedAt;
        StartDate = dates.StartDate;
        EndDate = dates.EndDate;

        Site = basicInfo.Site;
        Comment = basicInfo.Comment;
        // Description property doesn't exist in Project class, map to Comment or ignore
        Manager = basicInfo.Manager;

        // Set companies from basicInfo
        if (basicInfo.Companies != null)
        {
            _companies.AddRange(basicInfo.Companies);
        }

        BusinessUnit = organization.BusinessUnit;
        Branch = organization.Branch;
        Division = organization.Division;
        Department = organization.Department;
        Country = organization.Country;

        SalesManager = workers.SalesManager;
        CreatedBy = workers.Creator;
        Manager = workers.Manager;

        // Set locations from organization
        if (organization.Locations != null)
        {
            _locations.AddRange(organization.Locations);
        }
    }

    /// <summary>
    ///     Creates a new project using the builder pattern.
    /// </summary>
    public static ProjectBuilder Builder() => new();

    /// <summary>
    ///     Business logic method to check if project can be modified.
    /// </summary>
    public bool CanBeModified() => Status switch
    {
        ProjectStatus.BidLost     => false,
        ProjectStatus.BidProposal => true,
        _                         => true,
    };

    /// <summary>
    ///     Business logic method to calculate project duration.
    /// </summary>
    public TimeSpan? GetDuration()
    {
        if (!StartDate.HasValue
            || !EndDate.HasValue)
        {
            return null;
        }

        return EndDate.Value.ToDateTime(TimeOnly.MinValue) - StartDate.Value.ToDateTime(TimeOnly.MinValue);
    }

    /// <summary>
    ///     Business logic method to check if project is in validation process.
    /// </summary>
    public bool IsInValidation() => Status == ProjectStatus.BidLost;

    /// <summary>
    ///     Business logic method to check if project is overdue.
    /// </summary>
    public bool IsOverdue() => EndDate.HasValue && EndDate.Value < DateOnly.FromDateTime(DateTime.Now) && Status != ProjectStatus.BidLost;

    /// <summary>
    ///     Business logic method to check if project is validated.
    /// </summary>
    public bool IsValidated() => Status == ProjectStatus.BidProposal;
}
