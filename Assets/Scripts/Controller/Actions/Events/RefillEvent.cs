using System;
using System.Collections.Generic;
using System.Text;

namespace DiceGame
{
    // Used for testing...
    public class RefillEvent : EventArgs
    {
        public class MoveTileInfo
        {
            public MoveTileInfo(int col, int fromRow, int toRow, string tileValue)
            {
                this.col = col;
                this.fromRow = fromRow;
                this.toRow = toRow;
                this.tileValue = tileValue;
            }
            
            public int col
            {
                get;
                private set;
            }

            public int fromRow
            {
                get;
                private set;
            }
            public int toRow
            {
                get;
                private set;
            }
            public string tileValue
            {
                get;
                private set;
            }            
        }

        public class NewTileInfo
        {
            public NewTileInfo(int col, int row, string tileValue)
            {
                this.col = col;
                this.row = row;
                this.tileValue = tileValue;
            }
            
            public int col
            {
                get;
                private set;
            }

            public int row
            {
                get;
                private set;
            }
            public string tileValue
            {
                get;
                private set;
            }
        }

        public RefillEvent()
        {
            moveTileList = new List<MoveTileInfo>();
            newTileList = new List<NewTileInfo>();
        }
        
        public List<MoveTileInfo> moveTileList
        {
            get;
            private set;
        }
        public List<NewTileInfo> newTileList
        {
            get;
            private set;
        }
    }
}
