using Dutchskull.Aspire.PolyRepo;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("redis");

var dbRepo = builder.AddRepository(
    "repository",
    "https://github.com/danjaniell/Aspire.Sample.Infrastructure.git",
    c => c.WithDefaultBranch("local")
          .WithTargetPath("../../repos"));

var apiRepo = builder.AddRepository(
    "repository",
    "https://github.com/danjaniell/Aspire.Sample.API.git",
    c => c.WithDefaultBranch("local")
          .WithTargetPath("../../repos"));

var uiRepo = builder.AddRepository(
    "repository",
    "https://github.com/danjaniell/Aspire.Sample.UI.git",
    c => c.WithDefaultBranch("local")
          .WithTargetPath("../../repos"));

var db = builder
    .AddProjectFromRepository("aspire-sample-db", dbRepo,
        "Infrastructure.DB.InMemory/*csproj");
var api = builder
    .AddProjectFromRepository("aspire-sample-api", apiRepo,
        "Weather.API/Weather.API.csproj");
var ui = builder
    .AddProjectFromRepository("aspire-sample-ui", uiRepo,
        "Weather.UI/Weather.UI.csproj");

builder.Build().Run();