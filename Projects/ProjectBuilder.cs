using ProjectAssignmentPortal.Application.Common;
using ProjectAssignmentPortal.Application.Models.Companies;
using ProjectAssignmentPortal.Application.Models.Countries;
using ProjectAssignmentPortal.Application.Models.Organisations;
using ProjectAssignmentPortal.Application.Models.ProfitCenters;
using ProjectAssignmentPortal.Application.Models.Projects.Enums;
using ProjectAssignmentPortal.Application.Models.Workers;

namespace ProjectAssignmentPortal.Application.Models.Projects;

/// <summary>
///     Builder pattern implementation for creating Project instances with many optional parameters.
///     Uses fluent interface for clean and readable object construction.
///     Supports both new project creation and database reconstruction scenarios.
/// </summary>
public class ProjectBuilder
{
    private string? _activitySector;

    // Additional optional properties from original Project class
    private Organisation? _branch;
    private Organisation? _businessUnit;
    private string? _comment;
    private List<ProjectCompany>? _companies;
    private Country? _country;
    private DateTime? _createdAt;
    private Worker? _creator;
    private Organisation? _department;
    private string? _description;
    private Organisation? _division;
    private DateOnly? _endDate;
    private DateOnly? _financialClosingDate;
    private DateOnly? _financialStartDate;

    // Database reconstruction fields
    private string? _id;
    private bool _isArchived;
    private bool _isClosed;
    private List<(string Id, string Code, string Label)>? _locations;
    private Worker? _manager;
    private string? _market;
    private string? _marketSector;
    private string _name = string.Empty;
    // Required fields
    private string _number = string.Empty;
    private ProfitCenter? _profitCenter;
    private Worker? _salesManager;

    // Optional project properties
    private string? _site;
    private DateOnly? _startDate;
    private ProjectStatus? _status;
    private ProjectType _type = ProjectType.Project;
    private DateTime? _updatedAt;

    /// <summary>
    ///     Builds the Project instance with validation.
    ///     Directly creates the project without intermediate factory method.
    /// </summary>
    public Result<Project> Build()
    {
        try
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(_number))
            {
                return Result<Project>.Failure("Project number cannot be empty or whitespace");
            }

            if (string.IsNullOrWhiteSpace(_name))
            {
                return Result<Project>.Failure("Project name cannot be empty or whitespace");
            }

            var projectBasicInfo = new ProjectBasicInfo(_site, _comment, _description, _manager, Companies: _companies);

            var projectDates = new ProjectDates(_createdAt ?? DateTime.UtcNow, _updatedAt, _startDate, _endDate, _financialStartDate, _financialClosingDate);

            var projectPeople = new ProjectWorkers(_manager, _salesManager, _creator);

            var projectOrganization = new ProjectOrganization(
                _businessUnit,
                _branch,
                _division,
                _department,
                _country,
                _activitySector,
                _marketSector,
                _locations);

            // Create identity with provided ID or new GUID
            var projectIdentity = new ProjectIdentity(_id
                                                      ?? Guid.NewGuid()
                                                             .ToString(),
                                                      _number,
                                                      _name,
                                                      _type,
                                                      _status ?? ProjectStatus.Business);

            var project = new Project(projectIdentity, projectBasicInfo, projectDates, projectPeople, projectOrganization);

            return Result<Project>.Success(project);
        }
        catch (Exception ex)
        {
            return Result<Project>.Failure($"Failed to build project: {ex.Message}");
        }
    }

    /// <summary>
    ///     Configures the activity sector.
    /// </summary>
    public ProjectBuilder WithActivitySector(string? activitySector)
    {
        _activitySector = activitySector;

        return this;
    }

    /// <summary>
    ///     Configures the branch.
    /// </summary>
    public ProjectBuilder WithBranch(Organisation? branch)
    {
        _branch = branch;

        return this;
    }

    /// <summary>
    ///     Configures the branch from a Result if successful.
    /// </summary>
    public ProjectBuilder WithBranch(Result<Organisation>? branchResult)
    {
        if (branchResult != null
            && branchResult.IsSuccess)
        {
            _branch = branchResult.Value;
        }

        return this;
    }

    /// <summary>
    ///     Configures the business unit.
    /// </summary>
    public ProjectBuilder WithBusinessUnit(Organisation? businessUnit)
    {
        _businessUnit = businessUnit;

        return this;
    }

    /// <summary>
    ///     Configures the business unit from a Result if successful.
    /// </summary>
    public ProjectBuilder WithBusinessUnit(Result<Organisation>? businessUnitResult)
    {
        if (businessUnitResult != null
            && businessUnitResult.IsSuccess)
        {
            _businessUnit = businessUnitResult.Value;
        }

        return this;
    }

    /// <summary>
    ///     Configures the project comment if not null or whitespace.
    /// </summary>
    public ProjectBuilder WithComment(string? comment)
    {
        if (!string.IsNullOrWhiteSpace(comment))
        {
            _comment = comment;
        }

        return this;
    }

    /// <summary>
    ///     Configures the project companies.
    /// </summary>
    public ProjectBuilder WithCompanies(List<ProjectCompany>? companies)
    {
        _companies = companies;

        return this;
    }

    /// <summary>
    ///     Configures project companies from a list of Company objects if not null or empty.
    /// </summary>
    public ProjectBuilder WithCompanies(List<Company>? companies)
    {
        if (companies == null
            || companies.Count == 0)
        {
            return this;
        }

        var projectCompanies = new List<ProjectCompany>();

        foreach (var company in companies)
        {
            var projectCompanyResult = ProjectCompany.Create(company);

            if (projectCompanyResult.IsSuccess)
            {
                projectCompanies.Add(projectCompanyResult.Value);
            }
        }

        _companies = projectCompanies;

        return this;
    }

    /// <summary>
    ///     Configures the country.
    /// </summary>
    public ProjectBuilder WithCountry(Country? country)
    {
        _country = country;

        return this;
    }

    /// <summary>
    ///     Configures the creation timestamp for database reconstruction.
    ///     This method should only be used when recreating projects from database records.
    /// </summary>
    public ProjectBuilder WithCreatedAt(DateTime createdAt)
    {
        _createdAt = createdAt;

        return this;
    }

    /// <summary>
    ///     Configures the department.
    /// </summary>
    public ProjectBuilder WithDepartment(Organisation? department)
    {
        _department = department;

        return this;
    }

    /// <summary>
    ///     Configures the department from a Result if successful.
    /// </summary>
    public ProjectBuilder WithDepartment(Result<Organisation>? departmentResult)
    {
        if (departmentResult != null
            && departmentResult.IsSuccess)
        {
            _department = departmentResult.Value;
        }

        return this;
    }

    /// <summary>
    ///     Configures the project description.
    /// </summary>
    public ProjectBuilder WithDescription(string? description)
    {
        _description = description;

        return this;
    }

    /// <summary>
    ///     Configures the directory URL.
    /// </summary>
    public ProjectBuilder WithDirectoryUrl(string? directoryUrl) => this;

    /// <summary>
    ///     Configures the division.
    /// </summary>
    public ProjectBuilder WithDivision(Organisation? division)
    {
        _division = division;

        return this;
    }

    /// <summary>
    ///     Configures the division from a Result if successful.
    /// </summary>
    public ProjectBuilder WithDivision(Result<Organisation>? divisionResult)
    {
        if (divisionResult != null
            && divisionResult.IsSuccess)
        {
            _division = divisionResult.Value;
        }

        return this;
    }

    /// <summary>
    ///     Configures the project end date if it has a value.
    /// </summary>
    public ProjectBuilder WithEndDate(DateOnly? endDate)
    {
        if (endDate.HasValue)
        {
            _endDate = endDate;
        }

        return this;
    }

    /// <summary>
    ///     Configures the financial closing date.
    /// </summary>
    public ProjectBuilder WithFinancialClosingDate(DateOnly? financialClosingDate)
    {
        _financialClosingDate = financialClosingDate;

        return this;
    }

    /// <summary>
    ///     Configures the financial start date.
    /// </summary>
    public ProjectBuilder WithFinancialStartDate(DateOnly? financialStartDate)
    {
        _financialStartDate = financialStartDate;

        return this;
    }

    /// <summary>
    ///     Configures the project ID for database reconstruction.
    ///     This method should only be used when recreating projects from database records.
    /// </summary>
    public ProjectBuilder WithId(string id)
    {
        _id = id;

        return this;
    }

    /// <summary>
    ///     Configures whether the project is archived.
    /// </summary>
    public ProjectBuilder WithIsArchived(bool isArchived)
    {
        _isArchived = isArchived;

        return this;
    }

    /// <summary>
    ///     Configures whether the project is closed.
    /// </summary>
    public ProjectBuilder WithIsClosed(bool isClosed)
    {
        _isClosed = isClosed;

        return this;
    }

    /// <summary>
    ///     Configures the project locations if not null or empty.
    /// </summary>
    public ProjectBuilder WithLocations(List<(string Id, string Code, string Label)>? locations)
    {
        if (locations != null
            && locations.Count > 0)
        {
            _locations = locations;
        }

        return this;
    }

    /// <summary>
    ///     Configures the project manager if not null.
    /// </summary>
    public ProjectBuilder WithManager(Worker? manager)
    {
        if (manager != null)
        {
            _manager = manager;
        }

        return this;
    }

    /// <summary>
    ///     Configures the market if not null or whitespace.
    /// </summary>
    public ProjectBuilder WithMarket(string? market)
    {
        if (!string.IsNullOrWhiteSpace(market))
        {
            _market = market;
        }

        return this;
    }

    /// <summary>
    ///     Configures the market sector.
    /// </summary>
    public ProjectBuilder WithMarketSector(string? marketSector)
    {
        _marketSector = marketSector;

        return this;
    }

    /// <summary>
    ///     Configures the project name (required).
    /// </summary>
    public ProjectBuilder WithName(string name)
    {
        _name = name;

        return this;
    }

    /// <summary>
    ///     Configures the project number (required).
    /// </summary>
    public ProjectBuilder WithNumber(string number)
    {
        _number = number;

        return this;
    }

    /// <summary>
    ///     Configures the planned budget.
    /// </summary>
    public ProjectBuilder WithPlannedBudget(decimal? plannedBudget) => this;

    /// <summary>
    ///     Configures the profit center.
    /// </summary>
    public ProjectBuilder WithProfitCenter(ProfitCenter? profitCenter)
    {
        _profitCenter = profitCenter;

        return this;
    }

    /// <summary>
    ///     Configures the project classification.
    /// </summary>
    public ProjectBuilder WithProjectClassification(string? projectClassification) => this;

    public ProjectBuilder WithProjectCreator(Worker? projectCreator)
    {
        if (projectCreator != null)
        {
            _creator = projectCreator;
        }

        return this;
    }

    public ProjectBuilder WithSalesManager(Worker? salesManager)
    {
        if (salesManager != null)
        {
            _salesManager = salesManager;
        }

        return this;
    }

    /// <summary>
    ///     Configures the project site if not null or whitespace.
    /// </summary>
    public ProjectBuilder WithSite(string? site)
    {
        if (!string.IsNullOrWhiteSpace(site))
        {
            _site = site;
        }

        return this;
    }

    /// <summary>
    ///     Configures the project start date if it has a value.
    /// </summary>
    public ProjectBuilder WithStartDate(DateOnly? startDate)
    {
        if (startDate.HasValue)
        {
            _startDate = startDate;
        }

        return this;
    }

    /// <summary>
    ///     Configures the project status for database reconstruction.
    ///     This method should only be used when recreating projects from database records.
    /// </summary>
    public ProjectBuilder WithStatus(ProjectStatus status)
    {
        _status = status;

        return this;
    }

    /// <summary>
    ///     Configures the project type (required).
    /// </summary>
    public ProjectBuilder WithType(ProjectType type)
    {
        _type = type;

        return this;
    }

    /// <summary>
    ///     Configures the last update timestamp for database reconstruction.
    ///     This method should only be used when recreating projects from database records.
    /// </summary>
    public ProjectBuilder WithUpdatedAt(DateTime? updatedAt)
    {
        if (updatedAt.HasValue)
        {
            _updatedAt = updatedAt;
        }

        return this;
    }
}
