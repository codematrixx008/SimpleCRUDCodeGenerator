namespace GeneratedCrud.Domain.DTOs;

public sealed class UpdateDepartmentDto
{
    public string DepartmentName { get; set; } = string.Empty;
    public string DepartmentCode { get; set; } = string.Empty;
    public string? Description { get; set; }
}
