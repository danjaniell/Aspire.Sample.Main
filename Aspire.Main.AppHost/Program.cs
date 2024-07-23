using Aspire.Main.AppHost;

var builder = DistributedApplication.CreateBuilder(args);
string dockerHostIp = Configurations.GetDockerHostIp();

builder
    .AddContainer("aspire-sample-db", "ghcr.io/danjaniell/aspire.sample.infrastructure", "master")
    //.WithEndpoint(scheme: "http", port: 7227, targetPort: 1818, name: "aspire-sample-db", isProxied: false, isExternal: true)
    .WithHttpEndpoint(port: 7227, targetPort: 1818, name: "aspire-sample-db", isProxied: false)
    .WithContainerRuntimeArgs("--net=host");
builder
    .AddContainer("aspire-sample-api", "ghcr.io/danjaniell/aspire.sample.api", "master")
    //.WithEndpoint(scheme: "http", port: 7017, targetPort: 1919, name: "aspire-sample-api", isProxied: false, isExternal: true)
    .WithHttpEndpoint(port: 7017, targetPort: 1919, name: "aspire-sample-api", isProxied: false)
    .WithEnvironment("DBUrl", $"http://{dockerHostIp}:1818")
    .WithContainerRuntimeArgs("--net=host");
builder
    .AddContainer("aspire-sample-ui", "ghcr.io/danjaniell/aspire.sample.ui", "master")
    //.WithEndpoint(scheme: "http", port: 7016, targetPort: 2020, name: "aspire-sample-ui", isProxied: false, isExternal: true)
    .WithHttpEndpoint(port: 7016, targetPort: 2020, name: "aspire-sample-ui", isProxied: false)
    .WithEnvironment("APIUrl", $"http://{dockerHostIp}:1919")
    .WithContainerRuntimeArgs("--net=host");

var app = builder.Build();
await app.RunAsync();
