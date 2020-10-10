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

        private uint halfsecond = 30;

        //Can the asset be editted?
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

        //Edit asset if asset name matches
        public void Edit<T>(IAssetData asset)
        {
            if(System.Diagnostics.Debugger.IsAttached == false)
            {
                System.Diagnostics.Debugger.Launch();
            }

            var editor = asset.AsDictionary<string, string>().Data;
            //Get data key and change value
            //editor["Event.cs.1068"] = "Dr. Harvey charged me 25g for the hospital visit. ";
            var keys = new List<string>(editor.Keys);

            foreach(var k in keys)
            {
                editor[k] = "Amelie";
            }
        }

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            //Check if player died each half second
            if (e.IsMultipleOf(this.halfsecond))
            {
                //Save funds upon death
                if (PlayerStateSaver.state == null && Game1.killScreen)
                {
                    PlayerStateSaver.Save();

                    this.Monitor.Log($"Money saved, amount {PlayerStateSaver.state.money}g");

                    if(Game1.player.Money <= 10000)
                    {
                        //amount lost (discounted)
                        this.Monitor.Log($"Lost {PlayerStateSaver.state.money - (int)Math.Round(PlayerStateSaver.state.money * 0.95)}g"); 
                        
                    }

                    else
                    //amount lost (capped)
                    this.Monitor.Log("Lost 500g");
                }
            }
            //Restore money after event ends
            else if (PlayerStateSaver.state != null && Game1.CurrentEvent == null && Game1.player.CanMove && Game1.player.spouse == "Harvey")
            {
                //capped (lose 500)
                if (Game1.player.Money > 10000)
                {
                    PlayerStateSaver.LoadCapped();
                    this.Monitor.Log("Money restored, to a point... Isn't Harvey nice?", LogLevel.Debug);
                }
                //discounted (lose 5%)
                else
                {
                    PlayerStateSaver.LoadDiscounted();
                    this.Monitor.Log("Money restored, excluding 5%... rebates don't cover everything", LogLevel.Debug);
                }

                this.Monitor.Log("Half health restored. You did almost die after all...", LogLevel.Debug);

               //Reset state saver
                PlayerStateSaver.state = null;
            }
        }

    }
}
