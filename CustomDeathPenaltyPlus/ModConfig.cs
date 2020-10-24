using System;

namespace CustomDeathPenaltyPlus
{
    internal class ModConfig
    {
        public bool RestoreItems { get; set; } = true;
        public int MoneyLossCap { get; set; } = 500;
        public double MoneytoRestorePercentage { get; set; } = 0.95;
        public double EnergytoRestorePercentage { get; set; } = 0.10;
        public double HealthtoRestorePercentage { get; set; } = 0.50;
        public int PassOutMoneyLossCap { get; set; } = 500;
        public double PassOutMoneytoRestorePercentage { get; set; } = 0.95;
        public double PassOutEnergytoRestorePercentage { get; set; } = 0.50;

    }
}
