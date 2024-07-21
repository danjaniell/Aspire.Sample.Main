using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddProject<Infrastructure_DB_InMemory>("db");

var apiservice = builder.AddProject<Weather_API>("apiservice")
       .WithReference(database);

builder.AddProject<Weather_UI>("webfrontend")
       .WithReference(apiservice);

builder.Build().Run();
