namespace ColdWaterLibrary.Features.Wrappers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ColdWaterLibrary.Features.Enums;

    /// <summary>
    /// Encapsulates an item type from one of three systems (Uncomplicated Custom Items, Exiled Custom Items, Base Game Items).
    /// </summary>
    [Serializable]
    public struct OverallItemType
    {
        #region operators
        public static implicit operator ItemType(OverallItemType x)
        {
            if (x.ItemTypeSystem != TypeSystem.BaseGame)
            {
                return ItemType.None;
            }

            if (!Enum.TryParse(x.ItemId.ToString(), false, out ItemType result))
            {
                return ItemType.None;
            }

            return result;
        }

        public static bool operator ==(OverallItemType left, OverallItemType right)
        {
            if (left.ItemId != right.ItemId)
            {
                return false;
            }

            if (left.ItemTypeSystem != right.ItemTypeSystem)
            {
                return false;
            }

            return true;
        }

        public static bool operator !=(OverallItemType left, OverallItemType right)
        {
            return !(left == right);
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="OverallItemType"/> struct.
        /// </summary>
        /// <param name="typeSystem">The <see cref="TypeSystem"/> to use.</param>
        /// <param name="id">The ID of the item to use.</param>
        public OverallItemType(TypeSystem typeSystem, int id)
        {
            this.ItemTypeSystem = typeSystem;
            this.ItemId = id;
        }

        /// <summary>
        /// Gets or sets the ID for item in its respective system.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Gets or sets the item system the role originates from.
        /// </summary>
        public TypeSystem ItemTypeSystem { get; set; }
    }
}
