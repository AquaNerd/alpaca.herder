var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.AlpacaHerder>("alpacaherder");

builder.Build().Run();
