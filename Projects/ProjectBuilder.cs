using ProjectAssignmentPortal.Application.Common;
using ProjectAssignmentPortal.Application.Models.Clients;
using ProjectAssignmentPortal.Application.Models.Companies;
using ProjectAssignmentPortal.Application.Models.Countries;
using ProjectAssignmentPortal.Application.Models.Markets;
using ProjectAssignmentPortal.Application.Models.Organisations;
using ProjectAssignmentPortal.Application.Models.ProfitCenters;
using ProjectAssignmentPortal.Application.Models.Projects.Enums;
using ProjectAssignmentPortal.Application.Models.ServiceTypes;
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
    private Client? _client;
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
    private Market? _market;
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
    ///     Configures the business unit.
    /// </summary>
    public ProjectBuilder WithBusinessUnit(Organisation? businessUnit)
    {
        _businessUnit = businessUnit;

        return this;
    }

    /// <summary>
    ///     Configures the client.
    /// </summary>
    public ProjectBuilder WithClient(Client? client)
    {
        _client = client;

        return this;
    }

    /// <summary>
    ///     Configures the project comment.
    /// </summary>
    public ProjectBuilder WithComment(string? comment)
    {
        _comment = comment;

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
    ///     Configures the project end date.
    /// </summary>
    public ProjectBuilder WithEndDate(DateOnly? endDate)
    {
        _endDate = endDate;

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
    ///     Configures the project locations.
    /// </summary>
    public ProjectBuilder WithLocations(List<(string Id, string Code, string Label)>? locations)
    {
        _locations = locations;

        return this;
    }

    /// <summary>
    ///     Configures the project manager.
    /// </summary>
    public ProjectBuilder WithManager(Worker? manager)
    {
        _manager = manager;

        return this;
    }

    /// <summary>
    ///     Configures the market.
    /// </summary>
    public ProjectBuilder WithMarket(Market? market)
    {
        _market = market;

        return this;
    }

    /// <summary>
    ///     Configures the market sector.
    /// </summary>
    public void WithMarketSector(string? marketSector)
    {
        _marketSector = marketSector;
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
    ///     Conditionally sets optional branch if result is successful
    /// </summary>
    public ProjectBuilder WithOptionalBranch(Result<Organisation>? branchResult) => branchResult != null && branchResult.IsSuccess
        ? WithBranch(branchResult.Value)
        : this;

    /// <summary>
    ///     Conditionally sets optional business unit if result is successful
    /// </summary>
    public ProjectBuilder WithOptionalBusinessUnit(Result<Organisation>? businessUnitResult) => businessUnitResult != null && businessUnitResult.IsSuccess
        ? WithBusinessUnit(businessUnitResult.Value)
        : this;

    /// <summary>
    ///     Conditionally sets optional client if result is successful
    /// </summary>
    public ProjectBuilder WithOptionalClient(Result<Client>? clientResult) => clientResult != null && clientResult.IsSuccess
        ? WithClient(clientResult.Value)
        : this;

    /// <summary>
    ///     Conditionally sets optional comment if not null or whitespace
    /// </summary>
    public ProjectBuilder WithOptionalComment(string? comment) => !string.IsNullOrWhiteSpace(comment) ? WithComment(comment) : this;

    /// <summary>
    ///     Conditionally sets companies if the list is not null or empty
    /// </summary>
    public ProjectBuilder WithOptionalCompanies(List<Company> companies)
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

        return WithCompanies(projectCompanies);
    }

    /// <summary>
    ///     Conditionally sets optional department if result is successful
    /// </summary>
    public ProjectBuilder WithOptionalDepartment(Result<Organisation>? departmentResult) => departmentResult != null && departmentResult.IsSuccess
        ? WithDepartment(departmentResult.Value)
        : this;

    /// <summary>
    ///     Conditionally sets optional directory URL if not null or whitespace
    /// </summary>
    public ProjectBuilder WithOptionalDirectoryUrl(string? directoryUrl) => !string.IsNullOrWhiteSpace(directoryUrl) ? WithDirectoryUrl(directoryUrl) : this;

    /// <summary>
    ///     Conditionally sets optional division if result is successful
    /// </summary>
    public ProjectBuilder WithOptionalDivision(Result<Organisation>? divisionResult) => divisionResult != null && divisionResult.IsSuccess
        ? WithDivision(divisionResult.Value)
        : this;

    /// <summary>
    ///     Conditionally sets optional end date if has value
    /// </summary>
    public ProjectBuilder WithOptionalEndDate(DateOnly? endDate) => endDate.HasValue ? WithEndDate(endDate.Value) : this;

    /// <summary>
    ///     Conditionally sets optional locations if not null or empty
    /// </summary>
    public ProjectBuilder WithOptionalLocations(List<(string Id, string Code, string Label)>? locations) =>
        locations != null && locations.Count > 0 ? WithLocations(locations) : this;

    /// <summary>
    ///     Conditionally sets optional manager if not null
    /// </summary>
    public ProjectBuilder WithOptionalManager(Worker? manager) => manager != null ? WithManager(manager) : this;

    /// <summary>
    ///     Conditionally sets optional project creator if not null
    /// </summary>
    public ProjectBuilder WithOptionalProjectCreator(Worker? creator) => creator != null ? WithProjectCreator(creator) : this;

    /// <summary>
    ///     Conditionally sets optional sales manager if not null
    /// </summary>
    public ProjectBuilder WithOptionalSalesManager(Worker? salesManager) => salesManager != null ? WithSalesManager(salesManager) : this;

    /// <summary>
    ///     Conditionally sets optional string value if not null or whitespace
    /// </summary>
    public ProjectBuilder WithOptionalSite(string? site) => !string.IsNullOrWhiteSpace(site) ? WithSite(site) : this;

    /// <summary>
    ///     Conditionally sets optional start date if has value
    /// </summary>
    public ProjectBuilder WithOptionalStartDate(DateOnly? startDate) => startDate.HasValue ? WithStartDate(startDate.Value) : this;

    /// <summary>
    ///     Conditionally sets optional updated date if has value
    /// </summary>
    public ProjectBuilder WithOptionalUpdatedAt(DateTime? updatedAt) => updatedAt.HasValue ? WithUpdatedAt(updatedAt.Value) : this;

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
        _creator = projectCreator;

        return this;
    }

    public ProjectBuilder WithSalesManager(Worker? salesManager)
    {
        _salesManager = salesManager;

        return this;
    }

    /// <summary>
    ///     Configures the service type.
    /// </summary>
    public ProjectBuilder WithServiceType(ServiceType? serviceType) => this;

    /// <summary>
    ///     Configures the project site.
    /// </summary>
    public ProjectBuilder WithSite(string? site)
    {
        _site = site;

        return this;
    }

    /// <summary>
    ///     Configures the project start date.
    /// </summary>
    public ProjectBuilder WithStartDate(DateOnly? startDate)
    {
        _startDate = startDate;

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
    public ProjectBuilder WithUpdatedAt(DateTime updatedAt)
    {
        _updatedAt = updatedAt;

        return this;
    }
}
