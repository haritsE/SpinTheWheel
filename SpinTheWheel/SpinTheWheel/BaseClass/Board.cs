using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace WindowsGSM1.BaseClass
{
    class Board
    {
        public Texture2D soal, bg;
        public int rightAnswer, soalNumber;
        public TextureButton a, b, c, d;
        public List<TextureButton> buttons;
        SpriteFont gameFont;

        public Board(int ns, int ra)
        {
            soalNumber = ns;
            rightAnswer = ra;
            buttons = new List<TextureButton>();
            a = new TextureButton(100, 100, "A");
            buttons.Add(a);
        }

        public void LoadContent(ContentManager content)
        {
            soal = content.Load<Texture2D>("soal" + soalNumber);
            gameFont = content.Load<SpriteFont>("gamefont");
            foreach (TextureButton tb in buttons)
            {
                tb.LoadContent(content.Load<Texture2D>("button"), content.Load<Texture2D>("button_pressed"), gameFont);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach(TextureButton tb in buttons)
            {
                tb.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (TextureButton tb in buttons)
            {
                tb.Draw(sb);
            }
        }
    }
}
