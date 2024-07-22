using Projects;
using System.Net.Sockets;

var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddProject<Infrastructure_DB_InMemory>("db");

var apiservice = builder.AddProject<Weather_API>("apiservice")
       .WithReference(database);

builder.AddProject<Weather_UI>("webfrontend")
       .WithReference(apiservice);

builder
    .AddResource(new ContainerResource("seq"))
    .WithAnnotation(new EndpointAnnotation(ProtocolType.Tcp, uriScheme: "http", name: "seq", port: 5341, targetPort: 80))
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithAnnotation(new ContainerImageAnnotation { Image = "datalust/seq", Tag = "latest" });

builder.Build().Run();
