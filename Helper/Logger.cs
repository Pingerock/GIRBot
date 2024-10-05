using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIRBot.Helper
{
    public static class Logger
    {
        public async static Task CreateLog(IConfiguration config, CommandContext ctx, string message)
        {
            var logChannel = ctx.Guild.Channels.Values.FirstOrDefault(c => c.Name == config["LogChannelName"]);
            if (logChannel != null)
            {
                await logChannel.SendMessageAsync(message);
            }
            else
            {
                await ctx.RespondAsync("Attention! I can't log the message. Check the config file.");
            }
        }
    }
}
