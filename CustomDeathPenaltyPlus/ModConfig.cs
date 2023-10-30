
namespace CustomDeathPenaltyPlus
{
    internal class ModConfig
    {
        public DeathPenaltyChanges DeathPenalty { get; set; } = new DeathPenaltyChanges();

        public PassOutPenaltyChanges PassOutPenalty { get; set; } = new PassOutPenaltyChanges();

        public OtherChanges OtherPenalties { get; set; } = new OtherChanges();

        internal class DeathPenaltyChanges
        {
            public bool RestoreItems { get; set; } = true;
            public int MoneyLossCap { get; set; } = 500;
            public float MoneytoRestorePercentage { get; set; } = 0.95f;
            public float EnergytoRestorePercentage { get; set; } = 0.10f;
            public float HealthtoRestorePercentage { get; set; } = 0.50f;
        }

        internal class PassOutPenaltyChanges
        {
            public int MoneyLossCap { get; set; } = 500;
            public float MoneytoRestorePercentage { get; set; } = 0.95f;
            public float EnergytoRestorePercentage { get; set; } = 0.50f;
        }

        internal class OtherChanges
        {
            public bool WakeupNextDayinClinic { get; set; } = false;
            public int HarveyFriendshipChange { get; set; } = 0;
            public int MaruFriendshipChange { get; set; } = 0;
            public bool MoreRealisticWarps { get; set; } = false;
            public bool DebuffonDeath { get; set; } = false;
        }
    }
}
