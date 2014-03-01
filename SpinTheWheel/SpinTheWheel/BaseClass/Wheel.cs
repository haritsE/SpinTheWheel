using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WindowsGSM1.BaseClass
{
    class Wheel
    {
        public const float PI = 3.14159265359f;
        const float FRICTION = 0.985f;
        Vector2 curPos;
        Texture2D image;
        public float speedRot, curRot;
        int partition;
        float partitionWidth;
        public int choosenOption;
        public bool alreadyCalculated, isSpinnable;
        MouseState oldState, newState;
        Random random = new Random();
        
        public Wheel(int x, int y, int part, float spd)
        {
            speedRot = spd;
            partition = part; 
            //tiap partisi selebar: 2*PI/partition derajat
            partitionWidth = 2 * PI / (float) partition;
            curPos = new Vector2(x, y);
            curRot = 0f;
            choosenOption = 0;
            alreadyCalculated = true;
            isSpinnable = true;
        }

        public void reset()
        {
            speedRot = 0f;
            curRot = 0f;
            choosenOption = 0;
            alreadyCalculated = true;
        }

        public void stop()
        {
            speedRot = 0f;
            alreadyCalculated = true;
        }

        public void LoadContent(Texture2D img)
        {
            image = img;
        }

        public void spin(){
            if(isSpinnable)
                speedRot = (float)(0.5f + random.NextDouble() * 2) * (2 * Wheel.PI); //setengah putaran per detik
        }

        public bool insideWheel(MouseState state)
        {
            float x = state.X - curPos.X;
            float y = state.Y - curPos.Y;
            Console.WriteLine((x*x + y*y));
            if (x*x + y*y < (image.Width/2)*(image.Width/2))
            {
                return true;
            }
            else
                return false;
        }

        public bool isSpinning()
        {
            return (Math.Abs(speedRot) > 0.01f);
        }

        public void Update(GameTime gameTime)
        {
            //clicking
            newState = Mouse.GetState();
            if (newState.LeftButton == ButtonState.Pressed)
            {
                if (insideWheel(newState))
                {
                    //Console.WriteLine("Inside!");
                }
                else
                {
                    //Console.WriteLine("Outside!");
                }

            }

            curRot += speedRot * (float) gameTime.ElapsedGameTime.TotalSeconds;
            speedRot *= FRICTION;
            //Console.WriteLine("Already calculated? :" + alreadyCalculated);
            
                //Console.WriteLine("Calculated.");
            curRot %= 2 * PI;
            choosenOption = (int) Math.Floor(curRot / partitionWidth);            
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(image, new Rectangle((int)curPos.X, (int)curPos.Y, image.Width, image.Height), null, Color.White, curRot, new Vector2(image.Width/2, image.Height/2), SpriteEffects.None, 0);
        }
    }
}
