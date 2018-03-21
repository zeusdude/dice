using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace DiceGame
{
    public class GameController
    {
        private string[] _testTiles = new string[] {"3lh"};

        private string[] _pickableTiles = new string[] {"1", "2hl", "2lh", "3hl", "3lh", "4", "5", "6h", "6v"};

        // Could make this harder or easier by adding more non-pickables and decreasing the tiles that destroy themselves
        // on pick
        private string[] _refillTiles = new string[] {"b", "x2", "x3", "2hl", "2lh", "4", "6h", "6v"};

        GameState _gameState;

        public GameController(GameState initialState)
        {
            _gameState = initialState;
        }

        public void Execute(IGameAction action)
        {
            state = action.doAction(_gameState, ActionEventHandler);
        }

        public GameState state 
        {
            get { return _gameState; }
            private set { _gameState = value; }
        }

        public string[] pickableTiles
        {
            get { return _pickableTiles; }
        }

        public string[] testTiles
        {
            get { return _testTiles; }
        }

        public bool isTilePickable(int boardCol, int boardRow)
        {
            string boardTile = _gameState[boardCol, boardRow];
            return _pickableTiles.Contains(boardTile);
        }

        //
        // Returns arrayList of GameState.TileLocation
        public List<GameState.TileLocation> pickableTileLocations()
        {
            List<GameState.TileLocation> pickableTileLocations = new List<GameState.TileLocation>();

            for (int col = 0; col < state.width; ++col) 
            {
                for (int row = 0; row < state.height; ++row)
                {
                    if (isTilePickable(col, row))
                    {
                        pickableTileLocations.Add(new GameState.TileLocation(col, row));    
                    }
                }
            }

            return pickableTileLocations;
        }

        public string[] refillTiles
        {
            get { return _refillTiles; }
        }


        public event EventHandler ActionEventHandler;
    }
}