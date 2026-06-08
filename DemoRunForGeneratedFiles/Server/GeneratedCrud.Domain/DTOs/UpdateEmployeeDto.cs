namespace GeneratedCrud.Domain.DTOs;

public sealed class UpdateEmployeeDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DOB { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? Address { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool IsDeleted { get; set; }
}
