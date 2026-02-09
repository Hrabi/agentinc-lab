var builder = DistributedApplication.CreateBuilder(args);

var web = builder.AddProject<Projects.AgenticLab_Web>("web");

builder.Build().Run();
