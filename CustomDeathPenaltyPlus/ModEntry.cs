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
            //Use respective default values if config has invalid values
            if (this.config.DeathPenalty.MoneytoRestorePercentage > 1 || this.config.DeathPenalty.MoneytoRestorePercentage < 0 || this.config.DeathPenalty.MoneyLossCap < 0 || this.config.DeathPenalty.EnergytoRestorePercentage > 1 || this.config.DeathPenalty.EnergytoRestorePercentage <= 0 || this.config.DeathPenalty.HealthtoRestorePercentage > 1 || this.config.DeathPenalty.HealthtoRestorePercentage <= 0)
            {
                this.Monitor.Log("Invalid values found in the config for DeathPenalty", LogLevel.Warn);
                if (this.config.DeathPenalty.MoneytoRestorePercentage > 1 || this.config.DeathPenalty.MoneytoRestorePercentage < 0)
                {
                    this.Monitor.Log($"- MoneytoRestorePercentage is an invalid value, default value will be used instead... {this.config.DeathPenalty.MoneytoRestorePercentage} isn't a decimal between 0 and 1", LogLevel.Info);
                    this.config.DeathPenalty.MoneytoRestorePercentage = 0.95;
                }

                if (this.config.DeathPenalty.MoneyLossCap < 0)
                {
                    this.Monitor.Log("- MoneyLossCap is an invalid value, default value will be used instead... Using a negative number won't add money, nice try though", LogLevel.Info);
                    this.config.DeathPenalty.MoneyLossCap = 500;
                }

                if (this.config.DeathPenalty.EnergytoRestorePercentage > 1 || this.config.DeathPenalty.EnergytoRestorePercentage < 0)
                {
                    this.Monitor.Log($"- EnergytoRestorePercentage is an invalid value, default value will be used instead... {this.config.DeathPenalty.EnergytoRestorePercentage} isn't a decimal between 0 and 1", LogLevel.Info);
                    this.config.DeathPenalty.EnergytoRestorePercentage = 0.10;
                }

                if (this.config.DeathPenalty.HealthtoRestorePercentage > 1 || this.config.DeathPenalty.HealthtoRestorePercentage <= 0)
                {
                    this.Monitor.Log($"- HealthtoRestorePercentage is an invalid value, default value will be used instead... {this.config.DeathPenalty.HealthtoRestorePercentage} isn't a decimal between 0 and 1, excluding 0", LogLevel.Info);
                    this.config.DeathPenalty.HealthtoRestorePercentage = 0.50;
                }  
            }

            if(this.config.PassOutPenalty.MoneyLossCap < 0 || this.config.PassOutPenalty.MoneytoRestorePercentage < 0 || this.config.PassOutPenalty.MoneytoRestorePercentage > 1 || this.config.PassOutPenalty.EnergytoRestorePercentage < 0 || this.config.PassOutPenalty.EnergytoRestorePercentage > 1)
            {
                this.Monitor.Log("Invalid values found in the config for PassOutPenalty", LogLevel.Warn);
                if (this.config.PassOutPenalty.MoneytoRestorePercentage > 1 || this.config.PassOutPenalty.MoneytoRestorePercentage < 0)
                {
                    this.Monitor.Log($"- MoneytoRestorePercentage is an invalid value, default value will be used instead... {this.config.PassOutPenalty.MoneytoRestorePercentage} isn't a decimal between 0 and 1", LogLevel.Info);
                    this.config.PassOutPenalty.MoneytoRestorePercentage = 0.95;
                }

                if (this.config.PassOutPenalty.MoneyLossCap < 0)
                {
                    this.Monitor.Log("- MoneyLossCap is an invalid value, default value will be used instead... Using a negative number won't add money, nice try though", LogLevel.Info);
                    this.config.PassOutPenalty.MoneyLossCap = 500;
                }

                if (this.config.PassOutPenalty.EnergytoRestorePercentage > 1 || this.config.PassOutPenalty.EnergytoRestorePercentage < 0)
                {
                    this.Monitor.Log($"- EnergytoRestorePercentage is an invalid value, default value will be used instead... {this.config.PassOutPenalty.EnergytoRestorePercentage} isn't a decimal between 0 and 1", LogLevel.Info);
                    this.config.PassOutPenalty.EnergytoRestorePercentage = 0.50;
                }
            }

           
            //Edit UI if items will be restored
            if (this.config.DeathPenalty.RestoreItems == true)
            {
                this.Helper.Content.AssetEditors.Add(new AssetEditor.UIFixes(Helper));
            }
            //Edit strings
            this.Helper.Content.AssetEditors.Add(new AssetEditor.StringsFromCSFilesFixes(Helper));
            //Edit mail
            this.Helper.Content.AssetEditors.Add(new AssetEditor.MailDataFixes(Helper));
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            //Check if player died or passed out each half second
            if (e.IsMultipleOf(30))
            {
                //Has player died?
                if (PlayerStateRestorer.state == null && Game1.killScreen)
                {
                    //Save playerstate using DeathPenalty values
                    PlayerStateRestorer.SaveStateDeath();

                    //Reload asset upon death to reflect amount lost
                    this.Helper.Content.InvalidateCache("Strings\\StringsFromCSFiles");
                    
                }
            }

            //Restore state after PlayerKilled event ends
            else if (PlayerStateRestorer.state != null && Game1.CurrentEvent == null && Game1.player.CanMove)
            {
                //Restore Player state using DeathPenalty values
                PlayerStateRestorer.LoadStateDeath();

                //Reset PlayerStateRestorer
                PlayerStateRestorer.state = null;
            }

            //Has player passed out?
            else if (Game1.timeOfDay == 2600 || Game1.player.stamina <= -15)
            {

                ModEntry.PlayerData.DidPlayerPassOutYesterday = true;

                //Don't save if player won't lose money
                var farmlocation = Game1.player.currentLocation as FarmHouse;
                var cellarlocation = Game1.player.currentLocation as Cellar;
                if (farmlocation != null || cellarlocation != null)
                {
                    return;
                }
                //Save playerstate using PassOutPenalty values
                else if(PlayerStateRestorer.statepassout == null)
                {
                    PlayerStateRestorer.SaveStatePassout();
                    //Save amount lost to data model
                    ModEntry.PlayerData.MoneyLostLastPassOut = (int)Math.Round(PlayerStateRestorer.statepassout.moneylost);
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
                this.Helper.Data.WriteJsonFile<PlayerData>($"data\\{Game1.player.Name}.json", ModEntry.PlayerData);
                //Reset PlayerStateRestorer
                PlayerStateRestorer.statepassout = null;
            }
            //Has player not passed out but DidPlayerPassOutYesterday property is true?
            else if(ModEntry.PlayerData.DidPlayerPassOutYesterday == true && PlayerStateRestorer.statepassout == null)
            {
                //Change property to false
                ModEntry.PlayerData.DidPlayerPassOutYesterday = false;
                //Save change to respective JSON file
                this.Helper.Data.WriteJsonFile<PlayerData>($"data\\{Game1.player.Name}.json", ModEntry.PlayerData);
            }
        }

        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                //System.Diagnostics.Debugger.Launch();
            }
            //Read player's JSON file for any needed values, create new instance if data doesn't exist
            ModEntry.PlayerData = this.Helper.Data.ReadJsonFile<PlayerData>($"data\\{Game1.player.Name}.json") ?? new PlayerData();

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
