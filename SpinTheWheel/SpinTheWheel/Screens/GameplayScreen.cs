#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using WindowsGSM1.BaseClass;
using System.Collections.Generic;
#endregion

namespace WindowsGSM1
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        public int activePlayer = 0;
        public Texture2D arrow;
        ContentManager content;
        string[,] partitionContent;
        SpriteFont gameFont;
        Wheel wheel;
        bool tabChoosable = true;
        bool isResultShown = true;
        public static int skorPlayer1, skorPlayer2, skorPlayer3;
        Vector2 playerPosition = new Vector2(100, 100);
        Vector2 enemyPosition = new Vector2(100, 100);
        MouseState oldState, newState;
        Vector2 initDist, curDist;
        TextureButton buttonSpin, buttonTab1, buttonTab2, buttonTab3, buttonShow;
        public static List<List<int>> soalPernah;
        List<TextureButton> buttons, tabButtons;
        SpriteFont font;
        Random random = new Random();
        float timer;

        float pauseAlpha;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            soalPernah = new List<List<int>>();
            soalPernah.Add(new List<int>());
            soalPernah.Add(new List<int>());
            soalPernah.Add(new List<int>());

            partitionContent = new string[,]{
            
                { "hukuman", "soal1", "bonus", "hukuman", "soal2", "soal3", "bonus", "soal4", "soal5", "hukuman", "soal6", "hukuman", "soal7", "bonus", "soal8", "hukuman", "bonus", "soal9"},
                   
                { "soal1", "hukuman", "soal2", "hukuman", "soal3", "bonus", "bonus", "soal4", "soal5", "soal6", "hukuman", "bonus", "soal7", "soal8", "bonus", "soal9", "hukuman", "hukuman"},

                { "soal1", "soal2", "hukuman", "bonus", "soal3", "bonus", "soal4", "soal5", "hukuman", "soal6", "hukuman", "soal7", "bonus", "soal8", "hukuman", "soal9", "hukuman", "bonus"}
            };
            
            skorPlayer1 = skorPlayer2 = skorPlayer3 = 0;
            timer = 0f;
            initDist = new Vector2(0, 0);
            curDist = new Vector2(0, 0);
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            wheel = new Wheel(500, 280, 18, 0f);

            buttons = new List<TextureButton>();
            tabButtons = new List<TextureButton>();

            buttonSpin = new TextureButton(800, 250, "");
            //buttonShow = new TextureButton(800, 200, "Lihat Soal!");
            buttonTab1 = new TextureButton(100, 525, "");
            buttonTab2 = new TextureButton(400, 525, "");
            buttonTab3 = new TextureButton(700, 525, "");


            //buttonShow.Selected += new EventHandler<PlayerIndexEventArgs>(buttonShow_Selected);
            buttonSpin.Selected += new EventHandler<PlayerIndexEventArgs>(buttonSpin_Selected);
            buttonTab1.Selected += new EventHandler<PlayerIndexEventArgs>(buttonTab1_Selected);
            buttonTab2.Selected += new EventHandler<PlayerIndexEventArgs>(buttonTab2_Selected);
            buttonTab3.Selected += new EventHandler<PlayerIndexEventArgs>(buttonTab3_Selected);

            //buttons.Add(buttonShow);
            buttons.Add(buttonSpin);
            tabButtons.Add(buttonTab1);
            tabButtons.Add(buttonTab2);
            tabButtons.Add(buttonTab3);
        }

        void buttonTab3_Selected(object sender, PlayerIndexEventArgs e)
        {
            if (tabChoosable)
            {
                activePlayer = 2;
                wheel.reset();
            }
        }

        void buttonTab2_Selected(object sender, PlayerIndexEventArgs e)
        {
            if (tabChoosable)
            {
                activePlayer = 1;
                wheel.reset();
            }
        }

        void buttonTab1_Selected(object sender, PlayerIndexEventArgs e)
        {
            if (tabChoosable)
            {
                activePlayer = 0;
                wheel.reset();
            }
        }

        public static SoalScreen soal;
        void buttonShow_Selected(object sender, PlayerIndexEventArgs e)
        {
            tabChoosable = true;
            wheel.isSpinnable = true;
            MessageBoxScreen msg = new MessageBoxScreen("Tekan enter bila sudah siap!", false, new Vector2(500, 300));
            msg.Accepted += new EventHandler<PlayerIndexEventArgs>(msg_Accepted);
            soal = new SoalScreen(activePlayer, wheel.choosenOption);
            ScreenManager.AddScreen(soal, ControllingPlayer);
            ScreenManager.AddScreen(msg, ControllingPlayer);
        }

        void showSpinResult()
        {
            wheel.stop();
            string action = partitionContent[activePlayer, wheel.choosenOption];
            if (action.Contains("soal"))
            {
                //kalo soal belum pernah dipake
                if (soalPernah[activePlayer].IndexOf((int)(partitionContent[activePlayer, wheel.choosenOption][partitionContent[activePlayer, wheel.choosenOption].Length - 1] - 48)) == -1)
                {
                    soalPernah[activePlayer].Add((int)(partitionContent[activePlayer, wheel.choosenOption][partitionContent[activePlayer, wheel.choosenOption].Length - 1] - 48));
                    MessageBoxScreen msgSoal = new MessageBoxScreen("Kamu dapat Soal!", false);
                    msgSoal.Accepted += new EventHandler<PlayerIndexEventArgs>(msgSoal_Accepted);
                    ScreenManager.AddScreen(msgSoal, ControllingPlayer);
                }
                else //soal udah pernah dipake, spin lagi
                {
                    MessageBoxScreen msgPernah = new MessageBoxScreen("Sudah pernah dapat soal ini... Putar ulang ya...", false);
                    msgPernah.Accepted += new EventHandler<PlayerIndexEventArgs>(msgPernah_Accepted);
                    ScreenManager.AddScreen(msgPernah, ControllingPlayer);
                }
                
            }
            else if (action.Contains("hukuman"))
            {
                MessageBoxScreen msgHukum = new MessageBoxScreen("Kamu dapat Hukuman!", false);
                msgHukum.Accepted += new EventHandler<PlayerIndexEventArgs>(msgHukum_Accepted);
                ScreenManager.AddScreen(msgHukum, ControllingPlayer);
            }
            else if (action.Contains("extra"))
            {
                MessageBoxScreen msgExtra = new MessageBoxScreen("Kamu dapat extra spin!", false);
                msgExtra.Accepted += new EventHandler<PlayerIndexEventArgs>(msgHukum_Accepted);
                ScreenManager.AddScreen(msgExtra, ControllingPlayer);
            }
            else if (action.Contains("bonus"))
            {
                MessageBoxScreen msgExtra = new MessageBoxScreen("Kamu dapat bonus!", false);
                msgExtra.Accepted += new EventHandler<PlayerIndexEventArgs>(msgHukum_Accepted);
                ScreenManager.AddScreen(msgExtra, ControllingPlayer);

                if (activePlayer == 0)
                {
                    skorPlayer1 += 100;
                }

                if (activePlayer == 1)
                {
                    skorPlayer2 += 100;
                }

                if (activePlayer == 2)
                {
                    skorPlayer3 += 100;
                }
            }
        }

        void msgPernah_Accepted(object sender, PlayerIndexEventArgs e)
        {
            wheel.isSpinnable = true;
        }

        void msgHukum_Accepted(object sender, PlayerIndexEventArgs e)
        {
            //ExitScreen();
            tabChoosable = true;
            wheel.isSpinnable = true;
        }

        void msgSoal_Accepted(object sender, PlayerIndexEventArgs e)
        {
            invokeSoal();
        }

        void invokeSoal()
        {
            tabChoosable = true;
            wheel.isSpinnable = true;
            MessageBoxScreen msg = new MessageBoxScreen("Tekan enter bila sudah siap!", false, new Vector2(500, 300));
            msg.Accepted += new EventHandler<PlayerIndexEventArgs>(msg_Accepted);
            //soal = new SoalScreen(activePlayer, wheel.choosenOption);
            soal = new SoalScreen(activePlayer, (int)(partitionContent[activePlayer, wheel.choosenOption][partitionContent[activePlayer, wheel.choosenOption].Length - 1] - 48));
            ScreenManager.AddScreen(soal, ControllingPlayer);
            ScreenManager.AddScreen(msg, ControllingPlayer);
        }

        void msg_Accepted(object sender, PlayerIndexEventArgs e)
        {
            if (soal != null)
            {
                soal.activate();
            }
        }

        void buttonSpin_Selected(object sender, PlayerIndexEventArgs e)
        {
            isResultShown = false;
            wheel.alreadyCalculated = false;
            wheel.spin();
            wheel.isSpinnable = false;
            tabChoosable = false;
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            font = content.Load<SpriteFont>("menufont");
            gameFont = content.Load<SpriteFont>("gamefont");
            wheel.LoadContent(content.Load<Texture2D>("wheel"));
            arrow = content.Load<Texture2D>("arrow");

            foreach (TextureButton tb in buttons)
            {
                //tb.LoadContent(content.Load<Texture2D>("button"), content.Load<Texture2D>("button_pressed"), content.Load<SpriteFont>("menufont"));
            }

            buttonSpin.LoadContent(content.Load<Texture2D>("spin_it"), content.Load<Texture2D>("spin_it"), font);

            for (int i = 0; i < tabButtons.Count; i++)
            {
                tabButtons[i].LoadContent(content.Load<Texture2D>("pemain" + (i+1) + "_unpressed"), content.Load<Texture2D>("pemain" + (i+1)), font);
            }
            foreach (TextureButton tb in tabButtons)
            {
                //tb.LoadContent(content.Load<Texture2D>("button"), content.Load<Texture2D>("button_pressed"), content.Load<SpriteFont>("menufont"));
            }

            //buttonSpin.LoadContent(content.Load<Texture2D>("button"), font);
            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            Thread.Sleep(300);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            //Console.WriteLine(!wheel.isSpinning() + " " + !wheel.isSpinnable); 
            if (!wheel.isSpinning() && !wheel.isSpinnable) //udah selesai muter dan gak bisa diputer
            {
                if (!isResultShown)
                {
                    showSpinResult();
                    isResultShown = true;
                }
            }

            newState = Mouse.GetState();

            if (newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released)
            {
                curDist.X = initDist.X = newState.X;
                curDist.Y = initDist.Y = newState.Y;
            }

            if (newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Pressed)
            {
                curDist.X = newState.X;
                curDist.Y = newState.Y;
            }
            //Console.WriteLine("Distance: " + Vector2.Distance(initDist, curDist));
            //Mouse Debugging
            //wheel.curRot = Vector2.Distance(initDist, curDist)/100;

            oldState = newState;

            for (int i = 0; i < tabButtons.Count; i++)
            {
                if (tabChoosable)
                {
                    if (activePlayer == i)
                    {
                        tabButtons[i].setPressed(true);
                    }
                    else
                        tabButtons[i].setPressed(false);
                }
                tabButtons[i].Update(gameTime);
            }

            foreach (TextureButton tb in buttons)
            {
                tb.Update(gameTime);
            }
            
            wheel.Update(gameTime);
            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                // Apply some random jitter to make the enemy move around.
                const float randomization = 10;

                enemyPosition.X += (float)(random.NextDouble() - 0.5) * randomization;
                enemyPosition.Y += (float)(random.NextDouble() - 0.5) * randomization;

                // Apply a stabilizing force to stop the enemy moving off the screen.
                Vector2 targetPosition = new Vector2(
                    ScreenManager.GraphicsDevice.Viewport.Width / 2 - gameFont.MeasureString("Insert Gameplay Here").X / 2,
                    200);

                enemyPosition = Vector2.Lerp(enemyPosition, targetPosition, 0.05f);

                // TODO: this game isn't very fun! You could probably improve
                // it by inserting something more interesting in this space :-)
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                // Otherwise move the player position.
                Vector2 movement = Vector2.Zero;

                if (keyboardState.IsKeyDown(Keys.Left))
                    movement.X--;

                if (keyboardState.IsKeyDown(Keys.Right))
                    movement.X++;

                if (keyboardState.IsKeyDown(Keys.Up))
                    movement.Y--;

                if (keyboardState.IsKeyDown(Keys.Down))
                    movement.Y++;

                Vector2 thumbstick = gamePadState.ThumbSticks.Left;

                movement.X += thumbstick.X;
                movement.Y -= thumbstick.Y;

                if (movement.Length() > 1)
                    movement.Normalize();

                playerPosition += movement * 2;
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            //ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();
            wheel.Draw(spriteBatch);
            spriteBatch.Draw(arrow, new Vector2(499 - arrow.Width/2, -10), Color.White);
            foreach (TextureButton tb in tabButtons)
            {
                tb.Draw(spriteBatch);
            }

            foreach (TextureButton tb in buttons)
            {
                tb.Draw(spriteBatch);
            }

            /* Draw Score */
            spriteBatch.DrawString(font, "Skor Pemain 1:\n" + skorPlayer1.ToString(), new Vector2(10, 0), Color.White);
            spriteBatch.DrawString(font, "Skor Pemain 2:\n" + skorPlayer2.ToString(), new Vector2(10, 100), Color.White);
            spriteBatch.DrawString(font, "Skor Pemain 3:\n" + skorPlayer3.ToString(), new Vector2(10, 200), Color.White);

            spriteBatch.DrawString(font, wheel.choosenOption.ToString(), new Vector2(490, 0), Color.White);
            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }


        #endregion
    }
}
