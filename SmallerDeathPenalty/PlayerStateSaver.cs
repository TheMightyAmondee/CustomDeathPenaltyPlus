using StardewValley;
using System;
using StardewValley.Locations;
using System.Runtime.CompilerServices;

namespace SmallerDeathPenalty
{
    /// <summary>
    /// Saves the specified player state
    /// </summary>
    internal class PlayerStateSaver
    {
        public static PlayerState state;

        // Saves player's current state for use later
        public static void Save()
        {
            state = new PlayerState(Game1.player.Money);
        }

        //Reloads state
        public static void Load()
        {
            //Is the player's money greater than 10,000g?
            if(state.money > 10000)
            {
                //Yes, restore money minus 500g
                Game1.player.Money = (int)state.money - 500;
            }
            else
            {
                //No, restore 95% of money
                Game1.player.Money = (int)Math.Round(state.money * 0.95);
            }
            //Restore lost items
            foreach(Item item in Game1.player.itemsLostLastDeath)
            {
                Game1.player.addItemToInventory(item);
            }
            //Clears items lost, prevents being purchasable at Guild
            Game1.player.itemsLostLastDeath.Clear();
            //Restore half health
            Game1.player.health = Game1.player.maxHealth / 2;
            //Restore some energy
            Game1.player.stamina = 50;

        }
    }
}
