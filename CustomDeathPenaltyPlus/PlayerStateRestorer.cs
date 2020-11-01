using StardewValley;
using System;

namespace CustomDeathPenaltyPlus
{
    internal class PlayerStateRestorer
    {
        internal class PlayerMoneyTracker
        {
            public int money;

            public double moneylost;

            public int timeofday;

            public PlayerMoneyTracker(int m, double ml, int t)
            {
                this.money = m;
                this.moneylost = ml;
                this.timeofday = t;
            }
        }

        public static PlayerMoneyTracker statedeath;

        public static PlayerMoneyTracker statepassout;

        private static ModConfig config;

        public static void SetConfig(ModConfig config)
        {
            PlayerStateRestorer.config = config;
        }

        // Saves player's current money and amount to be lost, killed
        public static void SaveStateDeath()
        {
            statedeath = new PlayerMoneyTracker(Game1.player.Money, Math.Min(config.DeathPenalty.MoneyLossCap, Game1.player.Money * (1-config.DeathPenalty.MoneytoRestorePercentage)), Game1.timeOfDay);
        }

        // Saves player's current money and amount to be lost, passed out
        public static void SaveStatePassout()
        {
            statepassout = new PlayerMoneyTracker(Game1.player.Money, Math.Min(config.PassOutPenalty.MoneyLossCap, Game1.player.Money * (1 - config.PassOutPenalty.MoneytoRestorePercentage)), Game1.timeOfDay);
        }

        //Load Player state, killed
        public static void LoadStateDeath()
        {
            //Restore money
            Game1.player.Money = statedeath.money - (int)Math.Round(statedeath.moneylost);
           
            //Restore stamina
            Game1.player.stamina = (int)(Game1.player.maxStamina * config.DeathPenalty.EnergytoRestorePercentage);

            //Restore health
            Game1.player.health = (int)(Game1.player.maxHealth * config.DeathPenalty.HealthtoRestorePercentage);
            if (Game1.player.health == 0)
            {
                Game1.player.health = 1;
            }

            //Restore items
            if (config.DeathPenalty.RestoreItems == true)
            {
                foreach (Item item in Game1.player.itemsLostLastDeath)
                {
                    Game1.player.addItemToInventory(item);
                }
                //Clears items lost, prevents being purchasable at Guild
                Game1.player.itemsLostLastDeath.Clear();
            }
        }

        //Load Player state, passed out
        public static void LoadStatePassout()
        {
            //Restore money
            Game1.player.Money = statepassout.money - (int)Math.Round(statepassout.moneylost);
        }
    }
}
