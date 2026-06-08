Add this using in {{SolutionName}}.Api/Program.cs:

using {{SolutionName}}.Infrastructure;

Add this line before "var app = builder.Build();":

builder.Services.AddInfrastructure(builder.Configuration);
