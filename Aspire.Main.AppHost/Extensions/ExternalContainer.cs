using Aspire.Hosting.Lifecycle;
using CliWrap.EventStream;
using CliWrap;
using Microsoft.Extensions.Logging;

namespace Aspire.Main.AppHost.Extensions;

internal sealed class ExternalContainerResource(string name, string containerNameOrId) : Resource(name)
{
    public string ContainerNameOrId { get; } = containerNameOrId;
}

internal static class ExternalContainerResourceExtensions
{
    public static IResourceBuilder<ExternalContainerResource> AddExternalContainer(this IDistributedApplicationBuilder builder, string name, string containerNameOrId)
    {
        builder.Services.TryAddLifecycleHook<ExternalContainerResourceLifecycleHook>();

        return builder.AddResource(new ExternalContainerResource(name, containerNameOrId))
            .WithInitialState(new CustomResourceSnapshot
            {
                ResourceType = "External container",
                State = "Starting",
                Properties = [new ResourcePropertySnapshot(CustomResourceKnownProperties.Source, "Custom")]
            })
            .ExcludeFromManifest();
    }
}

internal sealed class ExternalContainerResourceLifecycleHook(ResourceNotificationService notificationService, ResourceLoggerService loggerService)
    : IDistributedApplicationLifecycleHook, IAsyncDisposable
{
    private readonly CancellationTokenSource _tokenSource = new();

    public Task BeforeStartAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
    {
        foreach (var resource in appModel.Resources.OfType<ExternalContainerResource>())
        {
            this.StartTrackingExternalContainerLogs(resource, this._tokenSource.Token);
        }

        return Task.CompletedTask;
    }

    private void StartTrackingExternalContainerLogs(ExternalContainerResource resource, CancellationToken cancellationToken)
    {
        var logger = loggerService.GetLogger(resource);

        _ = Task.Run(async () =>
        {
            var cmd = Cli.Wrap("docker").WithArguments(["logs", "--follow", resource.ContainerNameOrId]);
            var cmdEvents = cmd.ListenAsync(cancellationToken);

            await foreach (var cmdEvent in cmdEvents)
            {
                switch (cmdEvent)
                {
                    case StartedCommandEvent:
                        await notificationService.PublishUpdateAsync(resource, state => state with { State = "Running" });
                        break;
                    case ExitedCommandEvent:
                        await notificationService.PublishUpdateAsync(resource, state => state with { State = "Finished" });
                        break;
                    case StandardOutputCommandEvent stdOut:
                        logger.LogInformation("External container {ResourceName} stdout: {StdOut}", resource.Name, stdOut.Text);
                        break;
                    case StandardErrorCommandEvent stdErr:
                        logger.LogInformation("External container {ResourceName} stderr: {StdErr}", resource.Name, stdErr.Text);
                        break;
                }
            }
        }, cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        this._tokenSource.Cancel();
        return default;
    }
}
