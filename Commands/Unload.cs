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
    // Command to remove ammunition from the user
    public class Unload : BaseCommandModule
    {
        private IConfiguration _config;

        public Unload(IConfiguration config)
        {
            _config = config;
        }

        [Command("unload")]
        public async Task Command(CommandContext ctx, DiscordUser target)
        {
            var adminRole = ctx.Guild.Roles.Values.FirstOrDefault(r => r.Name == _config["AdminName"]);
            if (!ctx.Member.Roles.Contains(adminRole))
            {
                await ctx.RespondAsync("You don't have permissions for this command!");
                return;
            }

            DiscordMember targetMember = await ctx.Guild.GetMemberAsync(target.Id);
            await targetMember.RevokeRoleAsync(RoleChecker.GetAmmoRole(targetMember));
            await ctx.RespondAsync($"{target.Mention} ran out of ammunition.");
            await Logger.CreateLog(_config, ctx, $"User {ctx.Member.Mention} used the !unload command on the user {targetMember.Mention}");
        }
    }
}
