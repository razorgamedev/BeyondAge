using BeyondAge.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondAge.Managers
{
    class DialogViewer
    {
        public bool Showing { get; private set; } = false;
        private LuaTable currentDialog = null;

        private int currentIndex = 0;
        private int charIndex = 0;
        private float charTimer;
        private float maxNextCharTime = 0.05f;

        private Point selector = Point.Zero;

        public void ShowDialog(LuaTable dialog, int startingIndex = 1)
        {
            currentDialog = dialog;
            currentIndex = startingIndex;
        }

        public void CloseDialog()
        {
            charIndex = 0;
            currentDialog = null;
            BeyondAge.TheGame.GameStatus = GameManager.Status.RUNNING;
        }

        public void Update(GameTime time)
        {
            if (currentDialog == null) return;
            var currentNode = currentDialog[currentIndex] as LuaTable;
            string text = currentNode[1] as string;

            if (charIndex < text.Length)
            {
                charTimer += (float)time.ElapsedGameTime.TotalSeconds;
                if (charTimer > maxNextCharTime)
                {
                    charTimer = 0;
                    charIndex++;
                }
            }

            if (GameInput.Self.KeyPressed(Keys.Escape)) { CloseDialog(); return; }
            if (GameInput.Self.KeyPressed(Keys.Enter) && currentNode["options"] == null)
            {
                if (currentIndex < currentDialog.Values.Count - 1)
                {
                    if (charIndex == text.Length)
                    {
                        currentIndex++;
                        charIndex = 0;
                    } else
                    {
                        charIndex = text.Length - 1;
                    }
                }
                else
                {
                    CloseDialog();
                    return;
                }
            }

            // Pointer control
            //if (currentIndex < currentDialog.Values.Count - 1)
            {
                if (currentNode["options"] != null)
                {
                    var options = currentNode["options"] as LuaTable;

                    // Goto the next dialog tree according to what was selected.
                    if (GameInput.Self.KeyPressed(Keys.Enter))
                    {
                        var currentChoice = options[selector.X + 1] as LuaTable;
                        var nextIndex = (int)(currentChoice[2] as Double?);
                        if (charIndex == text.Length)
                        {

                            if (nextIndex == -1)
                            {
                                CloseDialog();
                                GameInput.Self.PopKey(Keys.Enter); // Avoids re-entering the dialog
                                return;
                            }
                            currentIndex = nextIndex;
                            charIndex = 0;              // Reset the charIndex to zero
                        } else {
                            charIndex = text.Length - 1;
                        }
                    }

                    if (GameInput.Self.KeyPressed(Keys.Left))
                        selector.X--;

                    if (GameInput.Self.KeyPressed(Keys.Right))
                        selector.X++;
                    
                    if (selector.X < 0) selector.X = options.Values.Count - 1;
                    if (selector.X > options.Values.Count - 1) selector.X = 0;
                }
            }
        }

        public void UiDraw(SpriteBatch batch, Primitives primitives)
        {
            Showing = currentDialog != null;
            if (currentDialog != null)
            {
                primitives.DrawRect(new Rectangle(0, BeyondAge.Height - 256, BeyondAge.Width, 256), Color.SlateGray);
                var font = BeyondAge.Assets.GetFont("Font");

                var currentNode = currentDialog[currentIndex] as LuaTable;
                string text = (currentNode[1] as string);
                if (charIndex < text.Length)
                    text = text.Substring(0, charIndex);

                batch.DrawString(
                    font,
                    text,
                    new Vector2(32, BeyondAge.Height - 256 + 3),
                    Color.White
                    );

                // Show options
                if (currentNode["options"] != null)
                {
                    var options = currentNode["options"] as LuaTable;
                    
                    var index = 0;
                    var xpos = 32f;
                    foreach (LuaTable option in options.Values)
                    {
                        var otext = option[1] as string;
                        var nextIndex = (int)(option[2] as Double?);

                        batch.DrawString(
                            font,
                            otext,
                            new Vector2(xpos, BeyondAge.Height - 128 + 3),
                            Color.White
                            );

                        if (selector.X == index)
                        {
                            primitives.DrawLineRect(
                                new Rectangle(
                                    (int)(xpos - 16), (int)(BeyondAge.Height - 128 + 3 - 16),
                                    (int)(font.MeasureString(otext).X + 32), (int)(font.MeasureString(otext).Y + 32)
                                ), Color.White);
                        }

                        xpos += font.MeasureString(otext).X + 32;
                        index++;
                    }
                }
            }
        }
    }
}
