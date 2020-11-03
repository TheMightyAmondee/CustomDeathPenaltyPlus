using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomDeathPenaltyPlus
{
    public class PlayerData
    {
        public bool DidPlayerPassOutYesterday { get; set; }
        public int MoneyLostLastPassOut { get; set; }
        public bool DidPlayerWakeupinClinic { get; set; }
    }
}
