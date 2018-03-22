using System;
using System.Collections.Generic;
using System.Text;

namespace DiceGame
{
    public class GameState
    {
        public class TileLocation
        {
            public TileLocation(int column, int row)
            {
                this.column = column;
                this.row = row;
            }

            public int column { get; set; }
            public int row { get; set; }
        }

        // What tiles are currenly on the board
        string[,] _tiles;

        public GameState(int boardWidth, int boardHeight)
        {
            _tiles = new string[boardWidth, boardHeight];
        }

        public int width
        {
            get { return _tiles.GetLength(0); }
        }

        public int height
        {
            get { return _tiles.GetLength(1); }
        }

        public string this[int width, int height]
        {
            get { return _tiles[width, height]; }
            set { _tiles[width, height] = value; }
        }

        public string BoardAsString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int row = 0; row < this.height; ++row)
            {
                for (int col = 0; col < this.width; ++col) 
                {
                    stringBuilder.Append(String.Format("{0,3}", _tiles[col,row])).Append(" ");
                }
                stringBuilder.Append("\n");
            }

            return stringBuilder.ToString();
        }
    }
}
