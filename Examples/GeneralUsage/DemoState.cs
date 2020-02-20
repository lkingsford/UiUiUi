using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UiUiUi;

namespace GeneralUsageExample
{
    class DemoState : State
    {
        /// <summary>
        /// The game being played
        /// </summary>
        protected UiUiUi.Ui Ui;

        /// <summary>
        /// Create a game interface from a given game
        /// </summary>
        /// <param name="Game">Game object that is being played</param>
        public DemoState() : base()
        {
            Ui = new UiUiUi.Ui();
            Ui.Width = AppGraphicsDevice.PresentationParameters.BackBufferWidth;
            Ui.Height = AppGraphicsDevice.PresentationParameters.BackBufferHeight;
            Ui.LoadLayoutFromFile("Content\\Layout\\layout.xml", this);
            Ui.LoadSprites(AppContentManager);
        }

        /// <summary>
        /// State as of the previous Update
        /// </summary>
        private KeyboardState LastState;

        /// <summary>
        /// Run logic for this state - including input
        /// </summary>
        /// <param name="GameTime">Snapshot of timing</param>
        public override void Update(GameTime GameTime)
        {
            Ui.ProcessMouse(Mouse.GetState());
            Ui.ProcessKeyboard(Keyboard.GetState());
        }

        /// <summary>
        /// Draw this state
        /// </summary>
        /// <param name="GameTime">Snapshot of timing</param>
        public override void Draw(GameTime GameTime)
        {
            Ui.Draw(AppGraphicsDevice);
        }

        private void QuitClick(Control caller,
                               int relativeX,
                               int relativeY,
                               Control.MouseButton button)
        {
            StateStack.Remove(this);
        }

        private void OnClick(Control caller,
                               int relativeX,
                               int relativeY,
                               Control.MouseButton button)
        {
            var buttonToChange = Ui.Find("Button1");
            buttonToChange.Text = "You clicked it!";
        }


        private void ClearTextbox(Control caller)
        {
            caller.Text = "";
        }
    }
}
