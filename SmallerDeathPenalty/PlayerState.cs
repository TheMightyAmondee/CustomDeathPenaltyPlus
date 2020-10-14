using StardewValley;
using StardewValley.Tools;
using System.Collections.Generic;

namespace SmallerDeathPenalty
{
    //Stores player's funds
    //Double type is necessary for calculating money loss
    internal class PlayerState
    {
        public double money;

        public PlayerState(double m)
        {
            this.money = m;
        }
    }
}
