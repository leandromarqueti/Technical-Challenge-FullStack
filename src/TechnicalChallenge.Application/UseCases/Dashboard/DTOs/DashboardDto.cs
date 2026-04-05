using System.Collections.Generic;

namespace TechnicalChallenge.Application.UseCases.Dashboard.DTOs;

public class DashboardDto
{
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Balance { get; set; }
    public List<PersonSummaryDto> TotalsByPerson { get; set; } = new();
    public List<CategorySummaryDto> TotalsByCategory { get; set; } = new();
}

public class PersonSummaryDto
{
    public string Name { get; set; } = string.Empty;
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Balance { get; set; }
}

public class CategorySummaryDto
{
    public string Description { get; set; } = string.Empty;
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Balance { get; set; }
}
