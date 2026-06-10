namespace CodeGen.Core.Models;

public sealed class FullStackGenerationRequest
{
    public string TableName { get; set; } = "dbo.tblEmployee";
    public string SolutionName { get; set; } = "SimpleEmployeeCRUD";
    public string FrontendAppName { get; set; } = "SimpleEmployeeCRUD.React";
    public List<RelationDefinition> Relations { get; set; } = new();
}
