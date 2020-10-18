using StardewValley;
using System;
using StardewValley.Locations;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

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

        //Reloads state
        public static void Load()
        {
            //Use default values if config has invalid values
            if(config.RestoreMoneyPercentage > 1 || config.RestoreMoneyPercentage < 0)
            {
                if (state.money * 0.95 > 500)
                {
                    //Yes, restore money minus capped amount
                    Game1.player.Money = (int)state.money - 500;
                }
                else
                {
                    //No, restore specified percentage of money
                    Game1.player.Money = (int)Math.Round(state.money * 0.95);
                }
            }

            else
            {
                //Is lost percentage of funds more than capped amount?
                if (state.money * (1 - config.RestoreMoneyPercentage) > config.MoneyLossCap)
                {
                    //Yes, restore money minus capped amount
                    Game1.player.Money = (int)state.money - config.MoneyLossCap;
                }
                else
                {
                    //No, restore specified percentage of money
                    Game1.player.Money = (int)Math.Round(state.money * config.RestoreMoneyPercentage);
                }
            }

            //Restore stamina
            //Use default values if config has invalid values
            if (config.EnergytoRestorePercentage > 1 || config.EnergytoRestorePercentage < 0)
            {
                Game1.player.stamina = (int)(Game1.player.maxStamina * 0.10);
            }
            //Use config values
            else
            {
                //Restore some energy
                Game1.player.stamina = (int)(Game1.player.maxStamina * config.EnergytoRestorePercentage);
            }

            //Restore health
            //Use default values if config has invalid values
            if (config.HealthtoRestorePercentage > 1 || config.HealthtoRestorePercentage < 0)
            {
                Game1.player.health = (int)(Game1.player.maxHealth * 0.50);
            }
            //Use config values
            else
            {
                //Restore some health
                Game1.player.health = (int)(Game1.player.maxHealth * config.HealthtoRestorePercentage);
            }

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
