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

        void IGameAction.Execute(GameState state, EventHandler eventHandler)
        {
        }
    }
}
