namespace ColdWaterLibrary.Types
{
    using ColdWaterLibrary.Enums;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Encapsulates an item type from one of three systems (Uncomplicated Custom Items, Exiled Custom Items, Base Game Items).
    /// </summary>
    public struct OverallItemType
    {
        /// <summary>
        /// Gets or sets the ID for item in its respective system.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Gets or sets the item system the role originates from.
        /// </summary>
        public TypeSystem RoleType { get; set; }
    }
}
