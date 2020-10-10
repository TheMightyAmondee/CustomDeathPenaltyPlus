using StardewValley;
using System;
using StardewValley.Locations;

namespace SmallerDeathPenalty
{
    internal class PlayerStateSaver
    {
        public static PlayerState state;

        // Saves player's current funds and health for use later
        public static void Save()
        {
            state = new PlayerState(Game1.player.Money, Game1.player.health);
        }

        //Load funds (discounted), restore 95% of player's money
        public static void LoadDiscounted()
        {
            Game1.player.Money =  (int)Math.Round(state.money * 0.95);
            //Restore half the players health
            Game1.player.health = Game1.player.maxHealth / 2;
        }

        //Load funds (capped), restore player's money, minus 500g
        public static void LoadCapped()
        {
            Game1.player.Money = (int)state.money - 500;
            //Restore half players health
            Game1.player.health = Game1.player.maxHealth / 2;
        }
    }
}
