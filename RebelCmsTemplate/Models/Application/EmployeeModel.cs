namespace RebelCmsTemplate.Models.Application;

public class EmployeeModel
{
    public int EmployeeKey { get; init; }
    public int TenantKey { get; init; }
    public string? EmployeeLastName { get; init; }
    public string? EmployeeFirstName { get; init; }
    public string? EmployeeTitle { get; init; }
    public string? EmployeeTitleOfCourtesy { get; init; }
    public DateOnly? EmployeeBirthDate { get; init; }
    public DateOnly? EmployeeHireDate { get; init; }
    public string? EmployeeAddress { get; init; }
    public string? EmployeeCity { get; init; }
    public string? EmployeeRegion { get; init; }
    public string? EmployeePostalCode { get; init; }
    public string? EmployeeCountry { get; init; }
    public string? EmployeeHomePhone { get; init; }
    public string? EmployeeExtension { get; init; }
    public byte[]? EmployeePhoto { get; set; }
    public string? EmployeePhotoBase64String { get; set; }
    public string? EmployeeNotes { get; init; }
    public string? EmployeePhotoPath { get; init; }
    public double EmployeeSalary { get; init; }
    public int IsDelete { get; init; }
}