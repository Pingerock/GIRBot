using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GIRBot.Helper
{
    public static class RoleChecker
    {
        // Gains the role of the gunners's ammo supply
        public static DiscordRole GetAmmoRole(CommandContext ctx)
        {
            // Check that the user executing the command has ammo
            var ammoRole = ctx.Member.Roles.Where(role => role.Name.StartsWith("Боезапас")).ToList();

            if (ammoRole.Count == 0)
            {
                return null;
            }

            return ammoRole.First();
        }

        // Gains the role of the gunners's ammo supply
        public static DiscordRole GetAmmoRole(DiscordMember target)
        {
            // Check that the user executing the command has ammo
            var ammoRole = target.Roles.Where(role => role.Name.StartsWith("Боезапас")).ToList();

            if (ammoRole.Count == 0)
            {
                return null;
            }

            return ammoRole.First();
        }

        // Get the user's damage role
        public static DiscordRole GetDamageRole(CommandContext ctx, IConfiguration config, DiscordMember target)
        {
            var damageRole = target.Roles.Where(role => GetDamageRoles(config).ContainsKey(role.Name)).ToList();

            if (damageRole.Count == 0)
            {
                return null;
            }

            return damageRole.First();
        }

        // Creates a dictionary of damages (timeouts)
        public static Dictionary<string, TimeSpan> GetDamageRoles(IConfiguration config)
        {
            Dictionary<string, TimeSpan> damageRoles = new Dictionary<string, TimeSpan>();

            List<string> damageRoleNames = config.GetSection("DamageRoles").Get<List<string>>();

            if (damageRoleNames.Count == 0 || damageRoleNames == null)
            {
                Console.WriteLine("Warning! Missing damage gradation roles. Add 'DamageRoles' section to appsettings.json file in bot folder.");
                return null;
            }

            List<int> banDurations = config.GetSection("DamageDurations").Get<List<int>>();

            if (banDurations.Count == 0 || banDurations == null)
            {
                Console.WriteLine("Warning! Missing timeout list. Add 'DamageDurations' section to appsettings.json file in bot folder.");
                return null;
            }

            if (damageRoleNames.Count != banDurations.Count)
            {
                Console.WriteLine("Warning! The number of damage roles does not match the amount of timeout time. Correct the appsettings.json file in the bot folder.");
                return null;
            }

            for (int i = 0; i < damageRoleNames.Count; i++)
            {
                if (i == damageRoleNames.Count - 1)
                {
                    damageRoles.Add(damageRoleNames[i], TimeSpan.MaxValue);
                    break;
                }

                damageRoles.Add(damageRoleNames[i], TimeSpan.FromMinutes(banDurations[i]));
            }

            return damageRoles;
        }
    }
}
