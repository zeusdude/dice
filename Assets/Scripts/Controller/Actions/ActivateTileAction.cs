using System;
using System.Diagnostics;
using System.Text;

namespace DiceGame
{
    public class ActivateTileAction : IGameAction
    {
        private int _col;
        private int _row;

        public ActivateTileAction(int col, int row)
        {
          this._col = col;
          this._row = row;
        }

        void IGameAction.Execute(GameState state, EventHandler eventHandler)
        {
            ActivateTileEvent activateTileEvent = new ActivateTileEvent();
            activateTileEvent.score = 0;

            string tileChosen = state[this._col, this._row];
            char [,] pattern = this._GetPattern(tileChosen);

            Trace.WriteLine(String.Format("Activate {0} at {1},{2}", tileChosen, this._col, this._row));
            Trace.WriteLine(_PatternAsString(pattern));

            // Multipliers (2x, 3x, ...)
            //   If you activate a die that shoots another die and the victim is the same die type as the shooter then
            //   you get a multiplier (2x), hit another in the same activation (3x), etc...
            //
            //   Hitting a multiplier on an activation does what you expect, it multiplies the score you get on that
            //   activation.  Beware, hitting ONLY multipliers gets ZERO pts.
            int currentMultiplier = 2;
            int scoreMultiplier = 1;

            //
            // Destroy tiles based on pattern
            char multiplierTestChar = state[this._col, this._row][0];

            for (int col = 0; col < pattern.GetLength(0); ++col)
            {
                for (int row = 0; row < pattern.GetLength(1); ++row)
                {
                    if (pattern[col, row] == 'x')
                    {
                        int boardPosCol = this._col + (col-1);
                        int boardPosRow = this._row + (row-1);

                        if (boardPosCol >= 0 && boardPosCol < state.width && boardPosRow >= 0 && boardPosRow < state.height)
                        {
                            bool isSelf = (this._col == boardPosCol && this._row == boardPosRow);

                            // Only test first char, a '6' is a '6' regardless of orientation...
                            if ( ! isSelf && multiplierTestChar == state[boardPosCol, boardPosRow][0])
                            {
                                // "x" needs to go first to avoid matching this if-check on next pip hit...
                                state[boardPosCol, boardPosRow] = "x" + currentMultiplier.ToString();
                                ++currentMultiplier;
                            }
                            else
                            {
                                char scoreChar = state[boardPosCol, boardPosRow][0];
                                if (Char.IsNumber(scoreChar))
                                {
                                    int scoreCharAsInt = scoreChar - '0';
                                    activateTileEvent.score += scoreCharAsInt * 10;
                                }
                                else if (scoreChar == 'x')
                                {
                                    char multiplierChar = state[boardPosCol, boardPosRow][1];
                                    int multiplierCharAsInt = multiplierChar - '0';
                                    scoreMultiplier *= multiplierCharAsInt;
                                }

                                state[boardPosCol, boardPosRow] = "e"; // Empty
                            }

                            if (eventHandler != null)
                            {
                                activateTileEvent.shootVictimList.Add(new ActivateTileEvent.ShootVictimInfo(this._col, this._row, boardPosCol, boardPosRow));
                            }
                        }
                    }
                }
            }

            //
            // Deal with a blank die.  This happens when tileChosen returns a pattern of '-' for the center
            if (pattern[1,1] == '-')
            {
                char scoreChar = state[this._col, this._row][0];
                if (Char.IsNumber(scoreChar))
                {
                    int scoreCharAsInt = scoreChar - '0';
                    activateTileEvent.score += scoreCharAsInt * 10;
                }

                state[this._col, this._row] = "b"; // Blank

                if (eventHandler != null)
                {
                    activateTileEvent.removePipsInfo = new ActivateTileEvent.RemovePipsInfo(this._col, this._row);
                }
            }

            activateTileEvent.score *= scoreMultiplier;
            eventHandler(this, activateTileEvent);
        }

        private string _PatternAsString(char[,] pattern)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int row = 0; row < pattern.GetLength(1); ++row)
            {
                for (int col = 0; col < pattern.GetLength(0); ++col) 
                {
                    stringBuilder.Append(pattern[col,row]).Append(" ");
                }
                stringBuilder.Append("\n");
            }

            return stringBuilder.ToString();
        }

        private char [,] _GetPattern(string tileChosen)
        {
            //
            // Legend:
            //   '-': Do Nothing
            //   'x': Shoot tile in this location.  Center position of pattern is tile that was activated.
            //
            char[,] pattern = new char[,] {
                {'-', '-', '-'},
                {'-', '-', '-'},
                {'-', '-', '-'}
            };

            switch (tileChosen)
            {
                case "1":
                    pattern[1,1] = 'x';
                break;

                case "2hl":
                    pattern[0,0] = 'x';
                    pattern[2,2] = 'x';
                break;

                case "2lh":
                    pattern[0,2] = 'x';
                    pattern[2,0] = 'x';
                break;

                case "3hl":
                    pattern[0,0] = 'x';
                    pattern[1,1] = 'x';
                    pattern[2,2] = 'x';
                break;

                case "3lh":
                    pattern[0,2] = 'x';
                    pattern[1,1] = 'x';
                    pattern[2,0] = 'x';
                break;

                case "4":
                    pattern[0,0] = 'x';
                    pattern[2,2] = 'x';
                    pattern[0,2] = 'x';
                    pattern[2,0] = 'x';
                break;

                case "5":
                    pattern[0,0] = 'x';
                    pattern[2,2] = 'x';
                    pattern[1,1] = 'x';
                    pattern[0,2] = 'x';
                    pattern[2,0] = 'x';
                break;

                case "6h":
                    pattern[0,0] = 'x';
                    pattern[1,0] = 'x';
                    pattern[2,0] = 'x';

                    pattern[0,2] = 'x';
                    pattern[1,2] = 'x';
                    pattern[2,2] = 'x';
                break;

                case "6v":
                    pattern[0,0] = 'x';
                    pattern[0,1] = 'x';
                    pattern[0,2] = 'x';

                    pattern[2,0] = 'x';
                    pattern[2,1] = 'x';
                    pattern[2,2] = 'x';
                break;
            }

            return pattern;
        }
    }
}
