﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using System.Collections.Generic;
using System.Linq;

namespace SmallerDeathPenalty
{
    public class ModEntry
        : Mod, IAssetEditor
    {
        private ModConfig config;
        //Allow asset to be editted if name matches
        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Strings\\StringsFromCSFiles"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Edit asset
        public void Edit<T>(IAssetData asset)
        {
            var editor = asset.AsDictionary<string, string>().Data;

            //Does the PlayerStateSaver exist?
            if (PlayerStateSaver.state == null)
            {
                editor["Event.cs.1068"] = "Dr. Harvey charged me 500g for the hospital visit. ";
                editor["Event.cs.1058"] = "I seem to have lost 500g";
            }
            
            //Edit strings to reflect restored money, also check config values
            else
            {
                PlayerStateSaver.CheckConfig();
                if (config.MoneyLossCap == 0)
                {
                    editor["Event.cs.1068"] = "Dr. Harvey didn't charge me for the hospital visit, how nice. ";
                    editor["Event.cs.1058"] = "Fortunately, I still have all my money";
                }
                else if (PlayerStateSaver.state.money * (1 - config.RestoreMoneyPercentage) > config.MoneyLossCap)
                {
                    //Edit events to reflect capped amount lost
                    editor["Event.cs.1068"] = $"Dr. Harvey charged me {config.MoneyLossCap}g for the hospital visit. ";
                    editor["Event.cs.1058"] = $"I seem to have lost {config.MoneyLossCap}g";
                }
                else
                {
                    //Edit events to reflect discounted amount lost
                    editor["Event.cs.1068"] = $"Dr. Harvey charged me {PlayerStateSaver.state.money - (int)Math.Round(PlayerStateSaver.state.money * config.RestoreMoneyPercentage)}g for the hospital visit. ";
                    editor["Event.cs.1058"] = $"I seem to have lost {PlayerStateSaver.state.money - (int)Math.Round(PlayerStateSaver.state.money * config.RestoreMoneyPercentage)}g";
                }
            }

            if (config.RestoreItems == true)
            {
                //Remove unnecessary strings
                editor["Event.cs.1060"] = "";
                editor["Event.cs.1061"] = "";
                editor["Event.cs.1062"] = "";
                editor["Event.cs.1063"] = "";
                editor["Event.cs.1071"] = "";
            }
        }
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.GameLoop.GameLaunched += this.GameLaunched;
            
            this.config = this.Helper.ReadConfig<ModConfig>();

            PlayerStateSaver.SetConfig(this.config);
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            if(config.RestoreItems == true)
            {
                Helper.Content.AssetEditors.Add(new UIFixes(Helper));
            }
        }
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            //Check if player died each half second
            if (e.IsMultipleOf(30))
            {
                //Save funds upon death
                if (PlayerStateSaver.state == null && Game1.killScreen)
                {
                    PlayerStateSaver.Save();

                    //Display in monitor if config values are invalid
                    if (config.RestoreMoneyPercentage > 1 || config.RestoreMoneyPercentage < 0)
                    {
                        this.Monitor.Log("RestoreMoneyPercentage is an invalid value, default value will be used instead... (Is the value a decimal between 0 and 1?)", LogLevel.Debug);
                    }
                    if (config.EnergytoRestorePercentage > 1 || config.EnergytoRestorePercentage <= 0)
                    {
                        this.Monitor.Log("EnergytoRestorePercentage is an invalid value, default value will be used instead... (Is the value a decimal between 0 and 1 not including 0?)", LogLevel.Debug);
                    }
                    if (config.HealthtoRestorePercentage > 1 || config.HealthtoRestorePercentage <= 0)
                    {
                        this.Monitor.Log("HealthtoRestorePercentage is an invalid value, default value will be used instead... (Is the value a decimal between 0 and 1 not including 0?)", LogLevel.Debug);
                    }
                    if (config.MoneyLossCap < 0)
                    {
                        this.Monitor.Log("MoneyLossCap is an invalid value, default value will be used instead, (Using a negative number won't add money, nice try though)", LogLevel.Debug);
                    }

                    //Reload asset upon death to reflect amount lost, config is checked and fixed here
                    Helper.Content.InvalidateCache("Strings\\StringsFromCSFiles");
                }
            }
            //Restore state after event ends
            else if (PlayerStateSaver.state != null && Game1.CurrentEvent == null && Game1.player.CanMove)
            {
                //Restore Player state
                PlayerStateSaver.Load();

                this.Monitor.Log("Player state restored...", LogLevel.Debug);

                //Reset PlayerStateSaver
                PlayerStateSaver.state = null;
            }
        }
    }
}
