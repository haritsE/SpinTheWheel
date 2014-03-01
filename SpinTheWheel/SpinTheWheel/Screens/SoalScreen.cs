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
using Microsoft.Xna.Framework.Audio;
#endregion

namespace WindowsGSM1
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class SoalScreen : GameScreen
    {
        #region Fields

        public int activePlayer;
        bool isTiming = false;
        ContentManager content;
        SpriteFont gameFont;
        TimeSpan time;
        Texture2D board;
        MouseState oldState, newState;
        TextureButton buttonA, buttonB, buttonC, buttonD, buttonE;
        List<TextureButton> buttons;
        SpriteFont font;
        Random random = new Random();
        float timer;
        int noSoal;
        float pauseAlpha;
        Texture2D soalTexture;
        Dictionary<string, string> answers;
        SoundEffect sfxFail, sfxSuccess1, sfxSuccess2;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public SoalScreen(int aPlayer, int noSoal)
        {
            this.noSoal = noSoal;
            time = TimeSpan.FromMinutes(2);
            activePlayer = aPlayer;
            timer = 0f;
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            answers = new Dictionary<string, string>();
            /* Jawaban soal A */
            answers.Add("A1", "C");
            answers.Add("A2", "B");
            answers.Add("A3", "C");
            answers.Add("A4", "B");
            answers.Add("A5", "B");
            answers.Add("A6", "D");
            answers.Add("A7", "D");
            answers.Add("A8", "C");
            answers.Add("A9", "A");
            
            /* Jawaban soal B */
            answers.Add("B1", "D");
            answers.Add("B2", "B");
            answers.Add("B3", "C");
            answers.Add("B4", "D");
            answers.Add("B5", "C");
            answers.Add("B6", "E");
            answers.Add("B7", "D");
            answers.Add("B8", "C");
            answers.Add("B9", "A");

            /* Jawaban soal C */
            answers.Add("C1", "D");
            answers.Add("C2", "A");
            answers.Add("C3", "D");
            answers.Add("C4", "D");
            answers.Add("C5", "E");
            answers.Add("C6", "D");
            answers.Add("C7", "C");
            answers.Add("C8", "A");
            answers.Add("C9", "D");

            buttons = new List<TextureButton>();
        
            buttonA = new TextureButton(160, 420, "");
            buttonB = new TextureButton(310, 420, "");
            buttonC = new TextureButton(460, 420, "");
            buttonD = new TextureButton(610, 420, "");
            buttonE = new TextureButton(760, 420, "");

            buttonA.Selected += new EventHandler<PlayerIndexEventArgs>(buttonOption_Selected);
            buttonB.Selected += new EventHandler<PlayerIndexEventArgs>(buttonOption_Selected);
            buttonC.Selected += new EventHandler<PlayerIndexEventArgs>(buttonOption_Selected);
            buttonD.Selected += new EventHandler<PlayerIndexEventArgs>(buttonOption_Selected);
            buttonE.Selected += new EventHandler<PlayerIndexEventArgs>(buttonOption_Selected);
            
            buttons.Add(buttonA);
            buttons.Add(buttonB);
            buttons.Add(buttonC);
            buttons.Add(buttonD);
            buttons.Add(buttonE);
        }

        void buttonOption_Selected(object sender_, PlayerIndexEventArgs e)
        {
            TextureButton sender = (TextureButton) sender_;

            string ans = "";
            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i] == sender)
                {
                    ans += (char)(65 + i);
                }
            }
            //Console.WriteLine("answer: " + ans);

            processAnswer(ans);
        }

        

        void processAnswer(string ans)
        {
            string key = getSoalCode(activePlayer) + noSoal.ToString();
            if (answers.ContainsKey(key))
	        {
                if(isTiming){
                    if (ans == answers[key]) //success!
                    {
                        sfxSuccess1.Play();
                        sfxSuccess2.Play();
                        time = TimeSpan.FromMinutes(2);
                        isTiming = false;
                        MessageBoxScreen msgTrue = new MessageBoxScreen("Selamat, Anda benar!", false);
                        msgTrue.Accepted += new EventHandler<PlayerIndexEventArgs>(msgTrue_Accepted);
                        ScreenManager.AddScreen(msgTrue, ControllingPlayer);
                    } else { //fail! :(
                        sfxFail.Play();
                        time = TimeSpan.FromMinutes(2);
                        isTiming = false;
                        MessageBoxScreen msg = new MessageBoxScreen("Maaf, jawaban Anda salah!", false);
                        msg.Accepted += new EventHandler<PlayerIndexEventArgs>(msg_Accepted);
                        ScreenManager.AddScreen(msg, ControllingPlayer);
                    }
                } //else: do nothing
	        }
        }

        void msgTrue_Accepted(object sender, PlayerIndexEventArgs e)
        {
            if(activePlayer == 0){
                GameplayScreen.skorPlayer1 += 100;   
            }

            if (activePlayer == 1)
            {
                GameplayScreen.skorPlayer2 += 100;
            }

            if (activePlayer == 2)
            {
                GameplayScreen.skorPlayer3 += 100;
            }
            ExitScreen();
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        /// 

        public char getSoalCode(int playerNumber)
        {
            return (char)('A' + playerNumber);
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            
            soalTexture = content.Load<Texture2D>("soal/" + getSoalCode(activePlayer) + "/cats" + noSoal);
            board = content.Load<Texture2D>("board");
            font = content.Load<SpriteFont>("menufont");
            gameFont = content.Load<SpriteFont>("gamefont");

            sfxFail = content.Load<SoundEffect>("music/fail");
            sfxSuccess1 = content.Load<SoundEffect>("music/cheer");
            sfxSuccess2 = content.Load<SoundEffect>("music/tada");


            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].LoadContent(content.Load<Texture2D>("but" + (char)(65 + i)), content.Load<Texture2D>("but" + (char)(65 + i)), content.Load<SpriteFont>("menufont"));
            }

            foreach (TextureButton tb in buttons)
            {
                //tb.LoadContent(content.Load<Texture2D>("button_choice"), content.Load<Texture2D>("button_pressed"), content.Load<SpriteFont>("menufont"));
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

            if (isTiming)
            {
                time = time.Subtract(TimeSpan.FromSeconds(gameTime.ElapsedGameTime.TotalSeconds));
            }

            //Console.WriteLine("total seconds: " + time.TotalSeconds);

            if (time.TotalSeconds < 0)
            {
                time = TimeSpan.FromSeconds(5);
                isTiming = false;
                MessageBoxScreen msg = new MessageBoxScreen("Waktu habis! Tekan Enter untuk melanjutkan...", false);
                msg.Accepted += new EventHandler<PlayerIndexEventArgs>(msg_Accepted);
                ScreenManager.AddScreen(msg, ControllingPlayer);
            }
            newState = Mouse.GetState();

            if (newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released)
            {
                
            }

            if (newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Pressed)
            {
                
            }
            //Console.WriteLine("Distance: " + Vector2.Distance(initDist, curDist));
            //Mouse Debugging
            //wheel.curRot = Vector2.Distance(initDist, curDist)/100;

            oldState = newState;

            foreach (TextureButton tb in buttons)
            {
                tb.Update(gameTime);
            }

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                // Apply some random jitter to make the enemy move around.
                const float randomization = 10;

                // Apply a stabilizing force to stop the enemy moving off the screen.
                Vector2 targetPosition = new Vector2(
                    ScreenManager.GraphicsDevice.Viewport.Width / 2 - gameFont.MeasureString("Insert Gameplay Here").X / 2,
                    200);

                
                // TODO: this game isn't very fun! You could probably improve
                // it by inserting something more interesting in this space :-)
            }
        }

        void msg_Accepted(object sender, PlayerIndexEventArgs e)
        {
            GameplayScreen.soal = null;
            ExitScreen();
        }

        public void activate()
        {
            isTiming = true;
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

            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            //ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0
            // Our player and enemy are both actually just text strings
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();
            spriteBatch.Draw(board, new Rectangle(50, 50, board.Width + 100, board.Height), Color.White);
            spriteBatch.Draw(soalTexture, new Rectangle(100, 150, 800, 250), Color.White);
            spriteBatch.DrawString(gameFont, time.ToString(@"hh\:mm\:ss"), new Vector2(400, 65), Color.White);

            foreach (TextureButton tb in buttons)
            {
                tb.Draw(spriteBatch);
            }

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
