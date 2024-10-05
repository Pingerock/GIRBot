using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using GIRBot.Helper;

namespace GIRBot.Commands
{
    public class Shoot : BaseCommandModule
    {
        private IConfiguration _config;

        public Shoot(IConfiguration config)
        {
            _config = config;
        }

        [Command("shoot")]
        public async Task Command(CommandContext ctx, DiscordUser target)
        {
            // Check that the target user is not null
            if (target == null)
            {
                await ctx.RespondAsync("You can't shoot something that doesn't exist.");
                return;
            }

            // Check that the target is not a bot
            if (target.IsBot)
            {
                await ctx.RespondAsync("Bots are not your enemies. Your enemies are people.");
                return;
            }

            // Check that the calling user has the gunner role
            bool hasGunnerRole = ctx.Member.Roles.Any(role => role.Name.StartsWith(_config["GunnerRoleName"]));
            if (!hasGunnerRole)
            {
                await ctx.RespondAsync("You are not a gunner to decide whose body your bullets will hit.");
                return;
            }

            int ammoCount = AmmoCounter.AmmoCount(ctx);

            // Check that the gunner has ammo
            if (ammoCount == 0)
            {
                await ctx.RespondAsync("Excuse me, gunner, but you have no ammunition.");
                return;
            }

            // Reduce the number of cartridges
            await ctx.Member.RevokeRoleAsync(RoleChecker.GetAmmoRole(ctx));
            DiscordRole newAmmoCount = AmmoCounter.GetAmmoRole(ctx, ammoCount - 1);
            if (newAmmoCount != null)
            {
                await ctx.Member.GrantRoleAsync(newAmmoCount);
            }

            DiscordMember targetMember = await ctx.Guild.GetMemberAsync(target.Id);

            var adminRole = ctx.Guild.Roles.Values.FirstOrDefault(r => r.Name == _config["AdminName"]);
            if (targetMember.Roles.Contains(adminRole))
            {
                await ctx.RespondAsync("A brave move, but useless. You wasted a bullet.");
                return;
            }

            // Check existing damage roles on target user
            DiscordRole targetDamageRole = RoleChecker.GetDamageRole(ctx, _config, targetMember);
            if (targetDamageRole != null)
            {
                int targetDamageIndex = DamageCounter.GetDamageIndex(_config, targetDamageRole);
                DiscordRole newTargetDamageRole = DamageCounter.GetRoleByIndex(_config, ctx, targetDamageIndex + 1);
                if (newTargetDamageRole == targetDamageRole)
                {
                    await targetMember.BanAsync();
                    await ctx.RespondAsync($"{target.Mention} banned forever.");
                }
                else
                {
                    await targetMember.RevokeRoleAsync(targetDamageRole);
                    await targetMember.GrantRoleAsync(newTargetDamageRole);
                    await targetMember.TimeoutAsync(DateTime.Now + DamageCounter.GetTimeoutByIndex(_config, ctx, targetDamageIndex + 1));
                    await ctx.RespondAsync($"{target.Mention}'s wounds grew more severe.");
                }
            }
            else
            {
                DiscordRole newTargetDamageRole = DamageCounter.GetRoleByIndex(_config, ctx, 0);
                await targetMember.GrantRoleAsync(newTargetDamageRole);
                await targetMember.TimeoutAsync(DateTime.Now + DamageCounter.GetTimeoutByIndex(_config, ctx, 0));
                await ctx.RespondAsync($"{target.Mention} felt his own blood on himself for the first time.");
            }

            await Logger.CreateLog(_config, ctx, $"User {ctx.Member.Mention} shot at user {target.Mention}.");
        }
    }
}
