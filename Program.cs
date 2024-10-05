using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.Extensions.Configuration;
using DSharpPlus.EventArgs;
using DSharpPlus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using GIRBot.Commands;
using GIRBot.Helper;

namespace GIRBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();
            var config = serviceProvider.GetService<IConfiguration>();

            var discord = new DiscordClient(new DiscordConfiguration
            {
                Token = config["Token"],
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug,
                Intents = DiscordIntents.All 
            });

            var commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { config["Prefix"] },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false,
                Services = serviceProvider
            });

            commands.RegisterCommands<Heal>();
            commands.RegisterCommands<Reload>();
            commands.RegisterCommands<ReloadForce>();
            commands.RegisterCommands<Shoot>();
            commands.RegisterCommands<Unload>();

            await discord.ConnectAsync();
            Task.Run(() => Reloader.ReloadCheck(config, discord));
            await Task.Delay(-1);
        }

        private static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            return services;
        }

        private static Task OnReady(DiscordClient sender, ReadyEventArgs e)
        {
            sender.Logger.LogInformation("Bot is connected and ready to work!");
            return Task.CompletedTask;
        }
    }
}

