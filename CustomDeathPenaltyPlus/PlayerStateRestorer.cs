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

            public PlayerMoneyTracker(int m, double ml)
            {
                this.money = m;
                this.moneylost = ml;
            }
        }

        public static PlayerMoneyTracker state;

        public static PlayerMoneyTracker statepassout;

        private static ModConfig config;

        public static void SetConfig(ModConfig config)
        {
            PlayerStateRestorer.config = config;
        }

        // Saves player's current money and amount to be lost, killed
        public static void Save()
        {
            state = new PlayerMoneyTracker(Game1.player.Money, Math.Min(config.MoneyLossCap, Game1.player.Money * (1-config.MoneytoRestorePercentage)));
        }

        // Saves player's current money and amount to be lost, passed out
        public static void SavePassout()
        {
            statepassout = new PlayerMoneyTracker(Game1.player.Money, Math.Min(config.PassOutMoneyLossCap, Game1.player.Money * (1 - config.PassOutMoneytoRestorePercentage)));
        }

        //Load Player state, killed
        public static void Load()
        {
            //Restore money
            Game1.player.Money = state.money - (int)Math.Round(state.moneylost);
           
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

        //Load Player state, passed out
        public static void LoadPassout()
        {
            //Restore money
            Game1.player.Money = statepassout.money - (int)Math.Round(statepassout.moneylost);
        }
    }
}
