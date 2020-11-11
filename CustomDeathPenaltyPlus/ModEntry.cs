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
            //Reconcile config values if needed
            this.config.PassOutPenalty.Reconcile(this.Monitor);
            this.config.DeathPenalty.Reconcile(this.Monitor);

            //Edit UI
            this.Helper.Content.AssetEditors.Add(new AssetEditor.UIFixes(Helper));
            //Edit PlayerKilled events
            if (this.config.DeathPenalty.WakeupNextDayinClinic == true)
            {
                this.Helper.Content.AssetEditors.Add(new AssetEditor.MineEventFixes(Helper));
                this.Helper.Content.AssetEditors.Add(new AssetEditor.HospitalEventFixes(Helper));
            }
            //Edit strings
            this.Helper.Content.AssetEditors.Add(new AssetEditor.StringsFromCSFilesFixes(Helper));
            //Edit mail
            this.Helper.Content.AssetEditors.Add(new AssetEditor.MailDataFixes(Helper));
        }


        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            //Check if player died each half second
            if (e.IsMultipleOf(30))
            {

                if (true
                    // Has player died?
                    && Game1.killScreen
                    // has the players death state been saved?
                    && PlayerStateRestorer.statedeath == null)
                {
                    // death state has NOT been recorded... so do that now.

                    // Save playerstate using DeathPenalty values
                    PlayerStateRestorer.SaveStateDeath();

                    // Reload asset upon death to reflect amount lost
                    this.Helper.Content.InvalidateCache("Strings\\StringsFromCSFiles");
                }
            }

            // Restore state after PlayerKilled event ends
            if (true
                // player death state has been saved
                && PlayerStateRestorer.statedeath != null
                // no events are running
                && Game1.CurrentEvent == null
                // player is ready for action!
                && Game1.player.canMove)
            {
                if (this.config.DeathPenalty.WakeupNextDayinClinic == true)
                {
                    // If WakeupNextDayinClinic is true, warp farmer to clinic if necessary
                    if (Game1.currentLocation.NameOrUniqueName == "Mine")
                    {
                        Game1.warpFarmer("Hospital", 20, 12, false);
                    }

                    
                    // Only load new day in single-player
                    if (Context.IsMultiplayer == false)
                    {
                        Game1.NewDay(1.1f);

                        // Save necessary data to data model
                        ModEntry.PlayerData.DidPlayerWakeupinClinic = true;

                        // Write data model to JSON file
                        this.Helper.Data.WriteJsonFile<PlayerData>($"data\\{Constants.SaveFolderName}.json", ModEntry.PlayerData);

                    }
                    
                }
                // Restore Player state using DeathPenalty values
                PlayerStateRestorer.LoadStateDeath();

                // Reset PlayerStateRestorer
                PlayerStateRestorer.statedeath = null;
            }

            //Has player passed out?
            if (Game1.timeOfDay == 2600 || Game1.player.stamina <= -15)
            {
                //Set DidPlayerPassOUtYesterday to true in data model, and DidPlayerWakeupinClinic to false
                ModEntry.PlayerData.DidPlayerPassOutYesterday = true;
                ModEntry.PlayerData.DidPlayerWakeupinClinic = false;

                if (Game1.player.currentLocation as FarmHouse == null && Game1.player.currentLocation as Cellar == null && PlayerStateRestorer.statepassout == null)
                {
                    //Save playerstate using PassOutPenalty values
                    PlayerStateRestorer.SaveStatePassout();
                    //Save amount lost to data model
                    ModEntry.PlayerData.MoneyLostLastPassOut = (int)Math.Round(PlayerStateRestorer.statepassout.moneylost);
                }
            }

            //Prevents penalty applying if player can stay up past 2am using other mods
            if (Game1.timeOfDay == 2610)
            {
                if (PlayerStateRestorer.statepassout != null)
                {
                    ModEntry.PlayerData.DidPlayerPassOutYesterday = false;
                    ModEntry.PlayerData.MoneyLostLastPassOut = 0;
                    PlayerStateRestorer.statepassout = null;
                }
            }
        }

       

        private void Saving(object sender, SavingEventArgs e)
        {
            //Save data from data model to respective JSON file
            this.Helper.Data.WriteJsonFile<PlayerData>($"data\\{Constants.SaveFolderName}.json", ModEntry.PlayerData);
            //Have any values been saved after passing out?
            if (PlayerStateRestorer.statepassout != null)
            {   
                //Restore playerstate using PassOutPenalty values
                PlayerStateRestorer.LoadStatePassout();
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
            //Has player not died but DidPlayerWakeupinClinic is true?
            if(ModEntry.PlayerData.DidPlayerWakeupinClinic == true)
            {
                //Check player is in bed
                if(Game1.player.isInBed.Value == true || ModEntry.PlayerData.DidPlayerPassOutYesterday == true)
                {
                    //Change property to false
                    ModEntry.PlayerData.DidPlayerWakeupinClinic = false;
                    //Save change to respective JSON file
                    this.Helper.Data.WriteJsonFile<PlayerData>($"data\\{Constants.SaveFolderName}.json", ModEntry.PlayerData);
                }
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

            //Did player wake up in clinic?
            if(ModEntry.PlayerData.DidPlayerWakeupinClinic == true)
            {
                //Yes, warp to clinic and change health and stamina to reflect config values
                if(Game1.currentLocation.NameOrUniqueName != "Hospital")
                {
                    Game1.warpFarmer("Hospital", 20, 12, false);
                }
                Game1.player.stamina = (int)(Game1.player.maxStamina * this.config.DeathPenalty.EnergytoRestorePercentage);
                Game1.player.health = (int)(Game1.player.maxHealth * this.config.DeathPenalty.HealthtoRestorePercentage);
                if (Game1.player.health == 0)
                {
                    Game1.player.health = 1;
                }
            }
        }
    }
}
