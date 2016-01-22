using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProftaakGame
{
    class Target
    {

        public Texture2D texture;
        public Texture2D textureBroken;
        Texture2D dummyTexture;

        Vector2 position;
        Vector2 velocity;
        public int value;
        public Rectangle boundingBox;

        public bool obsolete = false;
        public bool hit = false;

        public Target(Vector2 newPosition, int newValue)
        {
            position = newPosition;
            velocity = new Vector2(2, 0);
            value = newValue;
        }

        public void LoadContent()
        {
            switch (value)
            {
                case 0:
                    texture = ScreenManager.Instance.Content.Load<Texture2D>("plate");
                    textureBroken = ScreenManager.Instance.Content.Load<Texture2D>("Sounds/plateBroken");
                    break;
                case 1:
                    texture = ScreenManager.Instance.Content.Load<Texture2D>("plate1");
                    textureBroken = ScreenManager.Instance.Content.Load<Texture2D>("Sounds/plate1Broken");
                    break;
                case 2:
                    texture = ScreenManager.Instance.Content.Load<Texture2D>("plate2");
                    textureBroken = ScreenManager.Instance.Content.Load<Texture2D>("Sounds/plate2Broken");
                    break;
                default:
                    texture = ScreenManager.Instance.Content.Load<Texture2D>("plate");
                    textureBroken = ScreenManager.Instance.Content.Load<Texture2D>("Sounds/plateBroken");
                    break;
            }

            //boundingBox = new Rectangle((int)position.X, (int)position.Y, texture.Width/2, texture.Height/2);

            dummyTexture = ScreenManager.Instance.Content.Load<Texture2D>("test");
        }

        public void Update(GameTime gameTime)
        {
            position += velocity;

            if (position.X > ScreenManager.Instance.Graphics.Viewport.Width)
            {
                this.obsolete = true;
            }

            boundingBox = new Rectangle((int)position.X + 50, (int)position.Y + 45, (texture.Width/2)+30, (texture.Height/2)+30);
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            if (hit)
            {
                spriteBatch.Draw(textureBroken, position, Color.White);
            }
            else
            {
                spriteBatch.Draw(texture, position, Color.White);
            }
            
        }
    }
}
