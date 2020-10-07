using StardewValley;
using StardewValley.Locations;

namespace SmallerDeathPenalty
{
    internal class PlayerStateSaver
    {
        public static PlayerState state;

        // Saves player's current funds
        public static void Save()
        {
            state = new PlayerState(Game1.player.Money);
        }

        //Load funds
        public static void Load()
        {
            Game1.player.Money = state.money;
        }
    }
}
