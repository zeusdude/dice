using System;
using System.Collections.Generic;
using System.Text;

namespace DiceGame
{
    // Used for testing...
    public class ActivateTileEvent : EventArgs
    {
        public class ShootVictimInfo
        {
            public ShootVictimInfo(int shooterCol, int shooterRow, int victimCol, int victimRow)
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

        public class RemovePipsInfo
        {
            public RemovePipsInfo(int tileCol, int tileRow)
            {
                this.tileCol = tileCol;
                this.tileRow = tileRow;
            }
            
            public int tileCol
            {
                get;
                private set;
            }

            public int tileRow
            {
                get;
                private set;
            }
        }

        public ActivateTileEvent()
        {
            shootVictimList = new List<ShootVictimInfo>();
            removePipsInfo = null;
            score = 0;
        }
        
        public List<ShootVictimInfo> shootVictimList
        {
            get;
            private set;
        }
        
        // This may be null
        public RemovePipsInfo removePipsInfo
        {
            get;
            set;
        }
        public int score
        {
            get;
            set;
        }
    }
}
