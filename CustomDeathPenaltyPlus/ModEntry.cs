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
            if (config.MoneytoRestorePercentage > 1 || config.MoneytoRestorePercentage < 0 || config.MoneyLossCap < 0 || config.EnergytoRestorePercentage > 1 || config.EnergytoRestorePercentage <= 0 || config.HealthtoRestorePercentage > 1 || config.HealthtoRestorePercentage <= 0)
            {
                if (config.MoneytoRestorePercentage > 1 || config.MoneytoRestorePercentage < 0)
                {
                    this.Monitor.Log($"MoneytoRestorePercentage is an invalid value, default value will be used instead... {config.MoneytoRestorePercentage} isn't a decimal between 0 and 1", LogLevel.Warn);
                    config.MoneytoRestorePercentage = 0.95;
                }

                if (config.MoneyLossCap < 0)
                {
                    this.Monitor.Log("MoneyLossCap is an invalid value, default value will be used instead... Using a negative number won't add money, nice try though", LogLevel.Warn);
                    config.MoneyLossCap = 500;
                }

                if (config.EnergytoRestorePercentage > 1 || config.EnergytoRestorePercentage < 0)
                {
                    this.Monitor.Log($"EnergytoRestorePercentage is an invalid value, default value will be used instead... {config.EnergytoRestorePercentage} isn't a decimal between 0 and 1", LogLevel.Warn);
                    config.EnergytoRestorePercentage = 0.10;
                }

                if (config.HealthtoRestorePercentage > 1 || config.HealthtoRestorePercentage <= 0)
                {
                    this.Monitor.Log($"HealthtoRestorePercentage is an invalid value, default value will be used instead... {config.HealthtoRestorePercentage} isn't a decimal between 0 and 1, excluding 0", LogLevel.Warn);
                    config.HealthtoRestorePercentage = 0.50;
                }

                if (config.PassOutMoneytoRestorePercentage > 1 || config.PassOutMoneytoRestorePercentage < 0)
                {
                    this.Monitor.Log($"PassOutMoneytoRestorePercentage is an invalid value, default value will be used instead... {config.PassOutMoneytoRestorePercentage} isn't a decimal between 0 and 1", LogLevel.Warn);
                    config.PassOutMoneytoRestorePercentage = 0.95;
                }

                if (config.PassOutMoneyLossCap < 0)
                {
                    this.Monitor.Log("PassOutMoneyLossCap is an invalid value, default value will be used instead... Using a negative number won't add money, nice try though", LogLevel.Warn);
                    config.PassOutMoneyLossCap = 500;
                }

                if (config.PassOutEnergytoRestorePercentage > 1 || config.PassOutEnergytoRestorePercentage < 0)
                {
                    this.Monitor.Log($"PassOutEnergytoRestorePercentage is an invalid value, default value will be used instead... {config.PassOutEnergytoRestorePercentage} isn't a decimal between 0 and 1", LogLevel.Warn);
                    config.PassOutEnergytoRestorePercentage = 0.50;
                }
            }

           
            //Edit UI if items will be restored
            if (config.RestoreItems == true)
            {
                Helper.Content.AssetEditors.Add(new AssetEditor.UIFixes(Helper));
            }
            //Edit strings
            Helper.Content.AssetEditors.Add(new AssetEditor.StringsFromCSFilesFixes(Helper));
            //Edit mail
            Helper.Content.AssetEditors.Add(new AssetEditor.MailDataFixes(Helper));
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            //Check if player died or passed out each half second
            if (e.IsMultipleOf(30))
            {
                //Save money upon death and calculate amount of money to lose
                if (PlayerStateRestorer.state == null && Game1.killScreen)
                {
                    PlayerStateRestorer.Save();

                    //Reload asset upon death to reflect amount lost
                    Helper.Content.InvalidateCache("Strings\\StringsFromCSFiles");
                    
                }
            }

            //Restore state after event ends
            else if (PlayerStateRestorer.state != null && Game1.CurrentEvent == null && Game1.player.CanMove)
            {
                //Restore Player state
                PlayerStateRestorer.Load();

                //Reset PlayerStateSaver
                PlayerStateRestorer.state = null;
            }

            //Save money upon passing out and calculate amount of money to lose
            else if (Game1.timeOfDay == 2600 || Game1.player.stamina <= -15)
            {
                

                //Don't save if player won't lose money
                var farmlocation = Game1.player.currentLocation as FarmHouse;
                var cellarlocation = Game1.player.currentLocation as Cellar;
                if (farmlocation != null || cellarlocation != null)
                {
                    return;
                }
                else if(PlayerStateRestorer.statepassout == null)
                {
                    PlayerStateRestorer.SavePassout();
                    ModEntry.PlayerData.DidPlayerPassOutYesterday = true;
                    ModEntry.PlayerData.MoneyLostLastPassOut = (int)Math.Round(PlayerStateRestorer.statepassout.moneylost);
                }
            }   
        }

        private void Saving(object sender, SavingEventArgs e)
        {  
            if (PlayerStateRestorer.statepassout != null)
            {   
                PlayerStateRestorer.LoadPassout();
                this.Helper.Data.WriteJsonFile<PlayerData>($"data\\{Game1.player.Name}.json", ModEntry.PlayerData);
                PlayerStateRestorer.statepassout = null;
            }
            else if(ModEntry.PlayerData.DidPlayerPassOutYesterday == true)
            {
                ModEntry.PlayerData.DidPlayerPassOutYesterday = false;
                this.Helper.Data.WriteJsonFile<PlayerData>($"data\\{Game1.player.Name}.json", ModEntry.PlayerData);
            }
        }

        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                //System.Diagnostics.Debugger.Launch();
            }
            ModEntry.PlayerData = this.Helper.Data.ReadJsonFile<PlayerData>($"data\\{Game1.player.Name}.json") ?? new PlayerData();

            if(ModEntry.PlayerData.DidPlayerPassOutYesterday == true)
            {
                Game1.player.stamina = (int)(Game1.player.maxStamina * config.PassOutEnergytoRestorePercentage);
                Helper.Content.InvalidateCache("Data\\mail");
            }
        }
    }
}
