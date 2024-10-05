using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GIRBot.Helper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIRBot.Commands
{
    public class Reload : BaseCommandModule
    {
        private IConfiguration _config;

        public Reload(IConfiguration config)
        {
            _config = config;
        }

        [Command("reload")]
        public async Task Command(CommandContext ctx)
        {
            // Checking the Administrator role
            var adminRole = ctx.Guild.Roles.Values.FirstOrDefault(r => r.Name == _config["AdminName"]);
            if (!ctx.Member.Roles.Contains(adminRole))
            {
                await ctx.RespondAsync("You don't have permissions for this command!");
                return;
            }

            DateTime lastReloadDate = DateTime.Parse(_config["LastReloadDate"]);
            int reloadTime = int.Parse(_config["ReloadTime"]);
            if ((DateTime.Now - lastReloadDate).TotalDays >= reloadTime)
            {
                await AmmoCounter.ReloadAmmo(_config, ctx);
                ConfigEditor.UpdateReloadDate(_config);
                await ctx.RespondAsync("The ammunition has been successfully reload!");
                await Logger.CreateLog(_config, ctx, "The !reload command was executed successfully.");
            }
            else
            {
                await ctx.RespondAsync("It's too early to reload the ammo.");
            }
        }
    }
}
