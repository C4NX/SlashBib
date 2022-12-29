using System.Runtime.InteropServices;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
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
    private readonly DynamicStringDataContainer _dynamicStrings;
    private readonly ActivitySwitcher _activitySwitcher;

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

    public ActivitySwitcher Activity
        => _activitySwitcher;

    public DynamicStringDataContainer Strings
        => _dynamicStrings;

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

        _dynamicStrings = new DynamicStringDataContainer();
        _dynamicStrings.ReadablitySettings.DiscordReady = true;

        ConfigureStrings();
        ConfigureHandlers();
        ConfigureExtension();

        _activitySwitcher = new ActivitySwitcher(this, new DiscordActivity[] {
            new DiscordActivity("{bot.username} is back !!"),
            new DiscordActivity("{bot.ping}ms, wow"),
            new DiscordActivity("On {guilds.count} towns !"),
        });

        _instance = this;
    }

    private void ConfigureStrings()
    {
        _dynamicStrings["bot.ping"] = new DynamicStringDataContainer.DynamicValue(() => _discordClient.Ping);
        _dynamicStrings["bot.username"] = new DynamicStringDataContainer.DynamicValue(() => _discordClient.CurrentUser.Username);
        _dynamicStrings["bot.version"] = typeof(Program).Assembly.GetName().Version?.ToString() ?? "Missing Version";
        _dynamicStrings["bot.guild.count"] = new DynamicStringDataContainer.DynamicValue(() => _discordClient.Guilds.Count);
        _dynamicStrings["bot.activity.count"] = new DynamicStringDataContainer.DynamicValue(() => _activitySwitcher?.Count ?? -1);
        _dynamicStrings["runtime.dotnet"] = Emzi0767.Utilities.RuntimeInformation.Version;
        _dynamicStrings["runtime.os"] = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
        _dynamicStrings["bot.strings.count"] = new DynamicStringDataContainer.DynamicValue(() => _dynamicStrings.Count);
        _dynamicStrings["bot.langs"] = new DynamicStringDataContainer.DynamicValue(() => Translator.GetAvailablesLanguages());
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

    private void ConfigureExtension()
    {
        var slash = _discordClient.UseSlashCommands();
        slash.RegisterCommands(typeof(Program).Assembly);

        slash.SlashCommandErrored += OnCommandErrored;
    }

    private async Task OnCommandErrored(SlashCommandsExtension sender, SlashCommandErrorEventArgs e)
    {
        DiscordEmbed errorEmbed = Embeder.CreateError(this, e.Exception, e.Context.User);
        try
        {
            await e.Context.CreateResponseAsync(errorEmbed, true);
        }
        catch (BadRequestException)
        {
            if (e.Context.Channel.Type == ChannelType.Text) {
                await e.Context.Channel.SendMessageAsync(errorEmbed);
            }
        }

        e.Handled = true;
    }

    private void ConfigureHandlers()
    {
        _discordClient.Ready += DiscordClientOnReady;
        _discordClient.Heartbeated += DiscordClientOnHeartbeated;
    }

    private async Task DiscordClientOnHeartbeated(DiscordClient sender, HeartbeatEventArgs e)
    {
        await _activitySwitcher.Switch();
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
        
        _logger.Information("SlashBib was reloaded.");

        //TODO: Reload
        await Task.CompletedTask;
    }
    
    public async Task RunAsync()
    {
        _logger.Information("Running SlashBib {Version} with.NET {RuntimeVersion} with {OperatingSystem}"
        , typeof(Program).Assembly.GetName().Version
        , Emzi0767.Utilities.RuntimeInformation.Version
        , RuntimeInformation.RuntimeIdentifier);


        if (IsDebug)
        {
            _logger.Debug("SlashBib is running on debug mode");
            _logger.Debug("Strings: {strings}", _dynamicStrings);
        }
        
        await _discordClient.ConnectAsync();

        await Task.Delay(-1);
    }

    public SlashDiscordEmbedBuilder GetEmbed()
        => _configuration.DefaultEmbed == null
            ? new SlashDiscordEmbedBuilder()
            : new SlashDiscordEmbedBuilder(_configuration.DefaultEmbed);

    public static SlashBibBot Create(string configFilename)
    {
        return new SlashBibBot(SlashConfiguration.FromFile(configFilename));
    }
}