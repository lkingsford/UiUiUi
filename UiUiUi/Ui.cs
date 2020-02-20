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
    public class Ui: Control
    {
        SourceSprite CursorSprite;
        int CursorWidth = 14;
        int CursorHeight = 19;
        public Ui() : base()
        {
            Name = "Ui";
        }

        public void LoadLayoutFromFile(string filename, object codeBehindObject)
        {
            var serializer = new XmlSerializer(typeof(List<Control>), new XmlRootAttribute("Layout"));
            var filestream = new FileStream(filename, FileMode.Open);
            CursorSprite = new SourceSprite();
            CursorSprite.Asset = "Controls/Cursor";
            Controls = (List<Control>)serializer.Deserialize(filestream);
            AssignEvents(codeBehindObject);
        }

        public override void LoadSprites(ContentManager content)
        {
             base.LoadSprites(content);
             CursorSprite.LoadSprite(content);
        }

        public override bool ProcessMouse(MouseState state)
        {
            base.ProcessMouse(state);
            return true;
        }

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