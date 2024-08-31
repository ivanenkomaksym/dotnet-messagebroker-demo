var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.WebUI>("webui");

builder.Build().Run();
