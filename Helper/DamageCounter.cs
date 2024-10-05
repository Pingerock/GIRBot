using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIRBot.Helper
{
    public static class DamageCounter
    {
        public static int GetDamageIndex(IConfiguration config, DiscordRole targetDamageRole)
        {
            Dictionary<string, TimeSpan> damageRoles = RoleChecker.GetDamageRoles(config);

            int i = -1;
            foreach (KeyValuePair<string, TimeSpan> kvp in damageRoles)
            {
                if (kvp.Key == targetDamageRole.Name)
                {
                    i++;
                    break;
                }

                i++;
            }

            return i;
        }

        public static DiscordRole GetRoleByIndex(IConfiguration config, CommandContext ctx, int index)
        {
            if (index < 0)
            {
                return null;    
            }

            Dictionary<string, TimeSpan> damageRoles = RoleChecker.GetDamageRoles(config);

            DiscordRole newTargetDamageRole;
            if (index == 6)
            {
                newTargetDamageRole = ctx.Guild.Roles.Values.First(r => r.Name == damageRoles.ElementAt(5).Key);
            }
            else
            {
                newTargetDamageRole = ctx.Guild.Roles.Values.First(r => r.Name == damageRoles.ElementAt(index).Key);
            }

            return newTargetDamageRole;
        }

        public static TimeSpan GetTimeoutByIndex(IConfiguration config, CommandContext ctx, int index)
        {
            Dictionary<string, TimeSpan> damageRoles = RoleChecker.GetDamageRoles(config);

            TimeSpan damageDuration = damageRoles.ElementAt(index).Value;

            return damageDuration;
        }
    }
}
