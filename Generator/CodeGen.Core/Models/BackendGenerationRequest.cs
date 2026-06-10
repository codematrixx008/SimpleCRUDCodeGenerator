namespace CodeGen.Core.Models;

public sealed class BackendGenerationRequest
{
    public string TableName { get; set; } = "dbo.tblEmployee";
    public string SolutionName { get; set; } = "SimpleEmployeeCRUD";
    public List<RelationDefinition> Relations { get; set; } = new();
}
