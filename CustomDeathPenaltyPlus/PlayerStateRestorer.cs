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

            public int minelevel;

            public PlayerDataTracker(int m, double ml, int ll, int mil)
            {
                this.money = m;
                this.moneylost = ml;
                this.levelslost = ll;
                this.minelevel = mil;
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

        // Saves player's current money, amount to be lost and mine data, killed
        public static void SaveStateDeath()
        {
            Random lostlevels = new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed + Game1.timeOfDay);

            var tracker = Game1.currentLocation as MineShaft;

            statedeath = new PlayerDataTracker(Game1.player.Money, Math.Min(config.DeathPenalty.MoneyLossCap, Game1.player.Money * (1 - config.DeathPenalty.MoneytoRestorePercentage)), Math.Min(Game1.player.deepestMineLevel, lostlevels.Next(0,16)), Game1.currentLocation as MineShaft != null ? tracker.mineLevel : 121);
        }

        // Saves player's current money, amount to be lost and mine data, passed out
        public static void SaveStatePassout()
        {
            statepassout = new PlayerDataTracker(Game1.player.Money, Math.Min(config.PassOutPenalty.MoneyLossCap, Game1.player.Money * (1 - config.PassOutPenalty.MoneytoRestorePercentage)), 0, 121);
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
                && config.ExtraDeathPenaltyCustomisation.ForgetMineLevels == true
                // Player has not reached the mine bottom
                && Game1.player.deepestMineLevel < 120
                && MineShaft.lowestLevelReached < 120
                // Player was in the mine
                && statedeath.minelevel < 121)
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
                    //Is the player's inventory full?
                    if (Game1.player.isInventoryFull() == true)
                    {
                        // Yes, drop item on the floor
                        Game1.player.dropItem(item);
                    }

                    else
                    {
                        // No, add item to player's inventory
                        Game1.player.addItemToInventory(item);
                    }  
                }
                // Clears items lost, prevents being purchasable at Guild
                Game1.player.itemsLostLastDeath.Clear();
            }

            // Is FriendshipPenalty greater than 0?
            if(config.ExtraDeathPenaltyCustomisation.FriendshipPenalty > 0 && Game1.player.friendshipData.ContainsKey("Harvey") && (Game1.currentLocation.NameOrUniqueName == "Hospital" || config.ExtraDeathPenaltyCustomisation.WakeupNextDayinClinic == true))
            {
                //Yes, change friendship level for Harvey

                Game1.player.changeFriendship(-Math.Min(config.ExtraDeathPenaltyCustomisation.FriendshipPenalty, Game1.player.getFriendshipLevelForNPC("Harvey")), Game1.getCharacterFromName("Harvey", true));
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
