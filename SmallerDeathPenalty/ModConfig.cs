using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallerDeathPenalty
{
    internal class ModConfig
    {
        public bool RestoreItems { get; set; } = true;
        public int MoneyLossCap { get; set; } = 500;
        public double RestoreMoneyPercentage { get; set; } = 0.95;
        public double EnergytoRestorePercentage { get; set; } = 0.10;
        public double HealthtoRestorePercentage { get; set; } = 0.50;

    }
}
