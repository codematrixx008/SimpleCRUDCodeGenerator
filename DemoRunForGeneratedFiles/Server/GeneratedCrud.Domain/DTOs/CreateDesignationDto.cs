namespace GeneratedCrud.Domain.DTOs;

public sealed class CreateDesignationDto
{
    public string DesignationName { get; set; } = string.Empty;
    public string DesignationCode { get; set; } = string.Empty;
    public string? Description { get; set; }
}
