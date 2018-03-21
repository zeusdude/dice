using System;
using System.Text;

namespace DiceGame
{
    public interface IGameAction
    {
        GameState doAction(GameState state, EventHandler eventHandler);
    }
}
