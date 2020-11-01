using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;
using StardewValley.Locations;
using Netcode;

namespace CustomDeathPenaltyPlus
{
    internal static class PassOutPenaltyChangesExtensions
    {
        public static void Reconcile(this ModConfig.PassOutPenaltyChanges changes, IMonitor monitor)
        {
            if (false
                || changes.MoneytoRestorePercentage > 1
                || changes.MoneytoRestorePercentage < 0)
            {
                monitor.Log($"MoneytoRestorePercentage in PassOutPenalty is invalid, default value will be used instead... {changes.MoneytoRestorePercentage} isn't a decimal between 0 and 1", LogLevel.Warn);
                changes.MoneytoRestorePercentage = 0.95;
            }

            if (changes.MoneyLossCap < 0)
            {
                monitor.Log("MoneyLossCap in PassOutPenalty is invalid, default value will be used instead... Using a negative number won't add money, nice try though", LogLevel.Warn);
                changes.MoneyLossCap = 500;
            }

            if (false
                || changes.EnergytoRestorePercentage > 1
                || changes.EnergytoRestorePercentage < 0)
            {
                monitor.Log($"EnergytoRestorePercentage in PassOutPenalty is invalid, default value will be used instead... {changes.EnergytoRestorePercentage} isn't a decimal between 0 and 1", LogLevel.Warn);
                changes.EnergytoRestorePercentage = 0.50;
            }
        }
    }

    internal static class DeathPenaltyChangesExtensions
    {
        public static void Reconcile(this ModConfig.DeathPenaltyChanges changes, IMonitor monitor)
        {
            if (false
                || changes.MoneytoRestorePercentage > 1
                || changes.MoneytoRestorePercentage < 0)
            {
                monitor.Log($"MoneytoRestorePercentage in DeathPenalty is invalid, default value will be used instead... {changes.MoneytoRestorePercentage} isn't a decimal between 0 and 1", LogLevel.Warn);
                changes.MoneytoRestorePercentage = 0.95;
            }

            if (changes.MoneyLossCap < 0)
            {
                monitor.Log("MoneyLossCap in DeathPenalty is invalid, default value will be used instead... Using a negative number won't add money, nice try though", LogLevel.Warn);
                changes.MoneyLossCap = 500;
            }

            if (false
                || changes.EnergytoRestorePercentage > 1
                || changes.EnergytoRestorePercentage < 0)
            {
                monitor.Log($"EnergytoRestorePercentage in DeathPenalty is invalid, default value will be used instead... {changes.EnergytoRestorePercentage} isn't a decimal between 0 and 1", LogLevel.Warn);
                changes.EnergytoRestorePercentage = 0.10;
            }

            if (false
                || changes.HealthtoRestorePercentage > 1
                || changes.HealthtoRestorePercentage <= 0)
            {
                monitor.Log($"HealthtoRestorePercentage in DeathPenalty is invalid, default value will be used instead... {changes.HealthtoRestorePercentage} isn't a decimal between 0 and 1, excluding 0", LogLevel.Warn);
                changes.HealthtoRestorePercentage = 0.50;
            }
        }
    }

    public class ModEntry
        : Mod
    {
        private ModConfig config;

        public static PlayerData PlayerData { get; private set; } = new PlayerData();

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.GameLoop.GameLaunched += this.GameLaunched;
            helper.Events.GameLoop.Saving += this.Saving;
            helper.Events.GameLoop.DayStarted += this.DayStarted;

            this.config = this.Helper.ReadConfig<ModConfig>();

            PlayerStateRestorer.SetConfig(this.config);
            AssetEditor.SetConfig(this.config);
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            this.config.PassOutPenalty.Reconcile(this.Monitor);
            this.config.DeathPenalty.Reconcile(this.Monitor);

            //Edit UI if items will be restored
            if (this.config.DeathPenalty.RestoreItems == true)
            {
                this.Helper.Content.AssetEditors.Add(new AssetEditor.UIFixes(Helper));
            }

            //Edit strings
            this.Helper.Content.AssetEditors.Add(new AssetEditor.StringsFromCSFilesFixes(Helper));
            //Edit mail
            this.Helper.Content.AssetEditors.Add(new AssetEditor.MailDataFixes(Helper));
            //Edit Hospital event
            this.Helper.Content.AssetEditors.Add(new AssetEditor.HospitalEventFixes(Helper));
        }


        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            //Check if player died each half second
            if (e.IsMultipleOf(30))
            {
                //Has player died?
                if (PlayerStateRestorer.statedeath == null && Game1.killScreen)
                {
                    //Save playerstate using DeathPenalty values
                    PlayerStateRestorer.SaveStateDeath();

                    //Reload asset upon death to reflect amount lost
                    this.Helper.Content.InvalidateCache("Strings\\StringsFromCSFiles");
                }
            }

            

            //Restore state after PlayerKilled event ends
            else if (PlayerStateRestorer.statedeath != null && Game1.CurrentEvent == null)
            {
                if(this.config.DeathPenalty.WakeupNextDay == true && Game1.currentLocation.NameOrUniqueName == "Hospital")
                {
                    //Load new day if WakeupNextDay in config is true
                    Game1.NewDay(1.1f);
                }

                if (Game1.player.canMove)
                {
                    //Restore Player state using DeathPenalty values
                    PlayerStateRestorer.LoadStateDeath();
                    //Reset PlayerStateRestorer
                    PlayerStateRestorer.statedeath = null;
                }

            }

            //Has player passed out?
            else if (Game1.timeOfDay == 2600 || Game1.player.stamina <= -15)
            {

                ModEntry.PlayerData.DidPlayerPassOutYesterday = true;

                var farmlocation = Game1.player.currentLocation as FarmHouse;
                var cellarlocation = Game1.player.currentLocation as Cellar;

                //Save playerstate using PassOutPenalty values
                if (farmlocation == null && cellarlocation == null && PlayerStateRestorer.statepassout == null)
                {
                    PlayerStateRestorer.SaveStatePassout();
                    //Save amount lost to data model
                    ModEntry.PlayerData.MoneyLostLastPassOut = (int)Math.Round(PlayerStateRestorer.statepassout.moneylost);
                }
            }

            //Prevents penalty applying if player can stay up past 2am
            else if (Game1.timeOfDay == 2610)
            {
                if(PlayerStateRestorer.statepassout != null)
                {
                    ModEntry.PlayerData.DidPlayerPassOutYesterday = false;
                    ModEntry.PlayerData.MoneyLostLastPassOut = 0;
                    PlayerStateRestorer.statepassout = null;
                }
            }
        }

        private void Saving(object sender, SavingEventArgs e)
        {  
            //Have any values been saved after passing out?
            if (PlayerStateRestorer.statepassout != null)
            {   
                //Restore playerstate using PassOutPenalty values
                PlayerStateRestorer.LoadStatePassout();
                //Save data from data model to respective JSON file
                this.Helper.Data.WriteJsonFile<PlayerData>($"data\\{Constants.SaveFolderName}.json", ModEntry.PlayerData);
                //Reset PlayerStateRestorer
                PlayerStateRestorer.statepassout = null;
            }
            //Has player not passed out but DidPlayerPassOutYesterday property is true?
            else if(ModEntry.PlayerData.DidPlayerPassOutYesterday == true && PlayerStateRestorer.statepassout == null)
            {
                //Change property to false
                ModEntry.PlayerData.DidPlayerPassOutYesterday = false;
                //Save change to respective JSON file
                this.Helper.Data.WriteJsonFile<PlayerData>($"data\\{Constants.SaveFolderName}.json", ModEntry.PlayerData);
            }
        }

        private void DayStarted(object sender, DayStartedEventArgs e)
        {

            //Read player's JSON file for any needed values, create new instance if data doesn't exist
            ModEntry.PlayerData = this.Helper.Data.ReadJsonFile<PlayerData>($"data\\{Constants.SaveFolderName}.json") ?? new PlayerData();

            //Did player pass out yesterday?
            if(ModEntry.PlayerData.DidPlayerPassOutYesterday == true)
            {
                //Yes, change stamina to reflect config settings
                Game1.player.stamina = (int)(Game1.player.maxStamina * this.config.PassOutPenalty.EnergytoRestorePercentage);
                //Invalidate mail
                Helper.Content.InvalidateCache("Data\\mail");
            }
        }
    }
}
