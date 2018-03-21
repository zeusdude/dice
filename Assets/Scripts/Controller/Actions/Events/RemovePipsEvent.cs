using System;
using System.Text;

namespace DiceGame
{
    // Used for testing...
    public class RemovePipsEvent : EventArgs
    {
        public RemovePipsEvent(int tileCol, int tileRow)
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
}
