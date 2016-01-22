using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using EV3MessengerLib;
using System.Collections.Generic;
using System;

namespace ProftaakGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Song music;

        //Cursor
        Texture2D cursor;
        int cursorX;
        int cursorY;
        Vector2 cursorPosition;
        Vector2 platePosition;
        Rectangle cursorBoundingBox;

        //Extra sprites
        Texture2D background;
        Texture2D shelf;
        Texture2D roof;
        Texture2D curtainL;
        Texture2D curtainR;
        Texture2D preScreen;

        Vector2 preScreenPosition;

        //Sound Effects
        List<SoundEffect> hitSoundEffects;
        SoundEffect screenOpen;
        SoundEffect whistle;
        SoundEffect countdown;

        private EV3Messenger ev3Messenger;

        float spawnTimer = 1.5f;
        const float spawnTIMER = 1.5f;

        float gameTimerStart;
        float gameTimer = 30;
        const float gameTIMER = 30;
        bool gameTimerOn = false;

        bool countdownSoundPlayed = false;

        List<Target> targets = new List<Target>();

        
        int score = 0;
        bool inGameMode = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1440;
            graphics.PreferredBackBufferHeight = 900;
            graphics.IsFullScreen = true;

            ev3Messenger = new EV3Messenger();
            ev3Messenger.Connect("COM12");

            hitSoundEffects = new List<SoundEffect>();

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            cursorX = GraphicsDevice.Viewport.Bounds.Width / 2;
            cursorY = GraphicsDevice.Viewport.Bounds.Height / 2;
            cursorPosition = new Vector2(cursorX, cursorY);

            platePosition = new Vector2(0, 100);
            preScreenPosition = new Vector2(0, 0);

            ScreenManager.Instance.Graphics = GraphicsDevice;

            if (ev3Messenger.IsConnected)
            {
                Console.WriteLine("Succesful!");
            }
            else
            {
                Console.WriteLine("Error Couldn't connect to brick.");
            }


            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ScreenManager.Instance.LoadContent(Content);

            // TODO: use this.Content to load your game content here
            cursor = Content.Load<Texture2D>("cursor");
            background = Content.Load<Texture2D>("BG");
            shelf = Content.Load<Texture2D>("Shelf");                              
            roof = Content.Load<Texture2D>("roof");
            curtainL = Content.Load<Texture2D>("curtainL");                          
            curtainR = Content.Load<Texture2D>("curtainR");
            preScreen = Content.Load<Texture2D>("preScreen");

            music = Content.Load<Song>("Sounds/music");

            hitSoundEffects.Add(Content.Load<SoundEffect>("Sounds/hit1"));
            hitSoundEffects.Add(Content.Load<SoundEffect>("Sounds/hit2"));
            hitSoundEffects.Add(Content.Load<SoundEffect>("Sounds/hit3"));

            screenOpen = Content.Load<SoundEffect>("Sounds/screenOpen");
            whistle = Content.Load<SoundEffect>("Sounds/whistle");
            countdown = Content.Load<SoundEffect>("Sounds/countdown");

            MediaPlayer.Play(music);
            MediaPlayer.IsRepeating = true;

            cursorBoundingBox = new Rectangle(cursorX, cursorY, (int)cursor.Width, (int)cursor.Height);

            foreach (Target target in targets)
            {
                target.LoadContent();
            }
            
        }


        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

       
        protected override void Update(GameTime gameTime)
        {
            MouseState MS = Mouse.GetState();

            //Make cursor sprite track mouse movement
            if (ev3Messenger.IsConnected)
            {
                EV3Message message = ev3Messenger.ReadMessage();
                if (message != null && message.MailboxTitle == "statusBox")
                {
                    string[] objects = message.ValueAsText.Split(',');
                    int controllerDegX = Convert.ToInt32(objects[0]);
                    int controllerDegY = Convert.ToInt32(objects[1]);

                    int screenStepX = GraphicsDevice.Viewport.Width / 50;
                    int screenStepY = GraphicsDevice.Viewport.Height / 50;

                    int controllerX = ((controllerDegX * -1) * screenStepX) + (GraphicsDevice.Viewport.Width / 2);
                    int controllerY = ((controllerDegY * -1) * screenStepY) + (GraphicsDevice.Viewport.Height / 2);

                    cursorPosition = new Vector2(controllerX, controllerY);
                    //Make the bounding box for the cursor follow the mouse movement
                    cursorBoundingBox = new Rectangle(controllerX, controllerY, (int)cursor.Width, (int)cursor.Height);
                }
            }
            else
            {
                int mouseX = MS.X;
                int mouseY = MS.Y;

                cursorPosition = new Vector2(mouseX, mouseY);
                //Make the bounding box for the cursor follow the mouse movement
                cursorBoundingBox = new Rectangle(mouseX, mouseY, (int)cursor.Width, (int)cursor.Height);
            }
            

            //Start game by pressing on spacebar
            KeyboardState KS = Keyboard.GetState();
            
            //Only allow the spacebar to trigger when inGameMode
            if (!inGameMode)
            {
                if (KS.IsKeyDown(Keys.Space))
                {
                    screenOpen.Play();
                    whistle.Play();
                    inGameMode = true;
                    gameTimerOn = true;
                }
            }

            //Quit game when pressing ESC key
            if (KS.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            

            //Check to see if inGameMode
            if (inGameMode)
            {
                if (preScreenPosition.Y > -900)
                {
                    preScreenPosition.Y += (preScreenPosition.Y - 900) / 100;
                }

                if (gameTimerOn)
                {
                    gameTimerStart = (float)gameTime.TotalGameTime.TotalSeconds;
                    gameTimerOn = false;
                }

                gameTimer = (float)gameTime.TotalGameTime.TotalSeconds - gameTimerStart;

                if (gameTimer > gameTIMER - 3)
                {
                    if (!countdownSoundPlayed)
                    {
                        countdown.Play();
                        countdownSoundPlayed = true;
                    }
                }
            
                if (gameTimer > gameTIMER)
                {
                    inGameMode = false;
                    gameTimerOn = false;
                    gameTimer = gameTIMER;
                    countdownSoundPlayed = false;
                    screenOpen.Play();
                    score = 0;
                }
            }
            else
            {
                if (preScreenPosition.Y < 0)
                {
                    preScreenPosition.Y -= (preScreenPosition.Y - 900) / 100;
                }
            }

            //Check for Collision on Mouse.LeftButton Press
            if (inGameMode)
            {
                if (MS.LeftButton == ButtonState.Pressed)
                {
                    checkCollision();
                }

            }

            //Check for hit Messeage on ev3 ONLY WHEN CONNECTED
            if (ev3Messenger.IsConnected)
            {
                EV3Message message = ev3Messenger.ReadMessage();
                if (message != null && message.MailboxTitle == "hitBox")
                {
                    checkCollision();
                }
            }

            //Calculate when to spawn new Target
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            spawnTimer -= elapsed;

            if (spawnTimer < 0)
            {
                Random rnd = new Random();

                Target t= new Target(new Vector2(-250, 150), rnd.Next(0, 3));
                Target t2 = new Target(new Vector2(-250, 500), rnd.Next(0, 3));

                t2.LoadContent();
                t.LoadContent();

                targets.Add(t);
                targets.Add(t2);

                spawnTimer = spawnTIMER;
            }
            
            //Run update method for each target in targets list
            foreach (Target target in targets)
            {
                
               target.Update(gameTime);
            }

            //Remove targets from list if obsolete = true
            targets.RemoveAll(target => target.obsolete == true);
            //targets.RemoveAll(target => target.hit == true);

            base.Update(gameTime);
        }

        public void checkCollision ()
        {
            foreach (Target target in targets)
            {
                if (target.hit == false)
                {
                    if (target.boundingBox.Intersects(cursorBoundingBox))
                    {
                        target.hit = true;

                        Random rdn = new Random();
                        hitSoundEffects[rdn.Next(0, 3)].Play();

                        switch (target.value)
                        {
                            case 0:
                                score += 100;
                                break;
                            case 1:
                                score += 500;
                                break;
                            case 2:
                                score += 1000;
                                break;
                            default:
                                score += 0;
                                break;
                        }

                        Console.WriteLine(score);

                        if (ev3Messenger.IsConnected)
                        {
                            ev3Messenger.SendMessage("scoreBox", score);
                        }
                    }
                    else
                    {
                       
                    }
                }

            }

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            //Draw the background
            spriteBatch.Draw(background, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

            //Draw the targets
            foreach (Target target in targets)
            {
                target.Draw(spriteBatch);
            }

            //Draw the Shelves
            spriteBatch.Draw(shelf, new Rectangle(0, 345, GraphicsDevice.Viewport.Width, 50), Color.White);
            spriteBatch.Draw(shelf, new Rectangle(0, 695, GraphicsDevice.Viewport.Width, 50), Color.White);

            //Draw Prescreen
            if (preScreenPosition.Y != -900)
            {
                spriteBatch.Draw(preScreen, preScreenPosition, Color.White);
            }
            

            //Draw Curtains
            spriteBatch.Draw(curtainL, new Rectangle(-30, 0, 200, GraphicsDevice.Viewport.Height), Color.White);
            spriteBatch.Draw(curtainR, new Rectangle(GraphicsDevice.Viewport.Width - 170, 0, 200, GraphicsDevice.Viewport.Height), Color.White);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null);
            spriteBatch.Draw(roof,new Rectangle(0, 0, GraphicsDevice.Viewport.Width, 94), new Rectangle(0 - 20, 0, GraphicsDevice.Viewport.Width + 20, 92), Color.White);
            spriteBatch.End();

            spriteBatch.Begin();
                //Draw the cursor
            spriteBatch.Draw(cursor, cursorPosition, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
