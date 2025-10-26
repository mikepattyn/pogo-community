using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Bot.Service.Application.Interfaces;
using Bot.Service.Application.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Bot.Service.Application.Services;

public class DiscordBotService : BackgroundService
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly InteractionService _interactions;
    private readonly IServiceProvider _services;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DiscordBotService> _logger;
    private readonly IBotBffClient _botBffClient;

    public DiscordBotService(
        IServiceProvider services,
        IConfiguration configuration,
        ILogger<DiscordBotService> logger,
        IBotBffClient botBffClient)
    {
        _services = services;
        _configuration = configuration;
        _logger = logger;
        _botBffClient = botBffClient;

        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds | 
                           GatewayIntents.GuildMessages | 
                           GatewayIntents.GuildMessageReactions |
                           GatewayIntents.GuildMembers |
                           GatewayIntents.MessageContent
        };

        _client = new DiscordSocketClient(config);
        _commands = new CommandService();
        _interactions = new InteractionService(_client.Rest);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _client.Log += LogAsync;
        _commands.Log += LogAsync;

        // Register event handlers
        _client.Ready += OnReadyAsync;
        _client.MessageReceived += OnMessageReceivedAsync;
        _client.ReactionAdded += OnReactionAddedAsync;
        _client.ReactionRemoved += OnReactionRemovedAsync;
        _client.UserJoined += OnUserJoinedAsync;

        // Register command modules
        await _commands.AddModulesAsync(typeof(Program).Assembly, _services);
        await _interactions.AddModulesAsync(typeof(Program).Assembly, _services);

        var token = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN");
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogError("DISCORD_BOT_TOKEN environment variable is not set");
            throw new InvalidOperationException("DISCORD_BOT_TOKEN environment variable is required");
        }

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        _logger.LogInformation("Discord bot started successfully");

        // Keep the service running
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task OnReadyAsync()
    {
        _logger.LogInformation($"Bot logged in as {_client.CurrentUser?.Username}#{_client.CurrentUser?.Discriminator}");
        
        // Register slash commands
        await _interactions.RegisterCommandsGloballyAsync();
    }

    private async Task OnMessageReceivedAsync(SocketMessage message)
    {
        if (message is not SocketUserMessage userMessage || message.Author.IsBot)
            return;

        var context = new SocketCommandContext(_client, userMessage);
        
        // Handle text commands
        int argPos = 0;
        if (userMessage.HasCharPrefix('!', ref argPos))
        {
            await _commands.ExecuteAsync(context, argPos, _services);
        }

        // Handle interactions
        if (userMessage.HasMentionPrefix(_client.CurrentUser, ref argPos))
        {
            await _interactions.ExecuteCommandAsync(context, _services);
        }
    }

    private async Task OnReactionAddedAsync(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
    {
        if (reaction.User.Value?.IsBot == true)
            return;

        // Handle raid reactions
        var allowedEmojisRaid = new[] { "👍" };
        var allowedEmojisRaidExtra = new[] { "1⃣", "2⃣", "3⃣", "4⃣", "5⃣", "6⃣", "7⃣", "8⃣", "9⃣" };
        var allowedEmojisRank = new[] { "🔴", "🔵", "🟡", "⚪" };

        var emojiName = reaction.Emote.Name;
        
        if (allowedEmojisRaid.Contains(emojiName))
        {
            // Handle joining raid
            _logger.LogInformation($"User {reaction.User.Value.Username} joined raid");
        }
        else if (allowedEmojisRaidExtra.Contains(emojiName))
        {
            // Handle adding extra players
            _logger.LogInformation($"User {reaction.User.Value.Username} added extra players");
        }
        else if (allowedEmojisRank.Contains(emojiName))
        {
            // Handle rank selection
            _logger.LogInformation($"User {reaction.User.Value.Username} selected rank");
        }
    }

    private async Task OnReactionRemovedAsync(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
    {
        if (reaction.User.Value?.IsBot == true)
            return;

        // Handle leaving raid
        _logger.LogInformation($"User {reaction.User.Value.Username} left raid");
    }

    private async Task OnUserJoinedAsync(SocketGuildUser user)
    {
        _logger.LogInformation($"New user joined: {user.Username}");

        // Create player in database
        var player = new CreatePlayerDto
        {
            DiscordId = user.Id.ToString(),
            DateJoined = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss"),
            FirstName = null,
            Nickname = user.DisplayName,
            Level = null,
            Team = null
        };

        try
        {
            await _botBffClient.CreatePlayerAsync(player);
            _logger.LogInformation($"Created player for Discord user {user.Username}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to create player for Discord user {user.Username}");
        }
    }

    private Task LogAsync(LogMessage log)
    {
        _logger.Log(log.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Debug,
            LogSeverity.Debug => LogLevel.Debug,
            _ => LogLevel.Information
        }, log.Message);

        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Discord bot...");
        await _client.StopAsync();
        await _client.LogoutAsync();
        await base.StopAsync(cancellationToken);
    }
}