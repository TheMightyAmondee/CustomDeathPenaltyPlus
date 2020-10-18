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
            //Use respective default values if config has invalid values
            if(config.RestoreMoneyPercentage > 1 || config.RestoreMoneyPercentage < 0 || config.MoneyLossCap < 0 || config.EnergytoRestorePercentage > 1 || config.EnergytoRestorePercentage <= 0 || config.HealthtoRestorePercentage > 1 || config.HealthtoRestorePercentage <= 0)
            {
                if(config.RestoreMoneyPercentage > 1 || config.RestoreMoneyPercentage < 0)
                {
                    config.RestoreMoneyPercentage = 0.95;
                }
                else if(config.MoneyLossCap < 0)
                {
                    config.MoneyLossCap = 500;
                }
                else if (config.EnergytoRestorePercentage > 1 || config.EnergytoRestorePercentage <= 0)
                {
                    config.EnergytoRestorePercentage = 0.10;
                }
                else if(config.HealthtoRestorePercentage > 1 || config.HealthtoRestorePercentage <= 0)
                {
                    config.HealthtoRestorePercentage = 0.50;
                }
            }

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
