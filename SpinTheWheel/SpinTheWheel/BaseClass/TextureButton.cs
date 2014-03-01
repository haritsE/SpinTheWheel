using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WindowsGSM1.BaseClass
{
    class TextureButton
    {
        private SpriteFont font;
        private Texture2D image, image1, image2;
        public Vector2 curPos;
        private String buttonText;
        public event EventHandler<PlayerIndexEventArgs> Selected;
        private MouseState oldState, newState;
        private Rectangle area;

        public TextureButton(int x, int y, string text)
        {
            curPos = new Vector2(x, y);
            buttonText = text;
            area = new Rectangle();
        }

        public void LoadContent(Texture2D img, Texture2D img2, SpriteFont sf)
        {
            image2 = img2;
            image1 = img;
            image = image1;
            font = sf;
        }

        public int GetHeight()
        {
            return font.LineSpacing;
        }

        public string GetText()
        {
            return buttonText;
        }

        public void setPressed(bool isPressed)
        {
            if (isPressed)
            {
                image = image2;
            }
            else
            {
                image = image1;
            }
        }

        /// <summary>
        /// Queries how wide the entry is, used for centering on the screen.
        /// </summary>
        public int GetWidth()
        {
            return (int) font.MeasureString(buttonText).X;
        }

        public void Update(GameTime gameTime)
        {
            newState = Mouse.GetState();
            
            if (oldState.LeftButton == ButtonState.Pressed && newState.LeftButton == ButtonState.Released && area.Contains(new Point(newState.X, newState.Y)))
            {
                //Console.WriteLine("Contains!");

                if(Selected != null)
                    Selected(this, null);
            }
            oldState = newState;
        }


        Vector2 origin = new Vector2(0, 0);
        public void Draw(SpriteBatch batch)
        {
            area.X = (int)curPos.X - 10;
            area.Y = (int)curPos.Y;
            area.Width = image.Width;
            area.Height = image.Height;
            //batch.Draw(image, area, Color.White);
            batch.Draw(image, new Vector2(area.X, area.Y), Color.White);
            batch.DrawString(font, buttonText, new Vector2(curPos.X, curPos.Y), Color.White);
        }
    }
}
