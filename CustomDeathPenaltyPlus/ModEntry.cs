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
using StardewValley.Menus;

namespace CustomDeathPenaltyPlus
{
    /// <summary>
    /// Extensions for the PassOutPenaltyChanges class
    /// </summary>
    internal static class PassOutPenaltyChangesExtensions
    {
        public static void Reconcile(this ModConfig.PassOutPenaltyChanges changes, IMonitor monitor)
        {
            // Reconcile MoneytoRestorePercentage if it's value is ouside the useable range
            if (false
                || changes.MoneytoRestorePercentage > 1
                || changes.MoneytoRestorePercentage < 0)
            {
                monitor.Log($"MoneytoRestorePercentage in PassOutPenalty is invalid, default value will be used instead... {changes.MoneytoRestorePercentage} isn't a decimal between 0 and 1", LogLevel.Warn);
                changes.MoneytoRestorePercentage = 0.95;
            }

            // Reconcile MoneyLossCap if it's value is -ve
            if (changes.MoneyLossCap < 0)
            {
                monitor.Log("MoneyLossCap in PassOutPenalty is invalid, default value will be used instead... Using a negative number won't add money, nice try though", LogLevel.Warn);
                changes.MoneyLossCap = 500;
            }

            // Reconcile EnergytoRestorePercentage if it's value is ouside the useable range
            if (false
                || changes.EnergytoRestorePercentage > 1
                || changes.EnergytoRestorePercentage < 0)
            {
                monitor.Log($"EnergytoRestorePercentage in PassOutPenalty is invalid, default value will be used instead... {changes.EnergytoRestorePercentage} isn't a decimal between 0 and 1", LogLevel.Warn);
                changes.EnergytoRestorePercentage = 0.50;
            }
        }
    }

    /// <summary>
    /// Extensions for the DeathPenaltyChanges class
    /// </summary>
    internal static class DeathPenaltyChangesExtensions
    {
        public static void Reconcile(this ModConfig.DeathPenaltyChanges changes, IMonitor monitor)
        {
            // Reconcile MoneytoRestorePercentage if it's value is ouside the useable range
            if (false
                || changes.MoneytoRestorePercentage > 1
                || changes.MoneytoRestorePercentage < 0)
            {
                monitor.Log($"MoneytoRestorePercentage in DeathPenalty is invalid, default value will be used instead... {changes.MoneytoRestorePercentage} isn't a decimal between 0 and 1", LogLevel.Warn);
                changes.MoneytoRestorePercentage = 0.95;
            }

            // Reconcile MoneyLossCap if the value is -ve
            if (changes.MoneyLossCap < 0)
            {
                monitor.Log("MoneyLossCap in DeathPenalty is invalid, default value will be used instead... Using a negative number won't add money, nice try though", LogLevel.Warn);
                changes.MoneyLossCap = 500;
            }

            // Reconcile EnergytoRestorePercentage if it's value is ouside the useable range
            if (false
                || changes.EnergytoRestorePercentage > 1
                || changes.EnergytoRestorePercentage < 0)
            {
                monitor.Log($"EnergytoRestorePercentage in DeathPenalty is invalid, default value will be used instead... {changes.EnergytoRestorePercentage} isn't a decimal between 0 and 1", LogLevel.Warn);
                changes.EnergytoRestorePercentage = 0.10;
            }

            // Reconcile HealthtoRestorePercentage if it's value is ouside the useable range
            if (false
                || changes.HealthtoRestorePercentage > 1
                || changes.HealthtoRestorePercentage <= 0)
            {
                monitor.Log($"HealthtoRestorePercentage in DeathPenalty is invalid, default value will be used instead... {changes.HealthtoRestorePercentage} isn't a decimal between 0 and 1, excluding 0", LogLevel.Warn);
                changes.HealthtoRestorePercentage = 0.50;
            }
        }
    }

    /// <summary>The mod entry point.</summary>
    public class ModEntry
        : Mod
    {
        private ModConfig config;

        public static PlayerData PlayerData { get; private set; } = new PlayerData();

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.GameLoop.GameLaunched += this.GameLaunched;
            helper.Events.GameLoop.Saving += this.Saving;
            helper.Events.GameLoop.DayStarted += this.DayStarted;

            // Read the mod config for values and create one if one does not currently exist
            this.config = this.Helper.ReadConfig<ModConfig>();

            PlayerStateRestorer.SetConfig(this.config);
            AssetEditor.SetConfig(this.config);
        }

        /// <summary>Raised after the game is launched, right before the first game tick</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Reconcile config values
            this.config.PassOutPenalty.Reconcile(this.Monitor);
            this.config.DeathPenalty.Reconcile(this.Monitor);

            // Is WakeupNextDayinClinic true?
            if (this.config.DeathPenalty.WakeupNextDayinClinic == true)
            {
                // Yes, edit some events

                //Edit MineEvents
                this.Helper.Content.AssetEditors.Add(new AssetEditor.MineEventFixes(Helper));
                //Edit HospitalEvents
                this.Helper.Content.AssetEditors.Add(new AssetEditor.HospitalEventFixes(Helper));
            }

            // Edit strings
            this.Helper.Content.AssetEditors.Add(new AssetEditor.StringsFromCSFilesFixes(Helper));
            
            // Edit mail
            this.Helper.Content.AssetEditors.Add(new AssetEditor.MailDataFixes(Helper));
        }

        /// <summary>Raised after the game state is updated</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
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
                    // Save playerstate using DeathPenalty values
                    PlayerStateRestorer.SaveStateDeath();

                    // Reload asset upon death to reflect amount lost
                    this.Helper.Content.InvalidateCache("Strings\\StringsFromCSFiles");
                }
            }

            // Close items lost menu if items will be restored
            if(true
                // Player death state has been saved
                && PlayerStateRestorer.statedeath != null
                // DeathPenalty.RestoreItems true
                && this.config.DeathPenalty.RestoreItems == true
                // An event is in progress, this would be the PlayerKilled event
                && Game1.CurrentEvent != null
                // The current clickable menu can be cast to an ItemListMenu
                && Game1.activeClickableMenu as ItemListMenu != null)
            {
                // Yes, we don't want that menu, so close it and end the event

                // Close the menu
                Game1.activeClickableMenu.exitThisMenuNoSound();
                // End the event
                Game1.CurrentEvent.exitEvent();
            }

            // Restore state after PlayerKilled event ends
            if (true
                // Player death state has been saved
                && PlayerStateRestorer.statedeath != null
                // No events are running
                && Game1.CurrentEvent == null
                // Player can move
                && Game1.player.canMove)
            {
                // Check if WakeupNextDayinClinic is true
                if (this.config.DeathPenalty.WakeupNextDayinClinic == true)
                {
                    // Yes, do some extra stuff

                    // Warp player to clinic if it is not the current location
                    if (Game1.currentLocation.NameOrUniqueName == "Mine")
                    {
                        Game1.warpFarmer("Hospital", 20, 12, false);
                    }
                   
                    // Is the game in multiplayer?
                    if (Context.IsMultiplayer == false)
                    {
                        // No, new day can be loaded

                        // Load new day
                        Game1.NewDay(1.1f);

                        // Save necessary data to data model
                        ModEntry.PlayerData.DidPlayerWakeupinClinic = true;

                        // Write data model to JSON file
                        this.Helper.Data.WriteJsonFile<PlayerData>($"data\\{Constants.SaveFolderName}.json", ModEntry.PlayerData);

                    }
                    
                }
                // Restore Player state using DeathPenalty values
                PlayerStateRestorer.LoadStateDeath();

                // Reset PlayerStateRestorer class with the statedeath field
                PlayerStateRestorer.statedeath = null;
            }

            // Chack if time is 2am or the player has passed out
            if (Game1.timeOfDay == 2600 || Game1.player.stamina <= -15)
            {
                // Set DidPlayerPassOutYesterday to true and DidPlayerWakeupinClinic to false in data model
                ModEntry.PlayerData.DidPlayerPassOutYesterday = true;
                ModEntry.PlayerData.DidPlayerWakeupinClinic = false;

                if (true
                    // Player is not in FarmHouse
                    && Game1.player.currentLocation as FarmHouse == null
                    // Player is not in Cellar
                    && Game1.player.currentLocation as Cellar == null
                    // Player pass out state has not been saved
                    && PlayerStateRestorer.statepassout == null)
                {
                    // Save playerstate using PassOutPenalty values
                    PlayerStateRestorer.SaveStatePassout();
                    // Save amount lost to data model
                    ModEntry.PlayerData.MoneyLostLastPassOut = (int)Math.Round(PlayerStateRestorer.statepassout.moneylost);
                }
            }

            // If player can stay up past 2am, discard saved values and reset changed properties in data model
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

        /// <summary>Raised before the game begins writing data to the save file</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void Saving(object sender, SavingEventArgs e)
        {
            // Save data from data model to respective JSON file
            this.Helper.Data.WriteJsonFile<PlayerData>($"data\\{Constants.SaveFolderName}.json", ModEntry.PlayerData);

            // Has the pass out state been saved after passing out?
            if (PlayerStateRestorer.statepassout != null)
            {   
                //Yes, reload the state

                // Restore playerstate using PassOutPenalty values
                PlayerStateRestorer.LoadStatePassout();

                // Reset PlayerStateRestorer class with the statepassout field
                PlayerStateRestorer.statepassout = null;
            }

            // Has player not passed out but DidPlayerPassOutYesterday property is true?
            else if(ModEntry.PlayerData.DidPlayerPassOutYesterday == true && PlayerStateRestorer.statepassout == null)
            {
                // Yes, fix this so the new day will load correctly

                // Change DidPlayerPassOutYesterday property to false
                ModEntry.PlayerData.DidPlayerPassOutYesterday = false;

                // Save change to respective JSON file
                this.Helper.Data.WriteJsonFile<PlayerData>($"data\\{Constants.SaveFolderName}.json", ModEntry.PlayerData);
            }

            // Is DidPlayerWakeupinClinic true?
            if(ModEntry.PlayerData.DidPlayerWakeupinClinic == true)
            {               
                //Is player in bed or has player passed out? (player has not died)
                if(Game1.player.isInBed.Value == true || ModEntry.PlayerData.DidPlayerPassOutYesterday == true)
                {
                    // Yes, fix this so the new day will load correctly

                    // Change property to false
                    ModEntry.PlayerData.DidPlayerWakeupinClinic = false;

                    // Save change to respective JSON file
                    this.Helper.Data.WriteJsonFile<PlayerData>($"data\\{Constants.SaveFolderName}.json", ModEntry.PlayerData);
                }
            }
        }

        /// <summary>Raised after the game begins a new day</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void DayStarted(object sender, DayStartedEventArgs e)
        {

            // Read player's JSON file for any needed values, create new instance if data doesn't exist
            ModEntry.PlayerData = this.Helper.Data.ReadJsonFile<PlayerData>($"data\\{Constants.SaveFolderName}.json") ?? new PlayerData();

            // Did player pass out yesterday?
            if(ModEntry.PlayerData.DidPlayerPassOutYesterday == true)
            {
                // Yes, fix player state

                // Change stamina to the amount restored by the config values
                Game1.player.stamina = (int)(Game1.player.maxStamina * this.config.PassOutPenalty.EnergytoRestorePercentage);

                // Invalidate cached mail data, this allows it to reload with correct values
                Helper.Content.InvalidateCache("Data\\mail");
            }

            //Did player wake up in clinic?
            if(ModEntry.PlayerData.DidPlayerWakeupinClinic == true)
            {
                //Yes, fix player state

                // Is the player not at the clinic?
                if(Game1.currentLocation.NameOrUniqueName != "Hospital")
                {
                    // Warp player to clinic
                    Game1.warpFarmer("Hospital", 20, 12, false);
                }

                // Change health and stamina to the amount restored by the config values
                Game1.player.stamina = (int)(Game1.player.maxStamina * this.config.DeathPenalty.EnergytoRestorePercentage);
                Game1.player.health = (int)(Game1.player.maxHealth * this.config.DeathPenalty.HealthtoRestorePercentage);

                // Is the player's health equal to 0?
                if (Game1.player.health == 0)
                {
                    // Yes, fix this to prevent an endless loop of dying

                    // Change health to equal 1
                    Game1.player.health = 1;
                }
            }
        }
    }
}
