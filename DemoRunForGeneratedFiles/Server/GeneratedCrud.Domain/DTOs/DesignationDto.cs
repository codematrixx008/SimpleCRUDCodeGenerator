namespace GeneratedCrud.Domain.DTOs;

public sealed class DesignationDto
{
    public int Id { get; set; }
    public string DesignationName { get; set; } = string.Empty;
    public string DesignationCode { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool IsDeleted { get; set; }
}
