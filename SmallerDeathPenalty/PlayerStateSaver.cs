using StardewValley;
using System;
using StardewValley.Locations;

namespace SmallerDeathPenalty
{
    internal class PlayerStateSaver
    {
        public static PlayerState state;

        // Saves player's current funds, items and health for use later
        public static void Save()
        {
            state = new PlayerState(Game1.player.Money, Game1.player.health);
        }

        //Reloads state
        public static void Load()
        {
            //Is the player's money greater than 10,000g
            if(state.money > 10000)
            {
                //Yes, restore money minus 500g
                Game1.player.Money = (int)state.money - 500;
            }
            else
            {
                //No, rstore 95% of money
                Game1.player.Money = (int)Math.Round(state.money * 0.95);
            }
            //Restore half health
            Game1.player.health = Game1.player.maxHealth / 2;
        }
    }
}
