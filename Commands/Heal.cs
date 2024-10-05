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
    public class Heal : BaseCommandModule
    {
        private IConfiguration _config;

        public Heal(IConfiguration config)
        {
            _config = config;
        }

        [Command("heal")]
        public async Task Command(CommandContext ctx, DiscordUser target)
        {
            // Checking the Administrator role
            var adminRole = ctx.Guild.Roles.Values.FirstOrDefault(r => r.Name == _config["AdminName"]);
            if (!ctx.Member.Roles.Contains(adminRole))
            {
                await ctx.RespondAsync("You don't have permissions for this command!");
                return;
            }

            DiscordMember targetMember = await ctx.Guild.GetMemberAsync(target.Id);

            DiscordRole targetDamageRole = RoleChecker.GetDamageRole(ctx, _config, targetMember);

            if (targetDamageRole == null)
            {
                await ctx.RespondAsync("This user has no damages.");
                return;
            }

            int targetDamageRoleIndex = DamageCounter.GetDamageIndex(_config, targetDamageRole);

            if (targetDamageRoleIndex >= 0)
            {
                await targetMember.RevokeRoleAsync(targetDamageRole);
                await targetMember.GrantRoleAsync(DamageCounter.GetRoleByIndex(_config, ctx, targetDamageRoleIndex - 1));
                await ctx.RespondAsync($"{targetMember.Mention} was healed and his injury severity was reduced.");
                await Logger.CreateLog(_config, ctx, $"User {ctx.Member.Mention} used the !heal command on the user {targetMember.Mention}");
            }
            else
            {
                await ctx.RespondAsync($"{targetMember.Mention} healthy and does not need treatment.");
            }
        }
    }
}
