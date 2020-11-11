using StardewModdingAPI;
using StardewValley;
using System;

namespace CustomDeathPenaltyPlus
{
    /// <summary>
    /// Edits game assets
    /// </summary>
    internal class AssetEditor
    {
        private static ModConfig config;

        // Allows the class to access the ModConfig properties
        public static void SetConfig(ModConfig config)
        {
            AssetEditor.config = config;
        }

        /// <summary>
        /// Edits content in Strings/StringsFromCSFiles
        /// </summary>
        public class StringsFromCSFilesFixes : IAssetEditor
        {
            private IModHelper modHelper;

            public StringsFromCSFilesFixes(IModHelper helper)
            {
                modHelper = helper;
            }

            // Allow asset to be editted if name matches and any object references exist
            public bool CanEdit<T>(IAssetInfo asset)
            {
                return asset.AssetNameEquals("Strings\\StringsFromCSFiles") && PlayerStateRestorer.statedeath != null;
            }

            // Edit asset
            public void Edit<T>(IAssetData asset)
            {
                var stringeditor = asset.AsDictionary<string, string>().Data;

                // Has player not lost any money?
                if (config.DeathPenalty.MoneyLossCap == 0 || config.DeathPenalty.MoneytoRestorePercentage == 1)
                {
                    // Yes, edit strings to show this special case
                    stringeditor["Event.cs.1068"] = "Dr. Harvey didn't charge me for the hospital visit, how nice. ";
                    stringeditor["Event.cs.1058"] = "Fortunately, I still have all my money";
                }
                else
                {
                    // No, edit strings to show amount lost
                    stringeditor["Event.cs.1068"] = $"Dr. Harvey charged me {(int)Math.Round(PlayerStateRestorer.statedeath.moneylost)}g for the hospital visit. ";
                    stringeditor["Event.cs.1058"] = $"I seem to have lost {(int)Math.Round(PlayerStateRestorer.statedeath.moneylost)}g";
                }

                // Is RestoreItems true?
                if (config.DeathPenalty.RestoreItems == true)
                {
                    // Yes, Remove unnecessary strings
                    stringeditor["Event.cs.1060"] = "";
                    stringeditor["Event.cs.1061"] = "";
                    stringeditor["Event.cs.1062"] = "";
                    stringeditor["Event.cs.1063"] = "";
                    stringeditor["Event.cs.1071"] = "";
                }
            }
        }
        /// <summary>
        /// Edits content in Data/mail
        /// </summary>
        public class MailDataFixes : IAssetEditor
        {
            private IModHelper modHelper;

            public MailDataFixes(IModHelper helper)
            {
                modHelper = helper;
            }

            // Allow asset to be editted if name matches
            public bool CanEdit<T>(IAssetInfo asset)
            {
                return asset.AssetNameEquals("Data\\mail");
            }

            // Edit asset
            public void Edit<T>(IAssetData asset)
            {
                var maileditor = asset.AsDictionary<string, string>().Data;

                var data = ModEntry.PlayerData;

                // Has player not lost any money?
                if (data.MoneyLostLastPassOut == 0)
                {
                    // Yes, edit strings to show this special case
                    maileditor["passedOut1_Billed_Male"] = "Dear Mr. @,^Last night, a Joja team member found you incapacitated. A medical team was dispatched to bring you home safely.^We're glad you're okay!^^(Be thankful you haven't been billed for this service)^^-Morris^Joja Customer Satisfaction Representative[#]Joja Invoice";
                    maileditor["passedOut1_Billed_Female"] = "Dear Ms. @,^Last night, a Joja team member found you incapacitated. A medical team was dispatched to bring you home safely.^We're glad you're okay!^^(Be thankful you haven't been billed for this service)^^-Morris^Joja Customer Satisfaction Representative[#]Joja Invoice";
                    maileditor["passedOut3_Billed"] = "@,^Someone dropped you off at the clinic last night. You'd passed out from exhaustion!^You've got to take better care of yourself and go to bed at a reasonable hour.^I haven't billed you for your medical expenses this time.^^-Dr. Harvey[#]From The Office Of Dr. Harvey";
                }

                else
                {
                    // No, edit strings to show amount lost
                    maileditor["passedOut1_Billed_Male"] = $"Dear Mr. @,^Last night, a Joja team member found you incapacitated. A medical team was dispatched to bring you home safely.^We're glad you're okay!^^(You've been billed {data.MoneyLostLastPassOut}g for this service)^^-Morris^Joja Customer Satisfaction Representative[#]Joja Invoice";
                    maileditor["passedOut1_Billed_Female"] = $"Dear Ms. @,^Last night, a Joja team member found you incapacitated. A medical team was dispatched to bring you home safely.^We're glad you're okay!^^(You've been billed {data.MoneyLostLastPassOut}g for this service)^^-Morris^Joja Customer Satisfaction Representative[#]Joja Invoice";
                    maileditor["passedOut3_Billed"] = $"@,^Someone dropped you off at the clinic last night. You'd passed out from exhaustion!^You've got to take better care of yourself and go to bed at a reasonable hour.^I've billed you {data.MoneyLostLastPassOut}g to cover your medical expenses.^^-Dr. Harvey[#]From The Office Of Dr. Harvey";
                }
            }
        }

        /// <summary>
        /// Edits content in Data/Events/Mine
        /// </summary>
        public class MineEventFixes : IAssetEditor
        {
            private IModHelper modHelper;

            public MineEventFixes(IModHelper helper)
            {
                modHelper = helper;
            }

            // Allow asset to be editted if name matches
            public bool CanEdit<T>(IAssetInfo asset)
            {
                return asset.AssetNameEquals("Data\\Events\\Mine");
            }

            // Edit asset
            public void Edit<T>(IAssetData asset)
            {
                var eventedits = asset.AsDictionary<string, string>().Data;

                // Is WakeupNextDayinClinic true?
                if(config.DeathPenalty.WakeupNextDayinClinic == true)
                {
                    //Yes, content can be editted
                    eventedits["PlayerKilled"] = "none/-100 -100/farmer 20 12 2 Harvey 21 12 3/changeLocation Hospital/pause 500/showFrame 5/message \" ...{2}?\"/pause 1000/message \"Easy, now... take it slow.\"/viewport 20 12 true/pause 1000/speak Harvey \"Good, you're finally awake. {0} found you unconscious in the mine... I had to perform an emergency surgery on you!#$b#Be a little more careful next time, okay?$s\"/showFrame 0/pause 1000/emote farmer 28/hospitaldeath/end";
                }
            }
        }

        /// <summary>
        /// Edits content in Data/Events/Hospital
        /// </summary>
        public class HospitalEventFixes : IAssetEditor
        {
            private IModHelper modHelper;

            public HospitalEventFixes(IModHelper helper)
            {
                modHelper = helper;
            }

            // Allow asset to be editted if name matches
            public bool CanEdit<T>(IAssetInfo asset)
            {
                return asset.AssetNameEquals("Data\\Events\\Hospital");
            }

            // Edit asset
            public void Edit<T>(IAssetData asset)
            {
                var eventedits = asset.AsDictionary<string, string>().Data;

                // Is WakeupNextDayinClinic true?
                if (config.DeathPenalty.WakeupNextDayinClinic == true)
                {
                    // Yes, content can be editted
                    eventedits["PlayerKilled"] = "none/-100 -100/farmer 20 12 2 Harvey 21 12 3/pause 1500/showFrame 5/message \" ...{0}?\"/pause 1000/message \"Easy, now... take it slow.\"/viewport 20 12 true/pause 1000/speak Harvey \"Good, you're finally awake. Someone found you unconscious and battered... I had to perform an emergency surgery on you!#$b#Be a little more careful next time, okay?$s\"/showFrame 0/pause 1000/emote farmer 28/hospitaldeath/end";
                }
            }
        }
    }
}
