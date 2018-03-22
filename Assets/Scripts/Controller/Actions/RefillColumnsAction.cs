using System;
using System.Text;

namespace DiceGame
{
    public class RefillColumnsAction : IGameAction
    {
        private string[] _tilesToChooseFrom;
        private Random _random = new Random();

        public RefillColumnsAction(string[] tilesToChooseFrom)
        {
            _tilesToChooseFrom = tilesToChooseFrom;
        }

        void IGameAction.Execute(GameState state, EventHandler eventHandler)
        {
            RefillEvent refillEvent = new RefillEvent();

            for (int col = 0; col < state.width; ++col) 
            {
                //
                // Move tiles to fill empty spaces below them
                bool didMove = false;
                do {
                    didMove = false;

                    for (int row = state.height-1; row > 0; --row)
                    {
                        if (state[col, row] == "e")
                        {
                            for (int above = row-1; (didMove == false) && above >= 0; --above)
                            {
                                if (state[col, above] != "e")
                                {
                                    state[col, row] = state[col, above];
                                    state[col, above] = "e";
                                    didMove = true;

                                    refillEvent.moveTileList.Add(new RefillEvent.MoveTileInfo(col, above, row, state[col, row]));
                                }
                            }
                        }
                    }
                } while (didMove);

                //
                // Create new tiles to fill empty spaces at top of column
                bool didNew = false;
                do {
                    didNew = false;

                    for (int row = state.height-1; row >= 0; --row)
                    {
                        if (state[col, row] == "e")
                        {
                            state[col, row] = _RandomTile();
                            didNew = true;

                            refillEvent.newTileList.Add(new RefillEvent.NewTileInfo(col, row, state[col, row]));
                        }
                    }
                } while (didNew);
            }

            if (eventHandler != null)
                eventHandler(this, refillEvent);
        }

        private string _RandomTile()
        {
            int randomIndex = _random.Next(0, this._tilesToChooseFrom.Length);
            return this._tilesToChooseFrom[randomIndex];
        }
    }
}
