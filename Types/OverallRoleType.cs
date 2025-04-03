namespace ColdWaterLibrary.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using ColdWaterLibrary.Enums;
    using ColdWaterLibrary.Extensions;
    using ColdWaterLibrary.Integration;
    using Exiled.API.Extensions;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomRoles.API.Features;
    using Exiled.Loader;
    using PlayerRoles;
    using UncomplicatedCustomRoles.API.Interfaces;

    /// <summary>
    /// Encapsulates a role type from one of three systems (Uncomplicated Custom Roles, Exiled Custom Roles, Base Game Roles).
    /// </summary>
    [Serializable]
    public struct OverallRoleType
    {
        #region operators
        public static implicit operator OverallRoleType(string x)
        {
            if (int.TryParse(x, out int i))
            {
                // checks custom roles that use int as ID.
                if (CustomRole.TryGet((uint)i, out CustomRole exiledCustomRole))
                {
                    return exiledCustomRole;
                }
                else
                {
                    MethodInfo tryGet = UncomplicatedIntegration.CustomRoleType.GetMethod("TryGet");
                    object[] parameters = new object[] { i, null };
                    if ((bool)tryGet.Invoke(null, parameters))
                    {
                        return (OverallRoleType)parameters[1];
                    }
                }

                // As last resort, parse enum for roletypeid
                if (Enum.TryParse<RoleTypeId>(x, out RoleTypeId roleTypeId))
                {
                    return roleTypeId;
                }
                else
                {
                    return new OverallRoleType(TypeSystem.Unknown, 0);
                }
            }

            IEnumerable<CustomRole> customRoles = CustomRole.Registered.Where(r => r.Name.ToLower() == x.ToLower());
            if (customRoles.Any())
            {
                return customRoles.First();
            }

            // TODO:
            // Finish UCR search w/ reflection.
            /*
            if (!UncomplicatedIntegration.IsUcrLoaded)
            {
                return new OverallRoleType(TypeSystem.Unknown, 0);
            }

            PropertyInfo ucrListInfo = UncomplicatedIntegration.CustomRoleType.GetProperty("List");
            PropertyInfo nameInfo = UncomplicatedIntegration.ICustomRoleInterface.GetProperty("Name");
            PropertyInfo idInfo = UncomplicatedIntegration.ICustomRoleInterface.GetProperty("Id");
            var uCustomRoles = (List<object>)ucrListInfo.GetValue(null);
            uCustomRoles = uCustomRoles.Where(w => (string)nameInfo.GetValue(w) == x).ToList();
            if (uCustomRoles.Any())
            {
                return new OverallRoleType(TypeSystem.Uncomplicated, (int)idInfo.GetValue(uCustomRoles.First()));
            }
            */
            return new OverallRoleType(TypeSystem.Unknown, 0);
        }

        public static implicit operator OverallRoleType(RoleTypeId x)
        {
            return new OverallRoleType(TypeSystem.BaseGame, (int)x);
        }

        public static implicit operator OverallRoleType(CustomRole x)
        {
            return new OverallRoleType(TypeSystem.ExiledCustom, (int)x.Id);
        }

        public static bool operator ==(OverallRoleType left, OverallRoleType right)
        {
            if (left.RoleType != right.RoleType)
            {
                return false;
            }

            if (left.RoleId != right.RoleId)
            {
                return false;
            }

            return true;
        }

        public static bool operator !=(OverallRoleType left, OverallRoleType right)
        {
            return !(left == right);
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="OverallRoleType"/> struct.
        /// </summary>
        /// <param name="id">The ID of the role.</param>
        /// <param name="roleVersion">The <see cref="TypeSystem"> of the role.</param>
        public OverallRoleType(TypeSystem roleVersion, int id)
        {
            this.RoleType = roleVersion;
            this.RoleId = id;
        }

        /// <summary>
        /// Gets or sets the ID for role in its respective system.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Gets or sets the role system the role originates from.
        /// </summary>
        public TypeSystem RoleType { get; set; }

        /// <summary>
        /// Gets the name of the underlying role.
        /// </summary>
        /// <returns>For <see cref="TypeSystem.BaseGame"/>: <see cref="RoleTypeId"/> : GetFullName.
        /// <para></para>For <see cref="TypeSystem.ExiledCustom"/>: <see cref="CustomRole"/> : Name.
        /// <para></para>For <see cref="TypeSystem.Uncomplicated"/>: ICustomRole : Name.
        /// <para></para>or <see cref="null"/>.</returns>
        public readonly string GetName()
        {
            switch (RoleType)
            {
                case TypeSystem.ExiledCustom:
                    if (!CustomRole.TryGet((uint)RoleId, out CustomRole exiledCustomRole))
                    {
                        return null;
                    }

                    return exiledCustomRole.Name;
                case TypeSystem.BaseGame:
                    RoleTypeId roleTypeId;
                    if (!Enum.TryParse(RoleId.ToString(), out roleTypeId))
                    {
                        return null;
                    }

                    return roleTypeId.GetFullName();

                // TODO: Add UCR name getter.
                case TypeSystem.Uncomplicated:
                    Assembly UcrAssembly = Loader.Plugins.FirstOrDefault(p => p.Name is "UncomplicatedCustomRoles")?.Assembly;
                    if (UcrAssembly is null)
                    {
                        return null;
                    }

                    Type customrtype = UcrAssembly.GetType("UncomplicatedCustomRoles.API.Features.CustomRole");
                    Type icustomrtype = UcrAssembly.GetType("UncomplicatedCustomRoles.API.Interfaces.ICustomRole");
                    PropertyInfo nameInfo = icustomrtype.GetProperty("Name");
                    MethodInfo summonedCustomRoleGet = customrtype.GetMethod("TryGet", new Type[] { typeof(int), icustomrtype.MakeByRefType() });
                    object[] parameters = new object[] { RoleId, null };
                    if ((bool)summonedCustomRoleGet.Invoke(null, parameters))
                    {
                        return (string)nameInfo.GetValue(parameters[1]);
                    }

                    break;
            }

            return null;
        }

        /// <inheritdoc/>
        public override bool Equals(object o)
        {
            if (o is OverallRoleType roletype)
            {
                return this == roletype;
            }

            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
