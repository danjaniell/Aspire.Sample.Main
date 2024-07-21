var builder = DistributedApplication.CreateBuilder(args);

builder.AddContainer("aspire-sample-db", "ghcr.io/danjaniell/aspire.sample.infrastructure", "master")
       .WithHttpEndpoint(port: 7227, targetPort: 8080);
builder.AddContainer("aspire-sample-api", "ghcr.io/danjaniell/aspire.sample.api", "master")
       .WithHttpEndpoint(port: 7017, targetPort: 8080)
       .WithEnvironment("DBUrl", "http://192.168.1.246:7227");
builder.AddContainer("aspire-sample-ui", "ghcr.io/danjaniell/aspire.sample.ui","master")
       .WithHttpEndpoint(port: 7016, targetPort: 8080)
       .WithEnvironment("APIUrl", "http://aspire-sample-api:8080");

builder.Build().Run();
