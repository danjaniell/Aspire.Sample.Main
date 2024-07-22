var builder = DistributedApplication.CreateBuilder(args);

builder.AddContainer("aspire-sample-db", "ghcr.io/danjaniell/aspire.sample.infrastructure", "master")
       .WithHttpEndpoint(port: 7227, targetPort: 8080, name: "aspire-sample-db");
builder.AddContainer("aspire-sample-api", "ghcr.io/danjaniell/aspire.sample.api", "master")
       .WithHttpEndpoint(port: 7017, targetPort: 8080, name: "aspire-sample-api")
       .WithEnvironment("DBUrl", "http://localhost:7227");
builder.AddContainer("aspire-sample-ui", "ghcr.io/danjaniell/aspire.sample.ui", "master")
       .WithHttpEndpoint(port: 7016, targetPort: 8080, name: "aspire-sample-ui")
       .WithEnvironment("APIUrl", "http://localhost:7017");

builder.Build().Run();
