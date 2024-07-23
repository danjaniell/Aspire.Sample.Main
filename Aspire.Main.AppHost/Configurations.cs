namespace Aspire.Main.AppHost;

public static class Configurations
{
    public static string GetDockerHostIp() =>
        Environment.GetEnvironmentVariable("DOCKER_HOST_IP") ?? string.Empty;
}
