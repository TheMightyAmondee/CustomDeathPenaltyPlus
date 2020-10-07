using StardewValley;
using StardewValley.Tools;
using System.Collections.Generic;

namespace SmallerDeathPenalty
{
    //Tracks player's funds
    internal class PlayerState
    {
        public int money;

        public PlayerState(int m)
        {
            this.money = m;
        }
    }
}
