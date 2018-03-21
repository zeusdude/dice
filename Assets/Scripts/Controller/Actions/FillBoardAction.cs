using System;
using System.Text;

namespace DiceGame
{
    public class FillBoardAction : IGameAction
    {
        private string[] _tilesToChooseFrom;
        private Random _random = new Random();

        public FillBoardAction(string[] tilesToChooseFrom)
        {
            _tilesToChooseFrom = tilesToChooseFrom;
        }

        GameState IGameAction.doAction(GameState state, EventHandler eventHandler)
        {
            for (int col = 0; col < state.width; ++col) 
            {
                for (int row = 0; row < state.height; ++row)
                {
                    state[col, row] = randomTile();
                }
            }

            return state;
        }

        public string randomTile()
        {
            int randomIndex = _random.Next(0, this._tilesToChooseFrom.Length);
            return this._tilesToChooseFrom[randomIndex];
        }
    }
}
