using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace _2DCollision
{
    
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        
        // The images to draw
        Texture2D personTexture;
        Texture2D blockTexture;
        SpriteFont font;
        

        // The images will be drawn with this SpriteBatch
        SpriteBatch spriteBatch;

        // Person
        Vector2 personPosition = new Vector2 (0,0);
        const int PersonMoveSpeed = 5;
        // Blocks
        List<Vector2> blockPositions = new List<Vector2>();
        float BlockSpawnProbability = 0.01f;
        const int BlockFallSpeed = 2;
        Random random = new Random();

        // For when a collision is detected
        bool personHit = false;
        bool previousPersonHit = false;
        int hitingBlocks = 0;
        int allBlocks = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        
        protected override void Initialize()
        {
            

            base.Initialize();

            // Start the player in the center along the bottom of the screen
            personPosition.X = (Window.ClientBounds.Width - personTexture.Width) / 2;
            personPosition.Y = Window.ClientBounds.Height - personTexture.Height;
        }


        protected override void LoadContent()
        {
            // Load textures
            blockTexture = Content.Load<Texture2D>("block");
            personTexture = Content.Load<Texture2D>("man");
            font = Content.Load<SpriteFont>("MyFont");

            // Create a sprite batch to draw those textures
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            
        }

       
        protected override void UnloadContent()
        {
            
        }

       
        protected override void Update(GameTime gameTime)
        {
            // Get input
            KeyboardState keyboard = Keyboard.GetState();
            GamePadState gamePad = GamePad.GetState(PlayerIndex.One);
            // Allows the game to exit
            if (gamePad.Buttons.Back == ButtonState.Pressed)
                this.Exit();
            // Move the player left and right with arrow keys or d-pad
            if (keyboard.IsKeyDown(Keys.Left) || gamePad.DPad.Left == ButtonState.Pressed)
            {
                personPosition.X -= PersonMoveSpeed;
            }
            if (keyboard.IsKeyDown(Keys.Right) || gamePad.DPad.Right == ButtonState.Pressed)
            {
                personPosition.X += PersonMoveSpeed;
            }
            // Prevent the person from moving off of the screen
            personPosition.X = MathHelper.Clamp(personPosition.X,0, Window.ClientBounds.Width - personTexture.Width);

            // Spawn new falling blocks
            if (random.NextDouble() < BlockSpawnProbability)
            {
                float x = (float)random.NextDouble() *
                    (Window.ClientBounds.Width - blockTexture.Width);
                blockPositions.Add(new Vector2(x, -blockTexture.Height));
            }
            // Get the bounding rectangle of the person
            Rectangle personRectangle =
            new Rectangle((int)personPosition.X, (int)personPosition.Y,
            personTexture.Width, personTexture.Height);
            // Update each block
            personHit = false;            
            for (int i = 0; i < blockPositions.Count; i++)
            {
                // Animate this block falling
                blockPositions[i] =
                new Vector2(blockPositions[i].X,
                blockPositions[i].Y + BlockFallSpeed);
                // Get the bounding rectangle of this block
                Rectangle blockRectangle =
                new Rectangle((int)blockPositions[i].X, (int)blockPositions[i].Y,
                blockTexture.Width, blockTexture.Height);
                // Check collision with person
                if (personRectangle.Intersects(blockRectangle))
                {
                    personHit = true;
                    previousPersonHit = personHit;
                }
                

                // Remove this block if it have fallen off the screen
                if (blockPositions[i].Y > Window.ClientBounds.Height)
                {
                    allBlocks += 1;
                    if (previousPersonHit) { hitingBlocks += 1; previousPersonHit = false; }
                    blockPositions.RemoveAt(i);
                    
                    // When removing a block, the next block will have the same index
                    // as the current block. Decrement i to prevent skipping a block.
                    i--;
                    
                }                          
            }
            
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice device = graphics.GraphicsDevice;
            // Change the background to red when the person was hit by a block
            if (personHit)
                device.Clear(Color.Red);
            else
                device.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            //Draw table
            spriteBatch.DrawString(font, String.Format("DODGED BLOCKS  {0} : {1} ALL BLOCKS", (allBlocks-hitingBlocks).ToString(), allBlocks.ToString()), new Vector2(0, 0), Color.Black);
            
            // Draw person
            spriteBatch.Draw(personTexture, personPosition, Color.White);
            // Draw blocks
            foreach (Vector2 blockPosition in blockPositions)
                spriteBatch.Draw(blockTexture, blockPosition, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
