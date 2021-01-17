using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;

namespace DiscordAntiToxicBot.Services
{
    public class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;
        private readonly ToxicLangService _ToxicService;

        // Dependency Injection will fill this value in for us
        //public ToxicLangService ToxicLangService { get; set; }

        public CommandHandlingService(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _ToxicService = services.GetRequiredService<ToxicLangService>();
            _services = services;

            // Hook CommandExecuted to handle post-command-execution logic.
            _commands.CommandExecuted += CommandExecutedAsync;
            // Hook MessageReceived so we can process each message to see
            // if it qualifies as a command.
            _discord.MessageReceived += MessageReceivedAsync;
            _discord.MessageReceived += RawMessageReceivedAsync;
        }

        public async Task RawMessageReceivedAsync(SocketMessage rawMessage)
        {
            // Ignore system messages, or messages from other bots
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;
            var context = new SocketCommandContext(_discord, message);
            
            var resultString = _ToxicService.CheckTextAsync(message.Content);

            JObject jresponse = JObject.Parse(resultString);

            var toxic               = Math.Round(jresponse["results"][0]["predictions"]["toxic"].Value<float>(), 3);
            var severe_toxic   = Math.Round(jresponse["results"][0]["predictions"]["severe_toxic"].Value<float>(), 3);
            var obscene         = Math.Round(jresponse["results"][0]["predictions"]["obscene"].Value<float>(), 3);
            var threat             = Math.Round(jresponse["results"][0]["predictions"]["threat"].Value<float>(), 3);
            var insult              = Math.Round(jresponse["results"][0]["predictions"]["insult"].Value<float>(), 3);
            var identity_hate = Math.Round(jresponse["results"][0]["predictions"]["identity_hate"].Value<float>(), 3);

            var acc = 0.00;
            if (
                    toxic               > acc &&
                    severe_toxic   > acc &&
                    obscene         > acc &&
                    threat             > acc &&
                    insult              > acc &&
                    identity_hate > acc
                )
            {
                // Check the config file if we are suppose remove the detected toxic speak
                if (bool.Parse(Config.ConfigFile["ToxicChatSettings"]["bRemoveToxicSpeach"]))
                {
                    await message.DeleteAsync();
                }
                if (bool.Parse(Config.ConfigFile["ToxicChatSettings"]["bPostToxicEmbedMsg"]))
                {
                    EmbedBuilder builder = new EmbedBuilder();
                    builder.Title = $"User {message.Author.Username} said something toxic!";
                    builder.Description = $"Toxic message: ||{message.Content}||";

                    builder.AddField("toxic: ", toxic, false);
                    builder.AddField("severe_toxic: ", severe_toxic, false);
                    builder.AddField("obscene: ", obscene, false);
                    builder.AddField("threat: ", threat, false);
                    builder.AddField("insult: ", insult, false);
                    builder.AddField("identity_hate: ", identity_hate, false);

                    builder.WithColor(Color.Red);
                    await context.Channel.SendMessageAsync("", false, builder.Build());
                }                
            }
        }

        public async Task InitializeAsync()
        {
            // Register modules that are public and inherit ModuleBase<T>.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // Ignore system messages, or messages from other bots
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            // This value holds the offset where the prefix ends
            var argPos = 0;
            // Perform prefix check. You may want to replace this with
            // (!message.HasCharPrefix('!', ref argPos))
            // for a more traditional command format like !help.
            if (!message.HasMentionPrefix(_discord.CurrentUser, ref argPos)) return;

            var context = new SocketCommandContext(_discord, message);
            // Perform the execution of the command. In this method,
            // the command service will perform precondition and parsing check
            // then execute the command if one is matched.
            await _commands.ExecuteAsync(context, argPos, _services); 
            // Note that normally a result will be returned by this format, but here
            // we will handle the result in CommandExecutedAsync,
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // command is unspecified when there was a search failure (command not found); we don't care about these errors
            if (!command.IsSpecified)
                return;

            // the command was successful, we don't care about this result, unless we want to log that a command succeeded.
            if (result.IsSuccess)
                return;

            // the command failed, let's notify the user that something happened.
            await context.Channel.SendMessageAsync($"error: {result}");
        }
    }
}
