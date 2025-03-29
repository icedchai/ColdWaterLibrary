namespace ColdWaterLibrary.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using ColdWaterLibrary.Enums;
    using Exiled.API.Extensions;
    using Exiled.CustomRoles.API.Features;
    using Exiled.Loader;
    using PlayerRoles;

    /// <summary>
    /// Encapsulates a role type from one of three systems (Uncomplicated Custom Roles, Exiled Custom Roles, Base Game Roles).
    /// </summary>
    public struct OverallRoleType
    {
        #region operators
        public static bool operator ==(RoleTypeId left, OverallRoleType right)
        {
            return right == left;
        }

        public static bool operator !=(RoleTypeId left, OverallRoleType right)
        {
            return !(right == left);
        }

        public static bool operator ==(OverallRoleType left, RoleTypeId right)
        {
            if (left.RoleType != TypeSystem.BaseGame)
            {
                return false;
            }

            if (left.RoleId != (int)right)
            {
                return false;
            }

            return true;
        }

        public static bool operator !=(OverallRoleType left, RoleTypeId right)
        {
            return !(left == right);
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
        /// <param name="roleTypeId">The <see cref="RoleTypeId"/> to convert.</param>
        public OverallRoleType(RoleTypeId roleTypeId)
        {
            this.RoleType = TypeSystem.BaseGame;
            this.RoleId = (int)roleTypeId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OverallRoleType"/> struct.
        /// </summary>
        /// <param name="customRole">The <see cref="CustomRole"/> to convert.</param>
        public OverallRoleType(CustomRole customRole)
        {
            this.RoleType = TypeSystem.ExiledCustom;
            this.RoleId = (int)customRole.Id;
        }

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
