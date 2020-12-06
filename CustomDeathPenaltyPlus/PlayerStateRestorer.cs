using StardewValley;
using System;
using StardewValley.Locations;
using StardewModdingAPI;

namespace CustomDeathPenaltyPlus
{
    internal class PlayerStateRestorer
    {
        /// <summary>
        /// Tracks the players state
        /// </summary>
        internal class PlayerDataTracker
        {
            public int money;

            public double moneylost;

            public int levelslost;

            public string location;

            public PlayerDataTracker(int m, double ml, int ll, string l)
            {
                this.money = m;
                this.moneylost = ml;
                this.levelslost = ll;
                this.location = l;
            }
        }

        public static PlayerDataTracker statedeath;

        public static PlayerDataTracker statepassout;

        private static ModConfig config;

        // Allows the class to access the ModConfig properties
        public static void SetConfig(ModConfig config)
        {
            PlayerStateRestorer.config = config;
        }

        // Saves player's current money and amount to be lost, killed
        public static void SaveStateDeath()
        {
            Random lostlevels = new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed + Game1.timeOfDay);

            statedeath = new PlayerDataTracker(Game1.player.Money, Math.Min(config.DeathPenalty.MoneyLossCap, Game1.player.Money * (1 - config.DeathPenalty.MoneytoRestorePercentage)), lostlevels.Next(0,15), Game1.currentLocation.NameOrUniqueName); ;
        }

        // Saves player's current money and amount to be lost, passed out
        public static void SaveStatePassout()
        {
            statepassout = new PlayerDataTracker(Game1.player.Money, Math.Min(config.PassOutPenalty.MoneyLossCap, Game1.player.Money * (1 - config.PassOutPenalty.MoneytoRestorePercentage)), 0, Game1.currentLocation.NameOrUniqueName);
        }

        // Load Player state, killed
        public static void LoadStateDeath()
        {
            // Change money to state saved in statedeath if money is lost
            if(Game1.player.Money != statedeath.money)
            {
                Game1.player.Money = statedeath.money - (int)Math.Round(statedeath.moneylost);
            }

            //Forget minelevels
            if (true
                // Mine levels will be lost
                && config.DeathPenalty.ExtraCustomisation.ForgetMineLevels == true
                // Player has not reached the mine bottom
                && Game1.player.deepestMineLevel < 120
                && MineShaft.lowestLevelReached < 120
                // Levelslost is not more than the deepest level reached
                && statedeath.levelslost < Game1.player.deepestMineLevel
                && statedeath.levelslost < MineShaft.lowestLevelReached
                // Player was in the mine
                && statedeath.location.Contains("UndergroundMine"))
            {
                // Adjust minelevel data accordingly
                Game1.player.deepestMineLevel = Game1.player.deepestMineLevel - statedeath.levelslost;
                MineShaft.lowestLevelReached = MineShaft.lowestLevelReached - statedeath.levelslost;
            }

            // Restore stamina to amount as specified by config values
            Game1.player.stamina = (int)(Game1.player.maxStamina * config.DeathPenalty.EnergytoRestorePercentage);

            // Restore health to amount as specified by config values
            Game1.player.health = Math.Max((int)(Game1.player.maxHealth * config.DeathPenalty.HealthtoRestorePercentage), 1);

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

            // Is FriendshipPenalty greater than 0?
            if(config.DeathPenalty.ExtraCustomisation.FriendshipPenalty > 0 && (Game1.currentLocation.NameOrUniqueName == "Hospital" || config.DeathPenalty.ExtraCustomisation.WakeupNextDayinClinic == true))
            {
                //Yes, change friendship level for Harvey

                Game1.player.changeFriendship(-Math.Min(config.DeathPenalty.ExtraCustomisation.FriendshipPenalty, Game1.player.getFriendshipLevelForNPC("Harvey")), Game1.getCharacterFromName("Harvey", true));
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
