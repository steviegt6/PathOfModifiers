using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Terraria;
using Terraria.UI;

namespace PathOfModifiers.UI.Elements
{
    public class UIFocusInputTextField : UIElement
    {
        internal bool Focused = false;
        internal string CurrentString = "";

        private readonly string _hintText;
        private int _textBlinkerCount;
        private int _textBlinkerState;
        public bool UnfocusOnTab { get; internal set; } = false;

        public delegate void EventHandler(object sender, EventArgs e);

        public event EventHandler OnTextChange;
        public event EventHandler OnUnfocus;
        public event EventHandler OnTab;

        public UIFocusInputTextField(string hintText)
        {
            _hintText = hintText;
        }

        public void SetText(string text)
        {
            if (text == null)
                text = "";

            if (CurrentString != text)
            {
                CurrentString = text;
                OnTextChange?.Invoke(this, new EventArgs());
            }
        }

        public override void Click(UIMouseEvent evt)
        {
            Main.clrInput();
            Focused = true;
        }

        public override void RightClick(UIMouseEvent evt)
        {
            base.RightClick(evt);

            SetText("");
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 MousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);
            if (!ContainsPoint(MousePosition) && Main.mouseLeft) // TODO: && focused maybe?
            {
                Focused = false;
                OnUnfocus?.Invoke(this, new EventArgs());
            }
            base.Update(gameTime);
        }
        private static bool JustPressed(Keys key)
        {
            return Main.inputText.IsKeyDown(key) && !Main.oldInputText.IsKeyDown(key);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            //Rectangle hitbox = GetOuterDimensions().ToRectangle();
            //Main.spriteBatch.Draw(Terraria.GameContent.TextureAssets.MagicPixel.Value, hitbox, Color.Red * 0.6f);

            if (Focused)
            {
                Terraria.GameInput.PlayerInput.WritingText = true;
                Main.instance.HandleIME();
                string newString = Main.GetInputText(CurrentString);
                if (!newString.Equals(CurrentString))
                {
                    CurrentString = newString;
                    OnTextChange?.Invoke(this, new EventArgs());
                }
                else
                {
                    CurrentString = newString;
                }
                if (JustPressed(Keys.Tab))
                {
                    if (UnfocusOnTab)
                    {
                        Focused = false;
                        OnUnfocus?.Invoke(this, new EventArgs());
                    }
                    OnTab?.Invoke(this, new EventArgs());
                }
                if (++_textBlinkerCount >= 20)
                {
                    _textBlinkerState = (_textBlinkerState + 1) % 2;
                    _textBlinkerCount = 0;
                }
            }
            string displayString = CurrentString;
            if (_textBlinkerState == 1 && Focused)
            {
                displayString += "|";
            }
            CalculatedStyle space = GetDimensions();
            if (CurrentString.Length == 0 && !Focused)
            {
                Utils.DrawBorderString(spriteBatch, _hintText, new Vector2(space.X, space.Y), Color.Gray);
            }
            else
            {
                Utils.DrawBorderString(spriteBatch, displayString, new Vector2(space.X, space.Y), Color.White);
            }
        }
    }
}