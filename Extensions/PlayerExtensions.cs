namespace ColdWaterLibrary.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using ColdWaterLibrary.Enums;
    using ColdWaterLibrary.Integration;
    using ColdWaterLibrary.Types;
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.CustomRoles.API;
    using Exiled.CustomRoles.API.Features;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Loader;
    using PlayerRoles;

    /// <summary>
    /// <see cref="Player"/> extensions for ColdWaterLib.
    /// </summary>
    public static class PlayerExtensions
    {
        public static OverallRoleType GetOverallRoleType(this Player player)
        {
            if (UncomplicatedIntegration.IsUcrLoaded)
            {
                MethodInfo summonedCustomRoleGet = UncomplicatedIntegration.SummonedCustomRoleType.GetMethod("Get", new Type[] { typeof(Player) });
                object ucrSumRole = summonedCustomRoleGet.Invoke(null, new object[] { player });
                if (ucrSumRole is not null)
                {
                    PropertyInfo ucrRoleProp = ucrSumRole.GetType().GetProperty("Role");
                    object ucrSumRoleRole = ucrRoleProp.GetValue(ucrSumRole);
                    PropertyInfo ucrRoleIdProp = ucrSumRoleRole.GetType().GetProperty("Id");
                    object ucrRoleId = ucrRoleIdProp.GetValue(ucrSumRoleRole);
                    if (ucrSumRole is not null)
                    {
                        return new OverallRoleType { RoleType = TypeSystem.Uncomplicated, RoleId = (int)ucrRoleId };
                    }
                }
            }

            // Put Exiled CustomRoles here
            if (!player.GetCustomRoles().IsEmpty())
            {
                return new OverallRoleType { RoleType = TypeSystem.ExiledCustom, RoleId = (int)player.GetCustomRoles()[0].Id };
            }

            return new OverallRoleType { RoleType = TypeSystem.BaseGame, RoleId = (int)player.Role.Type };
        }

        public static bool HasOverallRoleType(this Player player, OverallRoleType roleType)
        {
            switch (roleType.RoleType)
            {
                case TypeSystem.Uncomplicated:

                    if (!UncomplicatedIntegration.IsUcrLoaded)
                    {
                        Log.Info("At PlayerExtensions.HasOverallRole, UCR is NOT installed and the code for checking UCR roles will therefore not run.");
                        return false;
                    }

                    MethodInfo summonedCustomRoleGet = UncomplicatedIntegration.SummonedCustomRoleType.GetMethod("Get", new Type[] { typeof(Player) });
                    object ucrSumRole = summonedCustomRoleGet.Invoke(null, new object[] { player });
                    if (ucrSumRole is null) return false;
                    PropertyInfo ucrRoleProp = ucrSumRole.GetType().GetProperty("Role");
                    object ucrSumRoleRole = ucrRoleProp.GetValue(ucrSumRole);
                    PropertyInfo ucrRoleIdProp = ucrSumRoleRole.GetType().GetProperty("Id");
                    object ucrRoleId = ucrRoleIdProp.GetValue(ucrSumRoleRole);

                    return (int)ucrRoleId == roleType.RoleId;
                case TypeSystem.ExiledCustom:
                    if (player.GetCustomRoles().IsEmpty()) return false;
                    CustomRole.TryGet((uint)roleType.RoleId, out CustomRole cr);
                    return cr is not null && player.GetCustomRoles().Contains(cr);
                case TypeSystem.BaseGame:
                    if (!Enum.TryParse($"{roleType.RoleId}", out RoleTypeId roleid) || !player.GetCustomRoles().IsEmpty()) return false;
                    return player.Role.Type == roleid;
                default: return false;
            }
        }

        public static bool HasOverallRoleType(this Player player, List<OverallRoleType> roleType)
        {
            foreach (OverallRoleType role in roleType)
            {
                if (player.HasOverallRoleType(role))
                {
                    return true;
                }
            }

            return false;
        }

        public static void SetOverallRoleType(this Player player, OverallRoleType roleType)
        {
            switch (roleType.RoleType)
            {
                case TypeSystem.BaseGame:
                    if (!Enum.TryParse(roleType.RoleId.ToString(), out RoleTypeId baserole))
                    {
                        Log.Error($"At PlayerExtensions.SetOverallRole: BaseGame Role of ID {roleType.RoleId} does not exist!");
                        return;
                    }

                    player.Role.Set(baserole);
                    break;
                case TypeSystem.Uncomplicated:
                    if (!UncomplicatedIntegration.IsUcrLoaded)
                    {
                        return;
                    }

                    MethodInfo setCustomRole = UncomplicatedIntegration.UcrPlayerExtensionType.GetMethod("SetCustomRole", new Type[] { typeof(Player), typeof(int) });
                    setCustomRole.Invoke(null, new object[] { player, roleType.RoleId });

                    // player.SetCustomRole(roleType.RoleId);
                    break;
                case TypeSystem.ExiledCustom:
                    CustomRole crRole;
                    CustomRole.TryGet((uint)roleType.RoleId, out crRole);
                    if (crRole is null)
                    {
                        Log.Error($"At PlayerExtensions.SetOverallRole: Custom Role of ID {roleType.RoleId} does not exist!");
                        return;
                    }

                    crRole.AddRole(player);
                    break;
            }
        }
    }
}
