namespace ColdWaterLibrary.Core.Features.Enums
{
    /// <summary>
    /// Represents what system a given thing comes from.
    /// </summary>
    public enum TypeSystem
    {
        /// <summary>
        /// Unknown origin.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Created by Uncomplicated Custom Server team.
        /// </summary>
        Uncomplicated = 1,

        /// <summary>
        /// Created for the EXILED API.
        /// </summary>
        ExiledCustom = 2,

        /// <summary>
        /// Created by Northwood as part of basegame.
        /// </summary>
        BaseGame = 3,

        UcrRole = Uncomplicated,
        CrRole = ExiledCustom,
        BaseGameRole = BaseGame,
    }
}
