using BeyondAge.Graphics;
using NLua;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace BeyondAge.Utilities
{
    class DevelopmentConsole : DrawableGameComponent
    {
        SpriteBatch batch;
        Primitives primitives;
        Lua lua;

        List<Point> cursors = new List<Point>() { Point.Zero };
        public Point Cursor { get => cursors[0]; set => cursors[0] = value; }

        enum State
        {
            CLOSED,
            HALF,
            FULL
        }

        private int state = (int)State.CLOSED;
        private string commandText = "";

        public DevelopmentConsole(Game game, SpriteBatch batch, Primitives primitives, Lua lua, GameWindow window) : base(game)
        {
            this.lua = lua;
            this.batch = batch;
            this.primitives = primitives;
            this.Enabled = true;

            window.TextInput += TextInput;
        }

        private void TextInput(object sender, TextInputEventArgs e)
        {
            if (this.state == 0) return;
            
            if (GameInput.Self.KeyDown(Keys.Tab))
            {
                commandText += "  ";
            } else if (GameInput.Self.KeyDown(Keys.Enter)) {
                if (GameInput.Self.KeyDown(Keys.LeftShift))
                {
                    commandText += "\n";
                } else
                {
                    try
                    {
                        lua.DoString(commandText);
                        commandText = "";
                        Cursor = Point.Zero;
                    } catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            } else if (GameInput.Self.KeyDown(Keys.Back))
            {
                if (commandText.Length > 0)
                    commandText = commandText.Remove(Cursor.X - 1, 1);
                
                Cursor -= new Point(1, 0);

            } else if (GameInput.Self.KeyDown(Keys.Escape)) {
            } else
            {
             
                if (commandText.Length > 0 && Cursor.X != commandText.Length)
                    commandText = commandText.Insert(Cursor.X, e.Character.ToString());
                else
                    commandText += e.Character;

                Cursor += new Point(1, 0);
            }

        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (GameInput.Self.KeyPressed(Keys.Home)){
                this.state++;
                this.state %= 3;

                if (this.state == 0)
                    BeyondAge.TheGame.GameStatus = GameManager.Status.RUNNING;
            }

            if (this.state != 0)
            {
                BeyondAge.TheGame.GameStatus = GameManager.Status.PAUSED;
            }

            if (GameInput.Self.KeyPressed(Keys.Left))
            {
                if (Cursor.X > 0) Cursor -= new Point(1, 0);
            }

            if (GameInput.Self.KeyPressed(Keys.Right))
            {
                if (Cursor.X < commandText.Length)
                    Cursor += new Point(1, 0);
            }

            if (GameInput.Self.KeyPressed(Keys.Delete))
            {
                if (commandText.Length > 0 && Cursor.X < commandText.Length)
                    commandText = commandText.Remove(Cursor.X, 1);
            }
            
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            float height = BeyondAge.Height / 2f;
            if (this.state == (int)State.CLOSED) height *= 0;
            if (this.state == (int)State.HALF) height *= 0.2f;
            
            batch.Begin();
            primitives.DrawRect(new Rectangle(
                0, 0, BeyondAge.Width, (int)height
                ), new Color(0f, 0f, 0f, 0.8f));

            var font = BeyondAge.Assets.GetFont("Font");
            
            if (Cursor.X > 0 && commandText.Length > 0)
            {
                var txt = commandText.Substring(0, Cursor.X);
                primitives.DrawRect(new Rectangle(8 + (int)(font.MeasureString(txt).X), (int)(height - font.MeasureString("|").Y), 4, (int)(font.MeasureString("|").Y)), Color.White);
            }
            else
            {
                primitives.DrawRect(new Rectangle(8, (int)(height - font.MeasureString("|").Y), 4, (int)(font.MeasureString("|").Y)), Color.White );
            }
            
            batch.DrawString(font, commandText, new Vector2(8, height - font.MeasureString(commandText).Y), Color.White);

            batch.End();
        }

    }
}
