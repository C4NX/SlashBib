using System.Runtime.InteropServices;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;
using SlashBib.Core.Configuration;
using SlashBib.Core.Utilities;

namespace SlashBib.Core;

/// <summary>
/// Slash bib main class
/// </summary>
public class SlashBibBot
{
    private readonly DiscordClient _discordClient;
    private readonly SlashConfiguration _configuration;
    private ILogger _logger;
    private LoggingLevelSwitch? _loggingLevelSwitch;
    
    private static SlashBibBot? _instance;
    
    public bool IsDebug
    {
        get => _configuration.Debug;
        set
        {
            if (_loggingLevelSwitch != null)
                _loggingLevelSwitch.MinimumLevel = value ? LogEventLevel.Debug : LogEventLevel.Information;
            
            _configuration.Debug = value;
        }
    }

    public DiscordClient Discord
        => _discordClient;

    public SlashConfiguration Configuration
        => _configuration;

    private SlashBibBot(SlashConfiguration configuration, bool setLoggerAsGlobal = true)
    {
        _configuration = configuration;
        
        ConfigureLogger(setLoggerAsGlobal);
        _logger ??= Log.Logger; 
        
        _discordClient = new DiscordClient(new DiscordConfiguration
        {
            Token = configuration.GetSecret("TOKEN", null) 
                    ?? throw new KeyNotFoundException($"'TOKEN' missing in {configuration.SecretFilename}"),
            LoggerFactory = new SerilogLoggerFactory(_logger)
        });
        
        ConfigureHandlers();
        ConfigureExtension();

        var names = CommandHelper.GetLanguagesNames(this)
            .ToArray();
        var test = _configuration.GetOption<string>("fr_test");
        
        _instance = this;
    }

    private void ConfigureLogger(bool setGlobal)
    {
        _loggingLevelSwitch = new LoggingLevelSwitch(_configuration.Debug 
            ? LogEventLevel.Debug 
            : LogEventLevel.Information);
        _logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(_loggingLevelSwitch)
            .WriteTo.Console()
            .CreateLogger();

        if (setGlobal)
            Log.Logger = _logger;
    }

    public void ConfigureExtension()
    {
        _discordClient.UseSlashCommands()
            .RegisterCommands(typeof(Program).Assembly);
    }

    private void ConfigureHandlers()
    {
        _discordClient.Ready += DiscordClientOnReady;
    }

    public static SlashBibBot GetInstance()
        => _instance
           ?? throw new NullReferenceException("No SlashBibBot instance, please use static method Create(...)");
    
    private Task DiscordClientOnReady(DiscordClient sender, ReadyEventArgs e)
    {
        _logger.Information("Ready {Name} ({Id}): {PingStr}",
            sender.CurrentApplication.Name,
            sender.CurrentApplication.Id,
            $"{sender.Ping}ms");

        return Task.CompletedTask;
    }

    public async Task ReloadAsync()
    {
        _configuration.Reload();
        
        //TODO: Reload
        await Task.CompletedTask;
    }
    
    public async Task RunAsync()
    {
        _logger.Information("Running SlashBib {Version} with.NET {RuntimeVersion} with {OperatingSystem}"
        , typeof(Program).Assembly.GetName().Version
        , Emzi0767.Utilities.RuntimeInformation.Version
        , RuntimeInformation.RuntimeIdentifier);
        
        if(IsDebug)
            _logger.Debug("SlashBib is running on debug mode");
        
        await _discordClient.ConnectAsync();

        await Task.Delay(-1);
    }

    public DiscordEmbedBuilder GetEmbedBuilder()
        => _configuration.DefaultEmbed == null
            ? new DiscordEmbedBuilder()
            : new DiscordEmbedBuilder(_configuration.DefaultEmbed);

    public static SlashBibBot Create(string configFilename)
    {
        return new SlashBibBot(SlashConfiguration.FromFile(configFilename));
    }
}