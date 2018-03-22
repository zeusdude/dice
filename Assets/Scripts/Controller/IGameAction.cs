using System;
using System.Text;

namespace DiceGame
{
    public interface IGameAction
    {
        void Execute(GameState state, EventHandler eventHandler);
    }
}
