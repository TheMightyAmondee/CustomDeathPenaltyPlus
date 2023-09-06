using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;

namespace CustomDeathPenaltyPlus
{
    /// <summary>
    /// Class for determining translations
    /// </summary>
    internal static class i18n
    {
        private static ITranslationHelper translation;
        private static ModConfig config;
        public static void gethelpers(ITranslationHelper translation, ModConfig config)
        {
            i18n.translation = translation;
            i18n.config = config;
        }

        // Translations for string fragments
        public static string string_responseperson()
        {
            return i18n.GetTranslation("TheMightyAmondee.CDPP/responseperson");
        }
        public static string string_responseappendix1()
        {
            return i18n.GetTranslation("TheMightyAmondee.CDPP/responseappendix1");
        }
        public static string string_responseappendix2()
        {
            return i18n.GetTranslation("TheMightyAmondee.CDPP/responseappendix2");
        }
        public static string string_responseappendix3()
        {
            return i18n.GetTranslation("TheMightyAmondee.CDPP/responseappendix3");
        }
        public static string string_finallyawake()
        {
            return i18n.GetTranslation("TheMightyAmondee.CDPP/finallyawake");
        }

        // Translations for event fragments
        public static string string_nomoneylost()
        {
            return i18n.GetTranslation("TheMightyAmondee.CDPP/nomoneylost");
        }
        public static string string_nomoneylostmine()
        {
            return i18n.GetTranslation("TheMightyAmondee.CDPP/nomoneylostmine");
        }
        public static string string_nocharge()
        {
            return i18n.GetTranslation("TheMightyAmondee.CDPP/nocharge");
        }
        public static string string_nomoney()
        {
            return i18n.GetTranslation("TheMightyAmondee.CDPP/nomoney");
        }
        public static string string_becareful()
        {
            return i18n.GetTranslation("TheMightyAmondee.CDPP/becareful");
        }
        public static string string_bereallycareful()
        {
            return i18n.GetTranslation("TheMightyAmondee.CDPP/bereallycareful");
        }
        public static string string_nicetoseeyou()
        {
            return i18n.GetTranslation("TheMightyAmondee.CDPP/nicetoseeyou");
        }
        public static string string_wakeplayer()
        {
            return i18n.GetTranslation("TheMightyAmondee.CDPP/wakeplayer");
        }
        public static string string_easynow1()
        {
            return i18n.GetTranslation("TheMightyAmondee.CDPP/easynow1");
        }
        public static string string_easynow2()
        {
            return i18n.GetTranslation("TheMightyAmondee.CDPP/easynow2");
        }
        public static string string_qi()
        {
            return i18n.GetTranslation("TheMightyAmondee.CDPP/qi");
        }
        public static string string_whathappened()
        {
            return i18n.GetTranslation("TheMightyAmondee.CDPP/whathappened");
        }
        public static string string_somethingbad()
        {
            return i18n.GetTranslation("TheMightyAmondee.CDPP/somethingbad");
        }
        public static string string_moneylost()
        {
            var lost = (int)Math.Round(PlayerStateRestorer.statedeathps.Value.moneylost);
            return i18n.GetTranslation("TheMightyAmondee.CDPP/moneylost", new { moneylost = lost });
        }

        // Translations for replacements
        public static string string_replacementdialogue()
        {
            return i18n.GetTranslation("TheMightyAmondee.CDPP/replacementdialogue");
        }
        public static string string_replacementmail1()
        {
            return i18n.GetTranslation("TheMightyAmondee.CDPP/replacementmail1");
        }
        public static string string_replacementmail2()
        {
            return i18n.GetTranslation("TheMightyAmondee.CDPP/replacementmail1");
        }

        // Translations for mail fragments
        public static string string_mailnocharge()
        {
            return i18n.GetTranslation("TheMightyAmondee.CDPP/mailnocharge");
        }

        public static string string_mailnochargeharvey()
        {
            return i18n.GetTranslation("TheMightyAmondee.CDPP/mailnochargeharvey");
        }

        // GMCM translations

        public static string GMCM_DeathPenalty()
        {
            return i18n.GetTranslation("CDPP.GMCM_DeathPenalty");
        }
        public static string GMCM_PassOutPenalty()
        {
            return i18n.GetTranslation("CDPP.GMCM_PassOutPenalty");
        }
        public static string GMCM_OtherPenalty()
        {
            return i18n.GetTranslation("CDPP.GMCM_OtherPenalties");
        }
        public static string GMCM_RestoreItems()
        {
            return i18n.GetTranslation("CDPP.GMCM_RestoreItems");
        }
        public static string GMCM_MoneyLossCap()
        {
            return i18n.GetTranslation("CDPP.GMCM_MoneyLossCap");
        }
        public static string GMCM_MoneytoRestorePercentage()
        {
            return i18n.GetTranslation("CDPP.GMCM_MoneytoRestorePercentage");
        }
        public static string GMCM_EnergytoRestorePercentage()
        {
            return i18n.GetTranslation("CDPP.GMCM_EnergytoRestorePercentage");
        }
        public static string GMCM_HealthtoRestorePercentage()
        {
            return i18n.GetTranslation("CDPP.GMCM_HealthtoRestorePercentage");
        }
        public static string GMCM_WakeupNextDayinClinic()
        {
            return i18n.GetTranslation("CDPP.GMCM_WakeupNextDayinClinic");
        }
        public static string GMCM_HarveyFriendshipChange()
        {
            return i18n.GetTranslation("CDPP.GMCM_HarveyFriendshipChange");
        }
        public static string GMCM_MaruFriendshipChange()
        {
            return i18n.GetTranslation("CDPP.GMCM_MaruFriendshipChange");
        }
        public static string GMCM_MoreRealisticWarps()
        {
            return i18n.GetTranslation("CDPP.GMCM_MoreRealisticWarps");
        }
        public static string GMCM_DebuffonDeath()
        {
            return i18n.GetTranslation("CDPP.GMCM_DebuffonDeath");
        }


        /// <summary>
        /// Gets the correct translation
        /// </summary>
        /// <param name="key">The translation key</param>
        /// <param name="tokens">Tokens, if any</param>
        /// <returns></returns>
        public static Translation GetTranslation(string key, object tokens = null)
        {
            if (i18n.translation == null)
            {
                throw new InvalidOperationException($"You must call {nameof(i18n)}.{nameof(i18n.gethelpers)} from the mod's entry method before reading translations.");
            }

            return i18n.translation.Get(key, tokens);
        }
    }
}

