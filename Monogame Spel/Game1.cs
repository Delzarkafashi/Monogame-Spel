using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Monogame_Spel
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;

        private Texture2D go1;
        private Texture2D hoppa1;
        private Texture2D ducka1;
        private Texture2D flygandefinde1;
        private Texture2D currentTexture;

        private Texture2D backgroundTexture;
        private ParallaxTexture layer1;

        private SpriteFont font;

        private List<Vector2> enemys;
        private int enmysTimer = 120;
        private Random rnd;

        private Vector2 position;
        private Vector2 Speed;

        private bool isJumping;
        private bool isDuckar;
        private bool hit;
        private bool isPlaying;
        private double score = 0;

        private const int STARTY = 280; //gubbens statrt position

        private List<Block> blocks;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            position = new Vector2(200, STARTY);
            enemys = new List<Vector2>();
            rnd = new Random();
            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            go1 = Content.Load<Texture2D>("go1");
            hoppa1 = Content.Load<Texture2D>("hoppa");
            ducka1 = Content.Load<Texture2D>("ducka");
            flygandefinde1 = Content.Load<Texture2D>("flygandefinde");
            backgroundTexture = Content.Load<Texture2D>("bakgrund bild");

            //layer1 = new ParallaxTexture(backgroundTexture);

            font = Content.Load<SpriteFont>("font");

            blocks = new List<Block>();
            Texture2D blockTexture = Content.Load<Texture2D>("block_texture");

            // Skapa några block och lägg till dem i listan
            // Skapar ett block med storleken 50x50
            // 330 vänster höger--- 285 upp och ner
            blocks.Add(new Block(new Vector2(330, 285), blockTexture, new Vector2(50, 50))); 
                                                                                             
            blocks.Add(new Block(new Vector2(440, 270), blockTexture, new Vector2(60, 30))); 

            // Lägg till fler block efter behov


            currentTexture = go1; // Initial texture
        }

        //om player är på en block
        private bool IsPlayerOnBlock()
        {
            Rectangle playerRect = new Rectangle((int)position.X, (int)position.Y, currentTexture.Width, currentTexture.Height);
            foreach (var block in blocks)
            {
                Rectangle blockRect = new Rectangle((int)block.Position.X, (int)block.Position.Y, (int)block.Size.X, (int)block.Size.Y);
                // Om spelaren står på ett block
                if (playerRect.Bottom >= blockRect.Top && playerRect.Bottom <= blockRect.Top + 5 && playerRect.Intersects(blockRect))
                {
                    return true;
                }
            }
            return false;
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Enter) && !isPlaying)
            {
                Reset();
                isPlaying = true;
            }

            if (!isPlaying)
                return;

            score += gameTime.ElapsedGameTime.TotalSeconds;

            // Rörelse åt vänster och höger
            if (state.IsKeyDown(Keys.A))
            {
                position.X -= 3f; // Justera rörelsehastigheten som du vill ha
            }
            else if (state.IsKeyDown(Keys.D))
            {
                position.X += 3f; // Justera rörelsehastigheten som du vill ha
            }

            position += Speed;

            // Gränser för att hålla spelaren inom skärmen
            position.X = MathHelper.Clamp(position.X, 0, GraphicsDevice.Viewport.Width - currentTexture.Width);
            position.Y = MathHelper.Clamp(position.Y, 0, GraphicsDevice.Viewport.Height - currentTexture.Height);

            bool isPlayerOnGround = position.Y >= STARTY;

            if (position.Y > STARTY)
            {
                position = new Vector2(position.X, STARTY);
                Speed = Vector2.Zero;
                isJumping = false;
            }
            Speed += new Vector2(0, 0.2f);

            if (state.IsKeyDown(Keys.W) && !isJumping)
            {
                Speed = new Vector2(0, -5.0f);
                isJumping = true;
            }
            if (state.IsKeyDown(Keys.S) && !isJumping)
            {
                isDuckar = true;
                currentTexture = ducka1;
                position.Y = 338 - ducka1.Height;
            }
            else
            {
                isDuckar = false;
                currentTexture = go1;
            }

            enmysTimer--;
            if (enmysTimer <= 0)
            {
                enmysTimer = 120;

                if (rnd.Next(2) == 0)
                {
                    enemys.Add(new Vector2(800, STARTY));
                }
                else
                { 
                    enemys.Add(new Vector2(800, STARTY + 35));
                }
            }

            for (int i = 0; i < enemys.Count; i++)
            {
                enemys[i] = enemys[i] + new Vector2(-2, 0);
            }

            // Collision detection
            Rectangle playerBox = new Rectangle((int)position.X, (int)position.Y,
                currentTexture.Width, currentTexture.Height);
            hit = false;
            foreach (var enemy in enemys)
            {
                Rectangle enemyBox = new Rectangle((int)enemy.X, (int)enemy.Y,
                    flygandefinde1.Width, flygandefinde1.Height);

                var kollision = Rectangle.Intersect(playerBox, enemyBox);

                if (kollision.Width > 0 && kollision.Height > 0)
                {
                    Rectangle r1 = Normalize(playerBox, kollision);
                    Rectangle r2 = Normalize(enemyBox, kollision);
                    hit = TestCollision(currentTexture, r1, flygandefinde1, r2);
                    if (hit)
                    {
                        isPlaying = false; break;
                    }
                }
            }

            foreach (var block in blocks)
            {
                Rectangle playerRect = new Rectangle((int)position.X, (int)position.Y, currentTexture.Width, currentTexture.Height);

                // Använd storleken från blocket istället för att hårdkoda 50x50
                Rectangle blockRect = new Rectangle((int)block.Position.X, (int)block.Position.Y, (int)block.Size.X, (int)block.Size.Y);

                // Om det finns en kollision mellan spelaren och blocket
                if (playerRect.Intersects(blockRect))
                {
                    // Beräkna hur mycket spelaren överlappar med blocket i varje riktning
                    int overlapX = Math.Min(playerRect.Right, blockRect.Right) - Math.Max(playerRect.Left, blockRect.Left);
                    int overlapY = Math.Min(playerRect.Bottom, blockRect.Bottom) - Math.Max(playerRect.Top, blockRect.Top);

                    // Justera spelarens position för att undvika kollision
                    if (overlapX > overlapY)
                    {
                        // Kollision längs Y-axeln
                        if (playerRect.Top < blockRect.Top)
                            position.Y -= overlapY;
                        else
                            position.Y += overlapY;
                    }
                    else
                    {
                        // Kollision längs X-axeln
                        if (playerRect.Left < blockRect.Left)
                            position.X -= overlapX;
                        else
                            position.X += overlapX;
                    }
                }
            }


            base.Update(gameTime);
        }

        //beräknar skärmens storlek för bakgrunds bilden
        private Vector2 CalculateBackgroundScale(Texture2D background)
        {
            float screenWidth = GraphicsDevice.Viewport.Width;
            float screenHeight = GraphicsDevice.Viewport.Height;
            float scaleX = screenWidth / background.Width;
            float scaleY = screenHeight / background.Height;

            return new Vector2(scaleX, scaleY);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Vector2 backgroundScale = CalculateBackgroundScale(backgroundTexture);

            spriteBatch.Begin();
            // Använd den beräknade skalningsfaktorn när du ritar bakgrundsbilden
            spriteBatch.Draw(backgroundTexture, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, backgroundScale, SpriteEffects.None, 0f);

            if (isPlaying)
            {
                spriteBatch.DrawString(font, ((int)score).ToString(), new Vector2(110, 120), Color.Black);
                if (hit)
                {
                    spriteBatch.DrawString(font, "HIT!!", new Vector2(10, 40), Color.White);
                }
                spriteBatch.Draw(currentTexture, position, Color.White);

                foreach (var enemy in enemys)
                {
                    spriteBatch.Draw(flygandefinde1, enemy, Color.White);
                }

                // Rita ut blocken bara om spelet är igång
                foreach (var block in blocks)
                {
                    block.Draw(spriteBatch);
                }
            }
            else
            {
                spriteBatch.DrawString(font, "Tryck enter om du vill spela igen!", new Vector2(350, 200), Color.Black);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }



        private void Reset()
        {
            enemys.Clear();
            enmysTimer = 120;
            score = 0;
        }
        public static Rectangle Normalize(Rectangle refernce, Rectangle overlap)
        {
            return new Rectangle(
                overlap.X - refernce.X,
                overlap.Y - refernce.Y,
                overlap.Width,
                overlap.Height);
        }

        public static bool TestCollision(Texture2D t1, Rectangle r1, Texture2D t2, Rectangle r2)
        {
            int pixelCount = r1.Width * r1.Height;
            Color[] texture1Pixels = new Color[pixelCount];
            Color[] texture2Pixels = new Color[pixelCount];

            t1.GetData(0, r1, texture1Pixels, 0, pixelCount);
            t2.GetData(0, r2, texture2Pixels, 0, pixelCount);

            for (int i = 0; i < pixelCount; i++)
            {
                if (texture1Pixels[i].A > 0 && texture2Pixels[i].A > 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
