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
        public static string string_easynow()
        {
            return i18n.GetTranslation("TheMightyAmondee.CDPP/easynow");
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

        // Translations for mail fragments
        public static string string_mailnocharge()
        {
            return i18n.GetTranslation("TheMightyAmondee.CDPP/mailnocharge");
        }

        public static string string_mailnochargeharvey()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/mailnochargeharvey");
        }

        // Event translations

        public static string event_PlayerKilledMine()
        {
            return i18n.GetTranslation("CDPP.PlayerKilledMine");
        }
        public static string event_PlayerKilledIsland()
        {
            return i18n.GetTranslation("CDPP.PlayerKilledIsland");
        }
        public static string event_PlayerKilledHospital()
        {
            return i18n.GetTranslation("CDPP.PlayerKilledHospital");
        }
        public static string event_PlayerKilledSkullCave()
        {
            return i18n.GetTranslation("CDPP.PlayerKilledSkullCave");
        }

        public static string event_PlayerKilledFarm()
        {
            return i18n.GetTranslation("CDPP.PlayerKilledFarm");
        }

        /*
        public static string string_Caught_NoMoney(string shopkeeper)
        {
            return i18n.GetTranslation($"TheMightyAmondee.Shoplifter/Caught{shopkeeper}_NoMoney");
        }

        public static string string_BanFromShop()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/BanFromShop", new { daysbanned = config.DaysBannedFor });
        }

        public static string string_BanFromShop_Single()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/BanFromShop_Single");
        }

        public static string string_AlreadyShoplifted()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/AlreadyShoplifted", new { shopliftingamount = config.MaxShopliftsPerDay });
        }

        public static string string_AlreadyShoplifted_Single()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/AlreadyShoplifted_Single");
        }

        public static string string_AlreadyShopliftedSameShop()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/AlreadyShopliftedSameShop");
        }
        */

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

