using StardewValley;
using System;

namespace CustomDeathPenaltyPlus
{
    internal class PlayerStateRestorer
    {
        /// <summary>
        /// Tracks the players current money and amount of money that will be lost
        /// </summary>
        internal class PlayerMoneyTracker
        {
            public int money;

            public double moneylost;

            public PlayerMoneyTracker(int m, double ml)
            {
                this.money = m;
                this.moneylost = ml;
            }
        }

        public static PlayerMoneyTracker statedeath;

        public static PlayerMoneyTracker statepassout;

        private static ModConfig config;

        // Allows the class to access the ModConfig properties
        public static void SetConfig(ModConfig config)
        {
            PlayerStateRestorer.config = config;
        }

        // Saves player's current money and amount to be lost, killed
        public static void SaveStateDeath()
        {
            statedeath = new PlayerMoneyTracker(Game1.player.Money, Math.Min(config.DeathPenalty.MoneyLossCap, Game1.player.Money * (1 - config.DeathPenalty.MoneytoRestorePercentage)));
        }

        // Saves player's current money and amount to be lost, passed out
        public static void SaveStatePassout()
        {
            statepassout = new PlayerMoneyTracker(Game1.player.Money, Math.Min(config.PassOutPenalty.MoneyLossCap, Game1.player.Money * (1 - config.PassOutPenalty.MoneytoRestorePercentage)));
        }

        // Load Player state, killed
        public static void LoadStateDeath()
        {
            // Change money to state saved in statedeath
            Game1.player.Money = statedeath.money - (int)Math.Round(statedeath.moneylost);
           
            // Restore stamina to amount as specified by config values
            Game1.player.stamina = (int)(Game1.player.maxStamina * config.DeathPenalty.EnergytoRestorePercentage);

            // Restore health to amount as specified by config values
            Game1.player.health = (int)(Game1.player.maxHealth * config.DeathPenalty.HealthtoRestorePercentage);

            // Is the player's health equal to 0?
            if (Game1.player.health == 0)
            {
                // Yes, fix this to prevent an endless loop of dying

                // Change health to equal 1
                Game1.player.health = 1;
            }

            // Is RestoreItems true?
            if (config.DeathPenalty.RestoreItems == true)
            {
                //Yes, restore items and clear itemsLostLastDeath collection

                // Go through each item lost and saved to itemsLostLastDeath
                foreach (Item item in Game1.player.itemsLostLastDeath)
                {
                    // Add item to player's inventory
                    Game1.player.addItemToInventory(item);
                }
                // Clears items lost, prevents being purchasable at Guild
                Game1.player.itemsLostLastDeath.Clear();
            }
        }

        // Load Player state, passed out
        public static void LoadStatePassout()
        {
            // Change money to state saved in statepassout
            Game1.player.Money = statepassout.money - (int)Math.Round(statepassout.moneylost);
        }
    }
}
