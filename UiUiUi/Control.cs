using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace UiUiUi
{
    public class SourceSprite
    {
        public string Asset { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }

        [XmlIgnore]
        public Texture2D Sprite { get; private set; }
        public void LoadSprite(ContentManager content)
        {
            Sprite = content.Load<Texture2D>(Asset);
        }
    }

    public class Control
    {
        public Control()
        {
            Controls = new List<Control>();
        }

        // Left coordinate of the control relative to its parent
        public int Left { get; set; }
        // Top coordinate of the control relative to its parent
        public int Top { get; set; }
        // Width of control
        public int Width { get; set; }
        // Height of control
        public int Height { get; set; }
        // Left border before children/text inner control
        public int LeftBorder {get; set; }
        // Right border before children/text inner control
        public int RightBorder {get; set;}
        // Top border before children/text inner control
        public int TopBorder{get; set; }
        // Top border before children/text inner control
        public int BottomBorder {get; set;}
        // Default sprite
        public SourceSprite NormalSprite { get; set; }
        // Sprite when mouse is down
        public SourceSprite MouseDownSprite { get; set; }
        // Sprite when mouse is over
        public SourceSprite MouseOverSprite { get; set; }
        // Sprite when control is active (for instance, text editable)
        public SourceSprite ActiveSprite { get; set; }
        // Child controls of the control
        public List<Control> Controls { get; set; }
        // Name of this contraol
        public string Name { get; set; }
        // Text displayed on the control
        private string _text;
        public string Text { get {return _text;}
            set
            {
                _text = value;
                if (Cursor >= value.Length)
                {
                    Cursor = value.Length;
                }
            }
        }
        // Font text is displayed in
        protected SpriteFont SpriteFont;
        // Name of SpriteFont SpriteFont is loaded from
        public string Font { get; set; }
        [XmlIgnore]
        // Object that any data or events call
        protected object codeBehindObject;
        // Values for which sprite should be in use
        public enum SpriteStates { Normal, MouseOver, MouseDown, Active }
        // Which sprite should be in use
        public SpriteStates SpriteState = SpriteStates.Normal;
        // Values for Alignment of text on sprite
        public enum TextAlignments { Left, Right, Center }
        // Alignment of text on sprite
        public TextAlignments TextAlignment { get; set; }
        // Values for vertical alignment of text on sprite
        public enum TextVAlignments { Top, Bottom, Center}
        // Vertical text alignment
        public TextVAlignments TextVAlignment { get; set; } = TextVAlignments.Center;
        // Color of text on control
        public Color TextColor { get; set; }
        // I.E. Is it a text box?
        public bool TextEditable {get; set;}
        // Character to use as cursor
        public string TextCursor {get; set;} = "|";
        // Current position of cursor
        private int Cursor;
        // Last state that the mouse was in. Used for verifying new clicks.
        [XmlIgnore]
        public MouseState LastMouseState;
        // Surface to render controls/text to
        private RenderTarget2D controlsRenderTarget = null;
        // How far the controls are 'scrolled' in the X dimension
        public int ControlOffsetX;
        // How far the controls are 'scrolled' in the Y dimension
        public int ControlOffsetY;

        protected RenderTarget2D GetRenderTarget(GraphicsDevice device)
        {
            var desiredWidth = Width - LeftBorder - RightBorder;
            var desiredHeight = Height - TopBorder - BottomBorder;
            if (!(Width == 0 || Height == 0) && (
                 controlsRenderTarget == null ||
                 (controlsRenderTarget.Width != desiredWidth && controlsRenderTarget.Height != desiredHeight) ||
                 controlsRenderTarget.GraphicsDevice != device)
            )
            {
                controlsRenderTarget = new RenderTarget2D(device, desiredWidth, desiredHeight, false, device.PresentationParameters.BackBufferFormat, DepthFormat.Depth24, 4, RenderTargetUsage.PreserveContents);
            }
            return controlsRenderTarget;
        }

        public virtual void Draw(GraphicsDevice device, RenderTarget2D parentRenderTarget = null,
                                 int controlOffsetX = 0, int controlOffsetY = 0)
        // Draw the control
        // parentX and parentY are used as to be able to provide the relative
        // location to draw the control in
        {
            if (!Visible)
            {
                return;
            }

            var left = Left + controlOffsetX;
            var top = Top + controlOffsetY;

            var spriteBatch = new SpriteBatch(device);
            spriteBatch.Begin();
            // Render this control
            SourceSprite sprite = NormalSprite ?? null;
            sprite = SpriteState == SpriteStates.MouseOver ? MouseOverSprite ?? sprite : sprite;
            sprite = SpriteState == SpriteStates.Active ? ActiveSprite ?? sprite : sprite;
            sprite = SpriteState == SpriteStates.MouseDown ? MouseDownSprite ?? sprite : sprite;

            if (sprite != null)
            {
                spriteBatch.Draw(sprite.Sprite,
                                 new Rectangle(left, top , Width, Height),
                                 new Rectangle(sprite.Left, sprite.Top, Width, Height),
                                 Color.White);

            }


            spriteBatch.End();

            if (Controls.Count > 0 || (Text != null && SpriteFont != null))
            {
                var controlsRenderTarget = GetRenderTarget(device);
                device.SetRenderTarget(controlsRenderTarget);
                device.Clear(Color.Transparent);

                var textSpriteBatch = new SpriteBatch(device);
                textSpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                // Render text
                // TODO: This should probably be cached
                if (SpriteFont != null && Text != null)
                {
                    int x = 0;
                    int y = 0;
                    var textToRender = Text;
                    if (Activated)
                    {
                        textToRender = Text.Insert(Cursor, TextCursor);
                    }
                    var measurement = SpriteFont.MeasureString(textToRender);
                    var textWidth = measurement.X;
                    var textHeight = measurement.Y;
                    switch (TextAlignment)
                    {
                        case TextAlignments.Center:
                            x = (Width - (int)textWidth) / 2;
                            break;
                        case TextAlignments.Left:
                            x = 0;
                            break;
                        case TextAlignments.Right:
                            x = Width - (int)textWidth;
                            break;
                    }
                    switch (TextVAlignment)
                    {
                        case TextVAlignments.Center:
                            y = (Height - (int)textHeight) / 2;
                            break;
                        case TextVAlignments.Top:
                            y = 0;
                            break;
                        case TextVAlignments.Bottom:
                            y = Height - (int)textHeight;
                            break;
                    }
                    textSpriteBatch.DrawString(SpriteFont, textToRender, new Vector2(x + ControlOffsetX, y + ControlOffsetY), TextColor);
                }
                textSpriteBatch.End();

                // Draw child controls
                foreach (var control in Controls)
                {
                    control.Draw(device, controlsRenderTarget, ControlOffsetX, ControlOffsetY);
                }

                device.SetRenderTarget(parentRenderTarget);
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                spriteBatch.Draw(controlsRenderTarget, new Vector2(left + LeftBorder, top + TopBorder), Color.White);
                spriteBatch.End();
            }
        }

        public virtual void LoadSprites(ContentManager content)
        {
            NormalSprite?.LoadSprite(content);
            MouseDownSprite?.LoadSprite(content);
            MouseOverSprite?.LoadSprite(content);
            ActiveSprite?.LoadSprite(content);
            if (!String.IsNullOrEmpty(Font))
            {
                SpriteFont = content.Load<SpriteFont>(Font);
            }
            foreach (var control in Controls)
            {
                control.LoadSprites(content);
            }
        }

        public virtual void AssignEvents(object codeBehindObject)
        // This assigns events to their handlers in the code behind object
        // I don't want to reflect each event - so doing it like this once
        // and caching it
        {
            this.codeBehindObject = codeBehindObject;
            foreach (var control in Controls)
            {
                control.AssignEvents(codeBehindObject);
            }

            if (codeBehindObject == null)
            {
                return;
            }

            if (OnClick != null)
                OnClickEvent = (OnClickDelegate)Delegate.CreateDelegate(typeof(OnClickDelegate), codeBehindObject, OnClick);
            if (EnterKey != null)
                EnterKeyEvent = (EnterKeyDelegate)Delegate.CreateDelegate(typeof(EnterKeyDelegate), codeBehindObject, EnterKey);
        }

        public delegate void OnClickDelegate(Control caller, int relativeX, int relativeY, MouseButton button);
        [XmlIgnore]
        OnClickDelegate OnClickEvent;
        public String OnClick { get; set; }

        public delegate void EnterKeyDelegate(Control caller);
        [XmlIgnore]
        EnterKeyDelegate EnterKeyEvent;
        public String EnterKey { get; set; }

        public bool Visible { get; set; } = true;

        // Used for keeping track of multiple controls of same name
        public int Index {get; set;}

        [XmlIgnore]
        public bool Activated { get; private set;} = false;

        protected bool MouseInControl(MouseState translatedState)
        {
            return translatedState.X >= 0 &&
                   translatedState.Y >= 0 &&
                   translatedState.X <= Width &&
                   translatedState.Y <= Height;
        }

        public void SetCursorFromMouse(int x, bool activated)
        {
            var textToRender = Text;
            if (Activated)
            {
                textToRender = Text.Insert(Cursor, TextCursor);
            }
            int idx = -1;
            int width = 0;
            int lastWidth = 0;
            foreach(var letter in textToRender)
            {
                ++idx;
                width = (int)SpriteFont.MeasureString(textToRender.Substring(0, idx + 1)).X;
                int addedWidth = width - lastWidth;

                if (Activated && idx == Cursor)
                {
                    continue;
                }
                // This is flawed. Maybe fix in the future.
                if ((width - addedWidth / 2) > x)
                {
                    if (Activated && idx > Cursor)
                    {
                        Cursor = idx - 2;
                    }
                    else
                    {
                        Cursor = idx - 1;
                    }
                    Cursor = Math.Max(Cursor, 0);
                    Cursor = Math.Min(Cursor, Text.Length);
                    return;
                }
                if (width > x)
                {
                    if (Activated && idx > Cursor)
                    {
                        Cursor = idx - 1;
                    }
                    else
                    {
                        Cursor = idx;
                    }
                    Cursor = Math.Max(Cursor, 0);
                    Cursor = Math.Min(Cursor, Text.Length);
                    return;
                }
            }
            Cursor = Text.Length;
        }

        // Expand these as I add support for them
        public enum MouseButton { Left };

        public virtual bool ProcessMouse(MouseState state)
        {
            if (!Visible)
            {
                return true;
            }

            //Mouse state relative to the control
            var translatedMouseState = new MouseState(
                state.X - Left, state.Y - Top,
                state.ScrollWheelValue, state.LeftButton, state.MiddleButton,
                state.RightButton, state.XButton1, state.XButton2);

            //Mouse state relative to the controls child controls (which
            //includes) text
            var embeddedTranslatedMouseState = new MouseState(
                state.X - Left - LeftBorder - ControlOffsetX, state.Y - Top - TopBorder - ControlOffsetY,
                state.ScrollWheelValue, state.LeftButton, state.MiddleButton,
                state.RightButton, state.XButton1, state.XButton2);

            if (MouseInControl(translatedMouseState))
            {
                foreach (var control in Controls)
                {
                    if (!control.ProcessMouse(embeddedTranslatedMouseState))
                    {
                        LastMouseState = state;
                        return false;
                    }
                }

                if (state.LeftButton == ButtonState.Pressed)
                {
                    SpriteState = SpriteStates.MouseDown;
                }
                else
                {
                    if (LastMouseState.LeftButton == ButtonState.Pressed)
                    {
                        if (OnClickEvent != null)
                        {
                            OnClickEvent(this, translatedMouseState.X, translatedMouseState.Y, MouseButton.Left);
                            LastMouseState = state;
                            return false;
                        }
                        if (TextEditable)
                        {
                            SetCursorFromMouse(translatedMouseState.X, Activated);
                            Activate();
                            LastMouseState = state;
                            return false;
                        }
                    }
                    if (!(TextEditable && Activated))
                    {
                        SpriteState = SpriteStates.MouseOver;
                    }
                }
            }
            else
            {
                foreach (var control in Controls)
                {
                    if (control.SpriteState == SpriteStates.MouseDown || control.SpriteState == SpriteStates.MouseOver)
                    {
                        control.SpriteState = SpriteStates.Normal;
                    }
                }

                if (state.LeftButton == ButtonState.Pressed && TextEditable)
                {
                    Deactivate();
                }
                else if (!TextEditable)
                {
                    SpriteState = SpriteStates.Normal;
                }
            }

            LastMouseState = state;

            return true;
        }

        public virtual void Deactivate()
        {

            Activated = false;
            SpriteState = SpriteStates.Normal;
        }

        public virtual void Activate()
        {
            Activated = true;
            SpriteState = SpriteStates.Active;
        }
        [XmlIgnore]
        public KeyboardState lastKeyboardState;

        /// <summary>
        /// Process keyboard events
        /// </summary>
        /// <param name="state">Keyboard state</param>
        /// <returns>True if the caller should keep processing keyboard events</returns>
        public virtual bool ProcessKeyboard(KeyboardState state)
        {
            if (Visible && TextEditable && Activated)
            {
                    foreach(var key in state.GetPressedKeys())
                    {
                        if (lastKeyboardState.IsKeyUp(key))
                        {
                            bool shift = state.CapsLock ^ (state.IsKeyDown(Keys.LeftShift) || state.IsKeyDown(Keys.RightShift));
                            char? character = null;
                            switch (key)
                            {
                                case Keys.Back:
                                    if (Cursor > 0)
                                    {
                                        Cursor -= 1;
                                        Text = Text.Remove(Cursor, 1);
                                    }
                                    break;
                                case Keys.Delete:
                                    if (Cursor == Text.Length)
                                    {
                                        break;
                                    }
                                    Text = Text.Remove(Cursor, 1);
                                    break;
                                case Keys.Left:
                                    Cursor -= 1;
                                    Cursor = Math.Max(Cursor, 0);
                                    break;
                                case Keys.Right:
                                    Cursor += 1;
                                    Cursor = Math.Min(Cursor, Text.Length);
                                    break;
                                case Keys.Home:
                                    Cursor = 0;
                                    break;
                                case Keys.End:
                                    Cursor = Text.Length;
                                    break;
                                case Keys.Enter:
                                    if (EnterKeyEvent != null)
                                    {
                                        EnterKeyEvent(this);
                                    }
                                    break;
                                case Keys.A: if (shift) { character = 'A'; } else { character = 'a'; } break;
                                case Keys.B: if (shift) { character = 'B'; } else { character = 'b'; } break;
                                case Keys.C: if (shift) { character = 'C'; } else { character = 'c'; } break;
                                case Keys.D: if (shift) { character = 'D'; } else { character = 'd'; } break;
                                case Keys.E: if (shift) { character = 'E'; } else { character = 'e'; } break;
                                case Keys.F: if (shift) { character = 'F'; } else { character = 'f'; } break;
                                case Keys.G: if (shift) { character = 'G'; } else { character = 'g'; } break;
                                case Keys.H: if (shift) { character = 'H'; } else { character = 'h'; } break;
                                case Keys.I: if (shift) { character = 'I'; } else { character = 'i'; } break;
                                case Keys.J: if (shift) { character = 'J'; } else { character = 'j'; } break;
                                case Keys.K: if (shift) { character = 'K'; } else { character = 'k'; } break;
                                case Keys.L: if (shift) { character = 'L'; } else { character = 'l'; } break;
                                case Keys.M: if (shift) { character = 'M'; } else { character = 'm'; } break;
                                case Keys.N: if (shift) { character = 'N'; } else { character = 'n'; } break;
                                case Keys.O: if (shift) { character = 'O'; } else { character = 'o'; } break;
                                case Keys.P: if (shift) { character = 'P'; } else { character = 'p'; } break;
                                case Keys.Q: if (shift) { character = 'Q'; } else { character = 'q'; } break;
                                case Keys.R: if (shift) { character = 'R'; } else { character = 'r'; } break;
                                case Keys.S: if (shift) { character = 'S'; } else { character = 's'; } break;
                                case Keys.T: if (shift) { character = 'T'; } else { character = 't'; } break;
                                case Keys.U: if (shift) { character = 'U'; } else { character = 'u'; } break;
                                case Keys.V: if (shift) { character = 'V'; } else { character = 'v'; } break;
                                case Keys.W: if (shift) { character = 'W'; } else { character = 'w'; } break;
                                case Keys.X: if (shift) { character = 'X'; } else { character = 'x'; } break;
                                case Keys.Y: if (shift) { character = 'Y'; } else { character = 'y'; } break;
                                case Keys.Z: if (shift) { character = 'Z'; } else { character = 'z'; } break;
                                //Decimal characters
                                case Keys.D0: if (shift) { character = ')'; } else { character = '0'; } break;
                                case Keys.D1: if (shift) { character = '!'; } else { character = '1'; } break;
                                case Keys.D2: if (shift) { character = '@'; } else { character = '2'; } break;
                                case Keys.D3: if (shift) { character = '#'; } else { character = '3'; } break;
                                case Keys.D4: if (shift) { character = '$'; } else { character = '4'; } break;
                                case Keys.D5: if (shift) { character = '%'; } else { character = '5'; } break;
                                case Keys.D6: if (shift) { character = '^'; } else { character = '6'; } break;
                                case Keys.D7: if (shift) { character = '&'; } else { character = '7'; } break;
                                case Keys.D8: if (shift) { character = '*'; } else { character = '8'; } break;
                                case Keys.D9: if (shift) { character = '('; } else { character = '9'; } break;
                                //Decimal numpad characters
                                case Keys.NumPad0: character = '0'; break;
                                case Keys.NumPad1: character = '1'; break;
                                case Keys.NumPad2: character = '2'; break;
                                case Keys.NumPad3: character = '3'; break;
                                case Keys.NumPad4: character = '4'; break;
                                case Keys.NumPad5: character = '5'; break;
                                case Keys.NumPad6: character = '6'; break;
                                case Keys.NumPad7: character = '7'; break;
                                case Keys.NumPad8: character = '8'; break;
                                case Keys.NumPad9: character = '9'; break;
                                //Special characters
                                case Keys.OemTilde: if (shift) { character = '~'; } else { character = '`'; } break;
                                case Keys.OemSemicolon: if (shift) { character = ':'; } else { character = ';'; } break;
                                case Keys.OemQuotes: if (shift) { character = '"'; } else { character = '\''; } break;
                                case Keys.OemQuestion: if (shift) { character = '?'; } else { character = '/'; } break;
                                case Keys.OemPlus: if (shift) { character = '+'; } else { character = '='; } break;
                                case Keys.OemPipe: if (shift) { character = '|'; } else { character = '\\'; } break;
                                case Keys.OemPeriod: if (shift) { character = '>'; } else { character = '.'; } break;
                                case Keys.OemOpenBrackets: if (shift) { character = '{'; } else { character = '['; } break;
                                case Keys.OemCloseBrackets: if (shift) { character = '}'; } else { character = ']'; } break;
                                case Keys.OemMinus: if (shift) { character = '_'; } else { character = '-'; } break;
                                case Keys.OemComma: if (shift) { character = '<'; } else { character = ','; } break;
                                case Keys.Space: character = ' '; break;
                            }
                            if (character != null)
                            {
                                Text = Text.Insert(Cursor, character.ToString());
                                Cursor += 1;
                            }
                        }
                    }
                lastKeyboardState = state;
                return false;
            }

            foreach (var control in Controls)
            {
                if (!control.ProcessKeyboard(state))
                {
                    return false;
                }
            }
            lastKeyboardState = state;

            return true;
        }

        public Control Find(string name)
        {
            foreach (var control in Controls)
            {
                if (control.Name == name)
                {
                    return control;
                }
                var searchedForControl = control.Find(name);
                if (searchedForControl != null)
                {
                    return searchedForControl;
                }
            }
            return null;
        }
    }
}
