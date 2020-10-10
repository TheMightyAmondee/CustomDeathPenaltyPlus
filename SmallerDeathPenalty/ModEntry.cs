using System;
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
                //System.Diagnostics.Debugger.Launch();
            }

            var editor = asset.AsDictionary<string, string>().Data;

            //Does the PlayerStateSaver not exist or is the player's money greater than 10,000g?
            if (PlayerStateSaver.state == null || Game1.player.Money > 10000)
            {
                //Edit events to reflect capped amount lost, default
                editor["Event.cs.1068"] = "Dr. Harvey charged me 500g for the hospital visit. ";
                editor["Event.cs.1058"] = "I seem to have lost 500g";
            }
            else
            {
                //Edit events to reflect discounted amount lost
                editor["Event.cs.1068"] = $"Dr. Harvey charged me {PlayerStateSaver.state.money - (int)Math.Round(PlayerStateSaver.state.money * 0.95)}g for the hospital visit. ";
                editor["Event.cs.1058"] = $"I seem to have lost {PlayerStateSaver.state.money - (int)Math.Round(PlayerStateSaver.state.money * 0.95)}g";
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
                    //Reload asset upon death to reflect amount lost
                    Helper.Content.InvalidateCache("Strings\\StringsFromCSFiles");

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
            else if (PlayerStateSaver.state != null && Game1.CurrentEvent == null && Game1.player.CanMove)
            {
                //capped (lose 500)
                if (PlayerStateSaver.state.money > 10000)
                {
                    PlayerStateSaver.LoadCapped();
                    this.Monitor.Log("Money restored, minus 500g...", LogLevel.Debug);
                }
                //discounted (lose 5%)
                else
                {
                    PlayerStateSaver.LoadDiscounted();
                    this.Monitor.Log("Money restored, excluding 5%...", LogLevel.Debug);
                }

                this.Monitor.Log("Half health restored. You did almost die after all...", LogLevel.Debug);

                //Reset PlayerStateSaver
                PlayerStateSaver.state = null;
            }
        }

    }
}
