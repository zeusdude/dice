using System;
using System.Text;

namespace DiceGame
{
    //
    // NoOpAction is not intended to be created.  I am just using it as a template
    // for building additional actions.
    public class NoOpAction : IGameAction
    {
        public NoOpAction()
        {
        }

        GameState IGameAction.doAction(GameState state, EventHandler eventHandler)
        {
            return state;
        }
    }
}
