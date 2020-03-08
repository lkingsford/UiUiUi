using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace UiUiUi
{
    /// <summary>
    /// A reference to part of a monogame sprite that is used to draw to the screen.
    /// </summary>
    public class SourceSprite
    {
        /// <summary>
        /// Name of the asset to load.
        /// </summary>
        /// <value></value>
        public string Asset { get; set; }
        /// <summary>
        /// The source left coordinate of the sprite to display.
        /// </summary>
        /// <value></value>
        public int Left { get; set; }
        /// <summary>
        /// The source top coordinate of the sprite to display.
        /// </summary>
        /// <value></value>
        public int Top { get; set; }

        /// <summary>
        /// The sprite itself to render, once loaded.
        /// </summary>
        /// <value></value>
        [XmlIgnore]
        public Texture2D Sprite { get; private set; }
        /// <summary>
        /// Load the Sprite with the name in the Asset from the content manager provided.
        /// </summary>
        /// <param name="content">Content manager to load the sprite off.</param>
        public void LoadSprite(ContentManager content)
        {
            Sprite = content.Load<Texture2D>(Asset);
        }
    }

    /// <summary>
    /// A control to display on the interface.
    /// </summary>
        public class Control
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Control()
        {
            Controls = new List<Control>();
        }

        /// <summary>
        /// Left coordinate of the control relative to its parent
        /// </summary>
        /// <value></value>
        public int Left { get; set; }
        /// <summary>
        /// Top coordinate of the control relative to its parent
        /// </summary>
        /// <value></value>
        public int Top { get; set; }
        /// <summary>
        /// Width of control
        /// </summary>
        /// <value></value>
        public int Width { get; set; }
        /// <summary>
        /// Height of control
        /// </summary>
        /// <value></value>
        public int Height { get; set; }
        /// <summary>
        /// Left border before children/text inner control
        /// </summary>
        /// <value></value>
        public int LeftBorder {get; set; }
        /// <summary>
        /// Right border before children/text inner control
        /// </summary>
        /// <value></value>
        public int RightBorder {get; set;}
        /// <summary>
        /// Top border before children/text inner control
        /// </summary>
        /// <value></value>
        public int TopBorder{get; set; }
        /// <summary>
        /// Top border before children/text inner control
        /// </summary>
        /// <value></value>
        public int BottomBorder {get; set;}
        /// <summary>
        /// Default sprite
        /// </summary>
        /// <value></value>
        public SourceSprite NormalSprite { get; set; }
        /// <summary>
        /// Sprite when mouse is down
        /// </summary>
        /// <value></value>
        public SourceSprite MouseDownSprite { get; set; }
        /// <summary>
        /// Sprite when mouse is over
        /// </summary>
        /// <value></value>
        public SourceSprite MouseOverSprite { get; set; }
        /// <summary>
        /// Sprite when control is active (for instance, text editable)
        /// </summary>
        /// <value></value>
        public SourceSprite ActiveSprite { get; set; }
        /// <summary>
        /// Child controls of the control
        /// </summary>
        /// <value></value>
        public List<Control> Controls { get; set; }
        /// <summary>
        /// Name of this control
        /// </summary>
        /// <value></value>
        public string Name { get; set; }
        /// <summary>
        /// Text that is actually displayed on the control after processing (so, formatting, carets etc)
        /// </summary>
        /// <value></value>
        private string _text;
        /// <summary>
        /// Text that is displayed on the control
        /// </summary>
        /// <value></value>
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
        /// <summary>
        /// Font text is displayed in
        /// </summary>
        /// <value></value>
        protected SpriteFont SpriteFont;

        /// <summary>
        /// Name of SpriteFont SpriteFont is loaded from
        /// </summary>
        /// <value></value>
        public string Font { get; set; }

        /// <summary>
        /// Object that any data or events call
        /// </summary>
        /// <value></value>
        [XmlIgnore]
        protected object codeBehindObject;

        /// <summary>
        /// Values for which sprite should be in use
        /// </summary>
        public enum SpriteStates { Normal, MouseOver, MouseDown, Active }

        /// <summary>
        /// Which sprite should be in use
        /// </summary>
        /// <value></value>
        protected SpriteStates SpriteState = SpriteStates.Normal;

        /// <summary>
        /// Values for Alignment of text on sprite
        /// </summary>
        /// <value></value>
        public enum TextAlignments { Left, Right, Center }

        /// <summary>
        /// Alignment of text on sprite
        /// </summary>
        /// <value></value>
        public TextAlignments TextAlignment { get; set; }

        /// <summary>
        /// Values for vertical alignment of text on sprite
        /// </summary>
        public enum TextVAlignments { Top, Bottom, Center}

        /// <summary>
        /// Vertical text alignment
        /// </summary>
        /// <value></value>
        public TextVAlignments TextVAlignment { get; set; } = TextVAlignments.Center;

        /// <summary>
        /// Color of text on control
        /// </summary>
        /// <value></value>
        public Color TextColor { get; set; }

        /// <summary>
        /// I.E. Is it a text box?
        /// </summary>
        /// <value></value>
        public bool TextEditable {get; set;}

        /// <summary>
        /// Character to use as cursor
        /// </summary>
        /// <value></value>
        public string TextCursor {get; set;} = "|";

        /// <summary>
        /// Current position of cursor
        /// </summary>
        private int Cursor;

        /// <summary>
        /// Last state that the mouse was in. Used for verifying new clicks.
        /// </summary>
        /// <value></value>
        [XmlIgnore]
        protected MouseState LastMouseState;

        /// <summary>
        /// Surface to render controls/text to
        /// </summary>
        private RenderTarget2D controlsRenderTarget = null;

        /// <summary>
        /// How far the controls are 'scrolled' in the X dimension
        /// </summary>
        public int ControlOffsetX;

        /// <summary>
        /// How far the controls are 'scrolled' in the Y dimension
        /// </summary>
        public int ControlOffsetY;

        /// <summary>
        /// Get the RenderTarget to render onto this control. Will create a new target if none yet
        /// exists.
        /// </summary>
        /// <param name="device">Graphics device to create RenderTarget on</param>
        /// <returns>A RenderTarget to draw on this control.</returns>
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

        /// <summary>
        /// Draw this control, and anything rendered on this control, onto a given rendertarget.
        /// </summary>
        /// <param name="device">Graphics device that is being used</param>
        /// <param name="parentRenderTarget">
        /// The render target belonging to the parent control, that this control is rendered onto.
        /// </param>
        /// <param name="controlOffsetX">
        /// The vertical coordinate to offset the render by on the parent target in addition to the
        /// control's left and top.
        /// </param>
        /// <param name="controlOffsetY">
        /// The horizontal coordinate to offset the render by on the parent target in addition to
        /// the control's left and top.
        /// </param>
        public virtual void Draw(GraphicsDevice device, RenderTarget2D parentRenderTarget = null,
                                 int controlOffsetX = 0, int controlOffsetY = 0)
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

        /// <summary>
        /// Load the sprites for the control from the content manager. This is because sprites are
        /// usually defined by their string name in the layout XML.
        /// </summary>
        /// <param name="content">App content manager to read sprites from</param>
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

        /// <summary>
        /// Assign events to delegates based on the string name of the event as specified.
        /// </summary>
        /// <param name="codeBehindObject">
        /// Object that contains functions to use as delegates.
        /// </param>
        /// <remarks>
        /// This is because reflection can be an expensive operation, so this sets all of them at
        /// once rather than reflecting each time it is triggered. This may be the best approach,
        /// but also may change in the future.
        /// </remarks>
        public virtual void AssignEvents(object codeBehindObject)
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

        /// <summary>
        /// Type of function to be called on an OnClick event.
        /// </summary>
        /// <param name="caller">The control that called the event.</param>
        /// <param name="relativeX">Where this control was clicked relative to its location.</param>
        /// <param name="relativeY">Where this control was clicked relative to its location.</param>
        /// <param name="button">Button that was clicked</param>
        public delegate void OnClickDelegate(Control caller, int relativeX, int relativeY, MouseButton button);

        /// <summary>
        /// Function to call when a mouse button is clicked on the control. Clicking means mouse
        /// down, followed by mouse up, when both are in the borders of the control.
        /// </summary>
        [XmlIgnore]
        OnClickDelegate OnClickEvent;

        /// <summary>
        /// String name of the function in the CodeBehind object to call on an OnClick event.
        /// AssignEvents needs to be called before it will work.
        /// </summary>
        /// <remarks>
        /// Should only be used when creating the Control from layout.
        /// </remarks>
        /// <value></value>
        public String OnClick { private get; set; }

        /// <summary>
        /// Type of function to be called on an EnterKey event.
        /// </summary>
        /// <param name="caller">The control that called the event.</param>
        public delegate void EnterKeyDelegate(Control caller);

        /// <summary>
        /// Function to call when an Enter key is pushed down and pushed up while the control is
        /// capturing keyboard input.
        /// </summary>
        [XmlIgnore]
        EnterKeyDelegate EnterKeyEvent;

        /// <summary>
        /// String name of the function in the CodeBehind object to call on an EnterKey event.
        /// AssignEvents needs to be called before it will work.
        /// </summary>
        /// <remarks>
        /// Should only be used when creating the Control from layout.
        /// </remarks>
        /// <value></value>
        public String EnterKey { get; set; }

        /// <summary>
        /// Whether this control is visible and can be interacted with.
        /// </summary>
        /// <value></value>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// A user definable index to allow multiple controls of the same name to exist and be
        /// distinguished - for instance, when creating controls from a list.
        /// </summary>
        /// <value></value>
        public int Index {get; set;}

        /// <summary>
        /// Whether this control is in an active state. This means, for a textbox style control, that
        /// it is accepting input.
        /// </summary>
        /// <value></value>
        [XmlIgnore]
        public bool Activated { get; private set;} = false;

        /// <summary>
        /// Whether the mouse is inside the borders of the control.
        /// </summary>
        /// <param name="translatedState">
        /// The current mouse state, relative to the placement of the control.
        /// </param>
        /// <returns>True if the mouse is inside the control</returns>
        protected bool MouseInControl(MouseState translatedState)
        {
            return translatedState.X >= 0 &&
                   translatedState.Y >= 0 &&
                   translatedState.X <= Width &&
                   translatedState.Y <= Height;
        }

        /// <summary>
        /// Set the location of the cursor in an editable text control from where the mouse is.
        /// </summary>
        /// <param name="x">Relative X coordinate of the mouse.</param>
        /// <param name="activated">
        /// Whether the control is already activated (and showing a cursor)
        /// </param>
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

        /// <summary>
        /// Buttons of the mouse
        /// </summary>
        public enum MouseButton { Left };

        /// <summary>
        /// Process the MouseState from the parent control, or monogame.
        /// </summary>
        /// <param name="state">The state of the mouse in relative terms.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Deactivate the control. This usually means no longer accepting keyboard input.
        /// </summary>
        public virtual void Deactivate()
        {

            Activated = false;
            SpriteState = SpriteStates.Normal;
        }

        /// <summary>
        /// Deactivate the control. This usually means accepting keyboard input.
        /// </summary>
        public virtual void Activate()
        {
            Activated = true;
            SpriteState = SpriteStates.Active;
        }
        /// <summary>
        /// The state of the keyboard in the previous cycle of the program.
        /// </summary>
        [XmlIgnore]
        protected KeyboardState lastKeyboardState;

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

        /// <summary>
        /// Recursively find a child control by name
        /// </summary>
        /// <param name="name">Name searching for</param>
        /// <returns>The control, if found. null otherwise.</returns>
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
