using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Options;

namespace MyApplication;

[ExcludeFromCodeCoverage]
internal sealed partial class DelayedShutdownHostLifetime : IHostLifetime, IDisposable
{
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly ConsoleLifetimeOptions _options;
    private readonly IHostEnvironment _environment;
    private readonly HostOptions _hostOptions;
    private readonly ILogger _logger;

    private CancellationTokenRegistration _applicationStartedRegistration;
    private CancellationTokenRegistration _applicationStoppingRegistration;
    private IDisposable[]? _stopSignalRegistrations;

    public DelayedShutdownHostLifetime(
        IOptions<ConsoleLifetimeOptions> options,
        IHostEnvironment environment,
        IHostApplicationLifetime applicationLifetime,
        IOptions<HostOptions> hostOptions,
        ILoggerFactory loggerFactory)
    {
        _options = options.Value;
        _environment = environment;
        _applicationLifetime = applicationLifetime;
        _hostOptions = hostOptions.Value;
        _logger = loggerFactory.CreateLogger("Microsoft.Hosting.Lifetime");
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    public Task WaitForStartAsync(CancellationToken cancellationToken)
    {
        if (!_options.SuppressStatusMessages)
        {
            _applicationStartedRegistration = _applicationLifetime.ApplicationStarted.Register(
                (p) => ((DelayedShutdownHostLifetime)p!).OnApplicationStarted(),
                this);

            _applicationStoppingRegistration = _applicationLifetime.ApplicationStopping.Register(
                (p) => ((DelayedShutdownHostLifetime)p!).OnApplicationStopping(),
                this);
        }

        _stopSignalRegistrations = new[]
        {
            PosixSignalRegistration.Create(PosixSignal.SIGINT, HandleSignal),
            PosixSignalRegistration.Create(PosixSignal.SIGTERM, HandleSignal),
            PosixSignalRegistration.Create(PosixSignal.SIGQUIT, HandleSignal),
        };

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _applicationStartedRegistration.Dispose();
        _applicationStoppingRegistration.Dispose();

        if (_stopSignalRegistrations is { Length: > 0 })
        {
            foreach (var disposable in _stopSignalRegistrations)
            {
                disposable?.Dispose();
            }
        }
    }

    private void HandleSignal(PosixSignalContext ctx)
    {
        Log.ShutdownSignalReceived(_logger, ctx.Signal, _hostOptions.ShutdownTimeout);

        ctx.Cancel = true;
        Task.Delay(_hostOptions.ShutdownTimeout)
            .ContinueWith(t => _applicationLifetime.StopApplication(), TaskScheduler.Default);
    }

    private void OnApplicationStarted()
    {
        Log.ApplicationStarted(_logger);
        Log.ApplicationStartedHostingEnvironment(_logger, _environment.EnvironmentName);
        Log.ApplicationStartedContentRoot(_logger, _environment.ContentRootPath);
    }

    private void OnApplicationStopping()
    {
        Log.ApplicationShuttingDown(_logger);
    }

    internal static partial class Log
    {
        [LoggerMessage(1, LogLevel.Information, "Application started. Press Ctrl+C to shut down.", EventName = "ApplicationStarted")]
        public static partial void ApplicationStarted(ILogger logger);

        [LoggerMessage(2, LogLevel.Information, "Hosting environment: {EnvName}", EventName = "ApplicationStartedHostingEnvironment")]
        public static partial void ApplicationStartedHostingEnvironment(ILogger logger, string envName);

        [LoggerMessage(3, LogLevel.Information, "Content root path: {ContentRoot}", EventName = "ApplicationStartedContentRoot")]
        public static partial void ApplicationStartedContentRoot(ILogger logger, string contentRoot);

        [LoggerMessage(4, LogLevel.Information, "Shutdown signal received: {Signal} handling new requests for {DelayDuration}", EventName = "ShutdownSignalReceived")]
        public static partial void ShutdownSignalReceived(ILogger logger, PosixSignal signal, TimeSpan delayDuration);

        [LoggerMessage(5, LogLevel.Information, "Application is shutting down...", EventName = "ApplicationShuttingDown")]
        public static partial void ApplicationShuttingDown(ILogger logger);
    }
}
