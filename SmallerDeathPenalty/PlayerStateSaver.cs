using StardewValley;
using System;

namespace SmallerDeathPenalty
{
    /// <summary>
    /// Contains the playerstate for saving and loading
    /// </summary>
    internal class PlayerStateSaver
    {
        public static PlayerState state;

        private static ModConfig config;

        public static void SetConfig(ModConfig config)
        {
            PlayerStateSaver.config = config;
        }

        // Saves player's current state for use later
        public static void Save()
        {
            state = new PlayerState(Game1.player.Money);
        }

        //Load Player state
        public static void Load()
        {
           
            //Is lost percentage of funds more than capped amount?
            if (state.money * (1 - config.MoneytoRestorePercentage) > config.MoneyLossCap)
            {
                //Yes, restore money minus capped amount
                Game1.player.Money = (int)state.money - config.MoneyLossCap;
            }
            else
            {
                //No, restore specified percentage of money
                Game1.player.Money = (int)Math.Round(state.money * config.MoneytoRestorePercentage);
            }

            //Restore stamina
            Game1.player.stamina = (int)(Game1.player.maxStamina * config.EnergytoRestorePercentage);

            //Restore health
            Game1.player.health = (int)(Game1.player.maxHealth * config.HealthtoRestorePercentage);

            //Restore items
            if (config.RestoreItems == true)
            {
                foreach (Item item in Game1.player.itemsLostLastDeath)
                {
                    Game1.player.addItemToInventory(item);
                }
                //Clears items lost, prevents being purchasable at Guild
                Game1.player.itemsLostLastDeath.Clear();
            }
        }
    }
}
