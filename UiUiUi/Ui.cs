using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace UiUiUi
{
    /// <summary>
    /// Parent level control that gets input directly from Monogame, and outputs on the screen.
    /// </summary>
    public class Ui: Control
    {
        /// <summary>
        /// Sprite of the cursor to display
        /// </summary>
        SourceSprite CursorSprite;
        /// <summary>
        /// Width of the cursor sprite
        /// </summary>
        int CursorWidth = 14;
        /// <summary>
        /// Height of the cursor sprite
        /// </summary>
        int CursorHeight = 19;
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <returns></returns>
        public Ui() : base()
        {
            Name = "Ui";
        }

        /// <summary>
        /// Create child controls from a given layout XML file.
        /// </summary>
        /// <param name="filename">Th path to a layout XML file.</param>
        /// <param name="codeBehindObject">
        /// The object that code on this Ui referes to. Any event functions are defined in this
        /// object.
        /// </param>
        public void LoadLayoutFromFile(string filename, object codeBehindObject)
        {
            var serializer = new XmlSerializer(typeof(List<Control>), new XmlRootAttribute("Layout"));
            var filestream = new FileStream(filename, FileMode.Open);
            CursorSprite = new SourceSprite();
            CursorSprite.Asset = "Controls/Cursor";
            Controls = (List<Control>)serializer.Deserialize(filestream);
            AssignEvents(codeBehindObject);
        }

        /// <summary>
        /// Load the sprites, including the cursor, from the given content manager.
        /// </summary>
        /// <param name="content">Content manager to load from.</param>
        public override void LoadSprites(ContentManager content)
        {
             base.LoadSprites(content);
             CursorSprite.LoadSprite(content);
        }

        /// <summary>
        /// Process the mouse.
        /// </summary>
        /// <param name="state">The current MouseState.</param>
        /// <returns>True</returns>
        public override bool ProcessMouse(MouseState state)
        {
            base.ProcessMouse(state);
            return true;
        }

        /// <summary>
        /// Draw these controls to the graphic device.
        /// </summary>
        /// <param name="device">Device to use</param>
        /// <param name="parentsRenderTarget">Render target to render to.</param>
        /// <param name="controlOffsetX">The horizontal offset of where to draw.</param>
        /// <param name="controlOffsetY">The vertical offset of where to draw.</param>
        public override void Draw(GraphicsDevice device, RenderTarget2D parentsRenderTarget = null, int controlOffsetX = 0, int controlOffsetY = 0)
        {
            var spriteBatch = new SpriteBatch(device);
            base.Draw(device, parentsRenderTarget, controlOffsetX, controlOffsetY);

            spriteBatch.Begin();
            spriteBatch.Draw(CursorSprite.Sprite,
                             new Rectangle(LastMouseState.X, LastMouseState.Y,
                                CursorWidth, CursorHeight),
                             null,
                             Color.White);
            spriteBatch.End();
        }
    }
}