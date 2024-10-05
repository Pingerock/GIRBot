using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GIRBot.Helper
{
    public static class AmmoCounter
    {
        public static bool AmmoCheck(CommandContext ctx)
        {
            var ammoRole = RoleChecker.GetAmmoRole(ctx);

            if (ammoRole == null)
            {
                return false;
            }

            return true;
        }

        public static int AmmoCount(CommandContext ctx)
        {
            var ammoRole = RoleChecker.GetAmmoRole(ctx);

            if (ammoRole == null)
            {
                return 0;
            }

            // Determine the number of rounds by the role name
            var ammoCount = int.Parse(ammoRole.Name.Split(' ')[1]);  // Example: "Ammo: 1" -> "1"

            return ammoCount;
        }

        public static int AmmoCount(DiscordMember target)
        {
            var ammoRole = RoleChecker.GetAmmoRole(target);

            if (ammoRole == null)
            {
                return 0;
            }

            // Determine the number of rounds by the role name
            var ammoCount = int.Parse(ammoRole.Name.Split(' ')[1]);  // "Боезапас: 1 патрон" -> "1"

            return ammoCount;
        }

        public static DiscordRole GetAmmoRole(CommandContext ctx, int currentAmmoCount)
        {
            var newAmmoRole = ctx.Guild.Roles.Values.Where(r => r.Name.StartsWith($"Боезапас: {currentAmmoCount} патрон")).ToList();

            if (!newAmmoRole.Any())
            {
                return null; 
            }

            return newAmmoRole.First();
        }

        public static DiscordRole GiveAmmo(CommandContext ctx, DiscordMember targetMember)
        {
            DiscordRole targetAmmoRole = RoleChecker.GetAmmoRole(targetMember);

            if (targetAmmoRole != null)
            {
                return GetAmmoRole(ctx, AmmoCount(targetMember) + 1);
            }

            return null;
        }

        // Распределение патронов
        public static async Task ReloadAmmo(IConfiguration config, CommandContext ctx)
        {
            DiscordRole gunnerRole = null;
            if (ctx.Guild != null)
            {
                gunnerRole = ctx.Guild.Roles.Values.FirstOrDefault(r => r.Name == config["GunnerRoleName"]);
            }
            else
            {
                var channelId = ctx.Channel.Id;
                var channel = await ctx.Client.GetChannelAsync(channelId);

                if (channel is DiscordChannel guildChannel)
                {
                    var guildId = guildChannel.GuildId;
                    var guild = await ctx.Client.GetGuildAsync(guildId.GetValueOrDefault());
                    gunnerRole = guild.Roles.Values.FirstOrDefault(r => r.Name == config["GunnerRoleName"]);
                }
            }

            Random random = new Random();
            int randMemberIndex;
            for (int i = 0; i < config.GetValue<int>("GunnersCount"); i++)
            {
                // Load all server participants
                var allMembers = await ctx.Guild.GetAllMembersAsync();

                // Filter members who have the 'Gunner' role
                List<DiscordMember> gunners = allMembers.Where(m => m.Roles.Contains(gunnerRole)).ToList();

                randMemberIndex = random.Next(0, gunners.Count);
                DiscordRole ammoCount = RoleChecker.GetAmmoRole(gunners[randMemberIndex]);
                if (ammoCount != null)
                {
                    DiscordRole newAmmoCount = AmmoCounter.GetAmmoRole(ctx, AmmoCounter.AmmoCount(gunners[randMemberIndex]) + 1);
                    if (newAmmoCount != null)
                    {
                        await gunners[randMemberIndex].RevokeRoleAsync(ammoCount);
                        await gunners[randMemberIndex].GrantRoleAsync(newAmmoCount);
                        await Logger.CreateLog(config, ctx, $"Gave a bullet to the user {gunners[randMemberIndex].Mention}");
                    }
                    else
                    {
                        await Logger.CreateLog(config, ctx, $"Gave a bullet to the user {gunners[randMemberIndex].Mention}, but it doesn't count. The ammunition is overfilled.");
                    }
                }
                else
                {
                    DiscordRole newAmmoCount = AmmoCounter.GetAmmoRole(ctx, 1);
                    await gunners[randMemberIndex].GrantRoleAsync(newAmmoCount);
                    await Logger.CreateLog(config, ctx, $"Gave a bullet to the user {gunners[randMemberIndex].Mention}");
                }
                
            }
        }
    }
}
