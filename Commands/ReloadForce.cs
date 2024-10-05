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
    public class ReloadForce : BaseCommandModule
    {
        private IConfiguration _config;

        public ReloadForce(IConfiguration config)
        {
            _config = config;
        }

        [Command("reloadforce")]
        public async Task Command(CommandContext ctx)
        {
            // Checking the Administrator role
            var adminRole = ctx.Guild.Roles.Values.FirstOrDefault(r => r.Name == _config["AdminName"]);
            if (!ctx.Member.Roles.Contains(adminRole))
            {
                await ctx.RespondAsync("You don't have permissions for this command!");
                return;
            }

            await AmmoCounter.ReloadAmmo(_config, ctx);
            ConfigEditor.UpdateReloadDate(_config);
            await ctx.RespondAsync("The ammunition has been successfully reload!");
            await Logger.CreateLog(_config, ctx, "The !reloadforce command was executed successfully.");
        }
    }
}
