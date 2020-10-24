using StardewModdingAPI;
using System;

namespace CustomDeathPenaltyPlus
{
    /// <summary>
    /// Edits game assets
    /// </summary>
    internal class AssetEditor
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
                return asset.AssetNameEquals("Strings\\UI");
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

            //Allow asset to be editted if name matches and any object references exist
            public bool CanEdit<T>(IAssetInfo asset)
            {
                return asset.AssetNameEquals("Strings\\StringsFromCSFiles") && PlayerStateSaver.state != null;
            }

            //Edit asset
            public void Edit<T>(IAssetData asset)
            {
                var editor = asset.AsDictionary<string, string>().Data;
                //Special case when no money is lost
                if (config.MoneyLossCap == 0 || config.MoneytoRestorePercentage == 1)
                {
                    editor["Event.cs.1068"] = "Dr. Harvey didn't charge me for the hospital visit, how nice. ";
                    editor["Event.cs.1058"] = "Fortunately, I still have all my money";
                }
                //Edit events to reflect amount lost
                else
                {
                    editor["Event.cs.1068"] = $"Dr. Harvey charged me {(int)Math.Round(PlayerStateSaver.state.moneylost)}g for the hospital visit. ";
                    editor["Event.cs.1058"] = $"I seem to have lost {(int)Math.Round(PlayerStateSaver.state.moneylost)}g";
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

        public class MailDataFixes : IAssetEditor
        {
            private IModHelper modHelper;

            public MailDataFixes(IModHelper helper)
            {
                modHelper = helper;
            }

            public bool CanEdit<T>(IAssetInfo asset)
            {
                return asset.AssetNameEquals("Data\\mail") && PlayerStateSaver.statepassout != null;
            }

            public void Edit<T>(IAssetData asset)
            {
                var editor = asset.AsDictionary<string, string>().Data;
                //Special case when no money is lost
                if (config.PassOutMoneyLossCap == 0 || config.PassOutMoneytoRestorePercentage == 1)
                {
                    editor["passedOut1_Billed_Male"] = "Dear Mr. @,^Last night, a Joja team member found you incapacitated. A medical team was dispatched to bring you home safely.^We're glad you're okay!^^(Be thankful you haven't been billed for this service)^^-Morris^Joja Customer Satisfaction Representative[#]Joja Invoice";
                    editor["passedOut1_Billed_Female"] = "Dear Ms. @,^Last night, a Joja team member found you incapacitated. A medical team was dispatched to bring you home safely.^We're glad you're okay!^^(Be thankful you haven't been billed for this service)^^-Morris^Joja Customer Satisfaction Representative[#]Joja Invoice";
                    editor["passedOut3_Billed"] = "@,^Someone dropped you off at the clinic last night. You'd passed out from exhaustion!^You've got to take better care of yourself and go to bed at a reasonable hour.^I haven't billed you for you're medical expenses this time.^^-Dr. Harvey[#]From The Office Of Dr. Harvey";
                }
                //Edit mail to reflect amount lost
                else
                {
                    editor["passedOut1_Billed_Male"] = $"Dear Mr. @,^Last night, a Joja team member found you incapacitated. A medical team was dispatched to bring you home safely.^We're glad you're okay!^^(You've been billed {(int)Math.Round(PlayerStateSaver.statepassout.moneylost)}g for this service)^^-Morris^Joja Customer Satisfaction Representative[#]Joja Invoice";
                    editor["passedOut1_Billed_Female"] = $"Dear Ms. @,^Last night, a Joja team member found you incapacitated. A medical team was dispatched to bring you home safely.^We're glad you're okay!^^(You've been billed {(int)Math.Round(PlayerStateSaver.statepassout.moneylost)}g for this service)^^-Morris^Joja Customer Satisfaction Representative[#]Joja Invoice";
                    editor["passedOut3_Billed"] = $"@,^Someone dropped you off at the clinic last night. You'd passed out from exhaustion!^You've got to take better care of yourself and go to bed at a reasonable hour.^I've billed you {(int)Math.Round(PlayerStateSaver.statepassout.moneylost)}g to cover your medical expenses.^^-Dr. Harvey[#]From The Office Of Dr. Harvey";
                }
            }
        }
    }
}
