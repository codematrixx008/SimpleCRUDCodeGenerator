namespace CodeGen.Core.Models;

public sealed class FrontendGenerationRequest
{
    public string TableName { get; set; } = "dbo.tblEmployee";
    public string FrontendAppName { get; set; } = "SimpleEmployeeCRUD.React";
    public List<RelationDefinition> Relations { get; set; } = new();
}
