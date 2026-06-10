namespace GeneratedCrud.Domain.DTOs;

public sealed class EmployeeDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DOB { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? Address { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool IsDeleted { get; set; }
    public int? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
}
