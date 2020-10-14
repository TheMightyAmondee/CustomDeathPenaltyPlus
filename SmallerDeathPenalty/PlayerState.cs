using StardewValley;
using StardewValley.Tools;
using System.Collections.Generic;

namespace SmallerDeathPenalty
{
/// <summary>
/// Tracks the players state
/// </summary>
    internal class PlayerState
    {
        public double money;

        public PlayerState(double m)
        {
            this.money = m;
        }
    }
}
