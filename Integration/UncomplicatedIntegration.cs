namespace ColdWaterLibrary.Integration
{
    using Exiled.Loader;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    public static class UncomplicatedIntegration
    {
        private static bool triedLoadingUcr = false;

        /// <summary>
        /// The UncomplicatedCustomRoles assembly.
        /// </summary>
        public static Assembly UcrAssembly { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether UncomplicatedCustomRoles assembly has been loaded.
        /// </summary>
        public static bool IsUcrLoaded
        {
            get
            {
                if (!triedLoadingUcr)
                {
                    ReflectUncomplicated();
                    triedLoadingUcr = true;
                }

                return UcrAssembly is not null;
            }
        }

        /// <summary>
        /// Gets the reflected class UncomplicatedCustomRoles.API.Features.CustomRole.
        /// </summary>
        public static Type CustomRoleType { get; internal set; }
        /// <summary>
        /// Gets the reflected interface UncomplicatedCustomRoles.API.Interfaces.ICustomRole.
        /// </summary>
        public static Type ICustomRoleInterface { get; internal set; }

        /// <summary>
        /// Gets the reflected class UncomplicatedCustomRoles.Extensions.PlayerExtension.
        /// </summary>
        public static Type UcrPlayerExtensionType { get; internal set; }

        /// <summary>
        /// Gets the reflected class UncomplicatedCustomRoles.API.Features.SummonedCustomRole.
        /// </summary>
        public static Type SummonedCustomRoleType { get; internal set; }

        private static void ReflectUncomplicated()
        {
            UcrAssembly = Loader.Plugins.FirstOrDefault(p => p.Name is "UncomplicatedCustomRoles")?.Assembly;
            if (UcrAssembly is not null)
            {
                CustomRoleType = UcrAssembly.GetType("UncomplicatedCustomRoles.API.Features.CustomRole");
                ICustomRoleInterface = UcrAssembly.GetType("UncomplicatedCustomRoles.API.Interfaces.ICustomRole");
                UcrPlayerExtensionType = UcrAssembly.GetType("UncomplicatedCustomRoles.Extensions.PlayerExtension");
                SummonedCustomRoleType = UcrAssembly.GetType("UncomplicatedCustomRoles.API.Features.SummonedCustomRole");
            }
        }
    }
}
