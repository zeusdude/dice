using System;
using System.Text;

namespace DiceGame
{
    public class ShootVictimEvent : EventArgs
    {
        public ShootVictimEvent(int shooterCol, int shooterRow, int victimCol, int victimRow)
        {
            this.shooterCol = shooterCol;
            this.shooterRow = shooterRow;
            this.victimCol = victimCol;
            this.victimRow = victimRow;
        }
        
        public int shooterCol
        {
            get;
            private set;
        }

        public int shooterRow
        {
            get;
            private set;
        }
        
        public int victimCol
        {
            get;
            private set;
        }

        public int victimRow
        {
            get;
            private set;
        }
    }
}
