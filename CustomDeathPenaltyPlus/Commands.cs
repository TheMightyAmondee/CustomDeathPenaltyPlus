using StardewModdingAPI;

namespace CustomDeathPenaltyPlus
{
    public class Commands
    {
        internal static ModConfig config;
        internal static void SetConfig(ModConfig config)
        {
            Commands.config = config;
        }
        public void DeathPenalty(string[] args, IMonitor monitor, IModHelper helper)
        {
            var dp = config.DeathPenalty;

            switch (args[0])
            {
                case "items":
                case "restoreitems":
                    {
                        try
                        {
                            dp.RestoreItems = bool.Parse(args[1]);
                        }
                        catch
                        {
                            monitor.Log("Value could not be parsed, specify true or false only", LogLevel.Error);
                            break;
                        }
                        monitor.Log($"RestoreItems set to {args[1]}", LogLevel.Info);
                        break;
                    }
                case "cap":
                case "moneylosscap":
                    {
                        try
                        {
                            if (int.Parse(args[1]) < 0)
                            {
                                monitor.Log("Value specified is not in the valid range for MoneyLossCap", LogLevel.Error);
                            }
                            else
                            {
                                dp.MoneyLossCap = int.Parse(args[1]);
                                monitor.Log($"DeathPenalty - MoneyLossCap set to {args[1]}", LogLevel.Info);
                            }
                        }

                        catch
                        {
                            monitor.Log("Value could not be parsed, specify a whole number only", LogLevel.Error);
                            break;
                        }

                        break;
                    }
                case "money":
                case "moneytorestorepercentage":
                    {
                        try
                        {
                            if (double.Parse(args[1]) < 0 || double.Parse(args[1]) > 1)
                            {
                                monitor.Log("Value specified is not in the valid range for MoneytoRestorePercentage", LogLevel.Error);
                            }
                            else
                            {
                                dp.MoneytoRestorePercentage = double.Parse(args[1]);
                                monitor.Log($"DeathPenalty - MoneytoRestorePercentage set to {args[1]}", LogLevel.Info);
                            }
                        }

                        catch
                        {
                            monitor.Log("Value could not be parsed, specify a number only", LogLevel.Error);
                            break;
                        }
                        break;
                    }
                case "health":
                case "healthtorestorepercentage":
                    {
                        try
                        {
                            if (double.Parse(args[1]) < 0 || double.Parse(args[1]) > 1)
                            {
                                monitor.Log("Value specified is not in the valid range for HealthtoRestorePercentage", LogLevel.Error);
                            }
                            else
                            {
                                dp.HealthtoRestorePercentage = double.Parse(args[1]);
                                monitor.Log($"HealthtoRestorePercentage set to {args[1]}", LogLevel.Info);
                            }
                        }

                        catch
                        {
                            monitor.Log("Value could not be parsed, specify a number only", LogLevel.Error);
                            break;
                        }
                        break;
                    }
                case "energy":
                case "energytorestorepercentage":
                    {
                        try
                        {
                            if (double.Parse(args[1]) < 0 || double.Parse(args[1]) > 1)
                            {
                                monitor.Log("Value specified is not in the valid range for EnergytoRestorePercentage", LogLevel.Error);
                            }
                            else
                            {
                                dp.EnergytoRestorePercentage = double.Parse(args[1]);
                                monitor.Log($"DeathPenalty - EnergytoRestorePercentage set to {args[1]}", LogLevel.Info);
                            }
                        }

                        catch
                        {
                            monitor.Log("Value could not be parsed, specify a number only", LogLevel.Error);
                            break;
                        }
                        break;
                    }
                case "friendship":
                case "friendshippenalty":
                    {
                        try
                        {
                            if (int.Parse(args[1]) < 0)
                            {
                                monitor.Log("Value specified is not in the valid range for FriendshipPenalty", LogLevel.Error);
                            }
                            else
                            {
                                dp.FriendshipPenalty = int.Parse(args[1]);
                                monitor.Log($"FriendshipPenalty set to {args[1]}", LogLevel.Info);
                            }
                        }

                        catch
                        {
                            monitor.Log("Value could not be parsed, specify a whole number only", LogLevel.Error);
                            break;
                        }
                        break;
                    }
                case "nextday":
                case "wakeupnextdayinclinic":
                    {
                        try
                        {
                            dp.WakeupNextDayinClinic = bool.Parse(args[1]);
                        }
                        catch
                        {
                            monitor.Log("Value could not be parsed, specify true or false only", LogLevel.Error);
                            break;
                        }
                        monitor.Log($"WakeupNextDayinClinic set to {args[1]}", LogLevel.Info);
                        break;
                    }
                default:
                    {
                        monitor.Log("Invalid config option specified\nAvailable options:\n- restoreitems OR items\n- moneylosscap OR cap\n- moneytorestorepercentage OR money\n- healthtorestorepercentage OR health\n- energytorestorepercentage OR energy\n- friendshippenalty OR friendship\n- wakeupnextdayinclinic OR nextday", LogLevel.Error);
                        break;
                    }
            }
            helper.WriteConfig(config);
        }
        public void PassOutPenalty(string[] args, IMonitor monitor, IModHelper helper)
        {
            var pp = config.PassOutPenalty;

            switch (args[0])
            {
                case "cap":
                case "moneylosscap":
                    {
                        try
                        {
                            if (int.Parse(args[1]) < 0)
                            {
                                monitor.Log("Value specified is not in the valid range for MoneyLossCap", LogLevel.Error);
                            }
                            else
                            {
                                pp.MoneyLossCap = int.Parse(args[1]);
                                monitor.Log($"PassOutPenalty - MoneytoRestorePercentage set to {args[1]}", LogLevel.Info);
                            }
                        }

                        catch
                        {
                            monitor.Log("Value could not be parsed, specify a whole number only", LogLevel.Error);
                            break;
                        }
                        break;
                    }
                case "money":
                case "moneytorestorepercentage":
                    {
                        try
                        {
                            if (double.Parse(args[1]) < 0 || double.Parse(args[1]) > 1)
                            {
                                monitor.Log("Value specified is not in the valid range for MoneytoRestorePercentage", LogLevel.Error);
                            }
                            else
                            {
                                pp.MoneytoRestorePercentage = double.Parse(args[1]);
                                monitor.Log($"PassOutPenalty - MoneytoRestorePercentage set to {args[1]}", LogLevel.Info);
                            }
                        }

                        catch
                        {
                            monitor.Log("Value could not be parsed, specify a number only", LogLevel.Error);
                            break;
                        }
                        break;
                    }
                case "energy":
                case "energytorestorepercentage":
                    {
                        try
                        {
                            if (double.Parse(args[1]) < 0 || double.Parse(args[1]) > 1)
                            {
                                monitor.Log("Value specified is not in the valid range for EnergytoRestorePercentage", LogLevel.Error);
                            }
                            else
                            {
                                pp.EnergytoRestorePercentage = double.Parse(args[1]);
                                monitor.Log($"PassOutPenalty - EnergytoRestorePercentage set to {args[1]}", LogLevel.Info);
                            }
                        }

                        catch
                        {
                            monitor.Log("Value could not be parsed, specify a number only", LogLevel.Error);
                            break;
                        }
                        break;
                    }
                default:
                    {
                        monitor.Log("Invalid config option specified\nAvailable options:\n- moneylosscap OR cap\n- moneytorestorepercentage OR money\n- energytorestorepercentage OR energy", LogLevel.Error);
                        break;
                    }
            }
            helper.WriteConfig(config);
        }

        public void ConfigInfo(string[] args, IMonitor monitor)
        {
            monitor.Log($"Current config settings:" +
                $"\n\nDeathPenalty" +
                $"\n\nRestoreItems: {config.DeathPenalty.RestoreItems.ToString().ToLower()}" +
                $"\nMoneyLossCap: {config.DeathPenalty.MoneyLossCap}" +
                $"\nMoneytoRestorePercentage: {config.DeathPenalty.MoneytoRestorePercentage}" +
                $"\nEnergytoRestorePercentage: {config.DeathPenalty.EnergytoRestorePercentage}" +
                $"\nHealthtoRestorePercentage: {config.DeathPenalty.HealthtoRestorePercentage}" +
                $"\nWakeupNextDayinClinic: {config.DeathPenalty.WakeupNextDayinClinic.ToString().ToLower()}" +
                $"\nFriendshipPenalty: {config.DeathPenalty.FriendshipPenalty}" +
                $"\n\nPassOutPenalty" +
                $"\n\nMoneyLossCap: {config.PassOutPenalty.MoneyLossCap}" +
                $"\nMoneytoRestorePercentage: {config.PassOutPenalty.MoneytoRestorePercentage}" +
                $"\nEnergytoRestorePercentage: {config.PassOutPenalty.EnergytoRestorePercentage}",
                LogLevel.Info);
        }
    }    
}
