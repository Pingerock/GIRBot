using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using GIRBot.Commands;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace GIRBot.Helper
{
    public static class Reloader
    {
        public async static Task ReloadCheck(IConfiguration config, DiscordClient discordClient)
        {
            // Extract values ​​from config
            int reloadTime = config.GetValue<int>("ReloadTime");
            DateTime lastReloadDate = config.GetValue<DateTime>("LastReloadDate");

            // Current date
            DateTime currentDate = DateTime.Now;

            // Calculate next reload date
            DateTime nextReloadDate = lastReloadDate.AddDays(reloadTime);

            await PerformReload(config, discordClient);

            // Update last reload date in config
            ConfigEditor.UpdateReloadDate(config);

            nextReloadDate = currentDate.AddDays(reloadTime);


            // Create a timer for the next reload
            TimeSpan timeUntilNextReload = nextReloadDate - currentDate;
            Timer timer = new Timer(_ => Task.Run(() => PerformReload(config, discordClient)), null, timeUntilNextReload, Timeout.InfiniteTimeSpan);
        }

        private static async Task PerformReload(IConfiguration config, DiscordClient discordClient)
        {
            // Insert one of admins userId so the bot could perform a reload by typing command "!reload"
            ulong userId = 0;
            var user = await discordClient.GetUserAsync(userId);

            // Insert channelId from your discord server so the bot could perform a reload by typing command "!reload"
            ulong channelId = 0;
            var channel = await discordClient.GetChannelAsync(channelId);

            var commands = discordClient.GetCommandsNext();
            var ctx = commands.CreateFakeContext(user, channel, "reload", "!", null);

            Reload reloadForce = new Reload(config);
            await reloadForce.Command(ctx);
        }
    }
}