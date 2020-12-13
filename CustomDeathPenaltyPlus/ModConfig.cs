using System;
using System.Collections;

namespace CustomDeathPenaltyPlus
{
    internal class ModConfig
    {
        public DeathPenaltyChanges DeathPenalty { get; set; } = new DeathPenaltyChanges();

        public PassOutPenaltyChanges PassOutPenalty { get; set; } = new PassOutPenaltyChanges();

        internal class DeathPenaltyChanges
        {
            public bool RestoreItems { get; set; } = true;
            public int MoneyLossCap { get; set; } = 500;
            public double MoneytoRestorePercentage { get; set; } = 0.95;
            public double EnergytoRestorePercentage { get; set; } = 0.10;
            public double HealthtoRestorePercentage { get; set; } = 0.50;
            public bool WakeupNextDayinClinic { get; set; } = false;
            public int FriendshipPenalty { get; set; } = 0;
        }

        internal class PassOutPenaltyChanges
        {
            public int MoneyLossCap { get; set; } = 500;
            public double MoneytoRestorePercentage { get; set; } = 0.95;
            public double EnergytoRestorePercentage { get; set; } = 0.50;
        }
    }
}
