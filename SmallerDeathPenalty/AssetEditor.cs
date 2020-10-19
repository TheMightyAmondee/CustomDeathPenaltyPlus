using System;
using StardewModdingAPI;

namespace SmallerDeathPenalty
{
    /// <summary>
    /// Edits game assets
    /// </summary>
    class AssetEditor
    {
        private static ModConfig config;

        public static void SetConfig(ModConfig config)
        {
            AssetEditor.config = config;
        }
        /// <summary>
        /// Edits strings in the UI
        /// </summary>
        public class UIFixes : IAssetEditor
        {
            private IModHelper modHelper;

            public UIFixes(IModHelper helper)
            {
                modHelper = helper;
            }
            //Allow asset to be editted if name matches
            public bool CanEdit<T>(IAssetInfo asset)
            {
                if (asset.AssetNameEquals("Strings\\UI"))
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
                var UIeditor = asset.AsDictionary<string, string>().Data;

                UIeditor["ItemList_ItemsLost"] = "Items recovered:";
            }
        }

        /// <summary>
        /// Edits strings in StringsFromCSFiles
        /// </summary>
        public class StringsFromCSFilesFixes : IAssetEditor
        {
            private IModHelper modHelper;

            public StringsFromCSFilesFixes(IModHelper helper)
            {
                modHelper = helper;
            }
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

                //Edit strings to reflect restored money
                else
                {
                    if (config.MoneyLossCap == 0 || config.MoneytoRestorePercentage == 1)
                    {
                        editor["Event.cs.1068"] = "Dr. Harvey didn't charge me for the hospital visit, how nice. ";
                        editor["Event.cs.1058"] = "Fortunately, I still have all my money";
                    }
                    else if (PlayerStateSaver.state.money * (1 - config.MoneytoRestorePercentage) > config.MoneyLossCap)
                    {
                        //Edit events to reflect capped amount lost
                        editor["Event.cs.1068"] = $"Dr. Harvey charged me {config.MoneyLossCap}g for the hospital visit. ";
                        editor["Event.cs.1058"] = $"I seem to have lost {config.MoneyLossCap}g";
                    }
                    else
                    {
                        //Edit events to reflect discounted amount lost
                        editor["Event.cs.1068"] = $"Dr. Harvey charged me {PlayerStateSaver.state.money - (int)Math.Round(PlayerStateSaver.state.money * config.MoneytoRestorePercentage)}g for the hospital visit. ";
                        editor["Event.cs.1058"] = $"I seem to have lost {PlayerStateSaver.state.money - (int)Math.Round(PlayerStateSaver.state.money * config.MoneytoRestorePercentage)}g";
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
        }
    }
}
