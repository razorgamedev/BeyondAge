using BeyondAge.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeyondAge.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace BeyondAge.Utilities
{
    class GameState: GameEventHandler
    {
        public GameStateManager gsm { get; set; }
    }

    class GameStateManager: GameEventHandler
    {
        private List<GameState> states;

        public GameStateManager()
        {
            states = new List<GameState>();
        }

        public GameState PushState(GameState state, bool load = true)
        {
            //state
            this.states.Add(state);
            state.gsm = this;
            if (load) state.Load();

            return state;
        }

        public GameState PopState()
        {
            var l = states.Last();
            states.Remove(l);
            return l;
        }

        public void Goto(GameState state)
        {
            if (states.Count > 0)
            {
                var s = PopState();
                s.Destroy();
            }

            PushState(state, true);
        }

        public override void Update(GameTime time)
        {
            for (int i = states.Count - 1; i >= 0; i--)
                states[i].Update(time);
        }

        public override void NonPausableUpdate(GameTime time)
        {
            states.ForEach(s => s.NonPausableUpdate(time));
        }

        public override void Draw(SpriteBatch batch, Primitives primitives)
        {
            states.ForEach(s => s.Draw(batch, primitives));
        }

        public override void DrawGui(SpriteBatch batch, Primitives primitives)
        {
            states.ForEach(s => s.DrawGui(batch, primitives));
        }
    }
}
