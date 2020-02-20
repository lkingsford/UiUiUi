using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace GeneralUsageExample
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class DesktopGLGame : Microsoft.Xna.Framework.Game
    {
        public class StartOptions
        {
            public bool StartLocalGame = false;
            public string Hostname = null;
            public int Port = 3001;
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        /// <summary>
        /// Current stack of states.
        /// Front of stack is the current state.
        /// </summary>
        List<State> States = new List<State>();

        public DesktopGLGame() : base()
        {
            graphics = new GraphicsDeviceManager(this);
        }

        private StartOptions _StartOptions;

        /// <summary>
        /// Current state
        /// </summary>
        State CurrentState
        {
            get
            {
                // Supposedly, .Last is optimized for List<>... I hope that LastOrDefault is too
                return States.LastOrDefault();
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

            // Initialise static state fields
            State.AppContentManager = Content;
            State.AppGraphicsDevice = GraphicsDevice;
            State.AppSpriteBatch = spriteBatch;
            State.ScreenWidth = GraphicsDevice.DisplayMode.Width;
            State.ScreenHeight = GraphicsDevice.DisplayMode.Height;

            // Allow states to manipulate states
            State.StateStack = States;
            State.GameApp = this;

            // Create the first state
            States.Add(new DemoState());
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Content.RootDirectory = "MgContent";
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // If the stack is empty, then it's quittin time
            if (CurrentState == null)
                Exit();

            // Otherwise, that's the thing we're doing
            CurrentState?.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            CurrentState?.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
