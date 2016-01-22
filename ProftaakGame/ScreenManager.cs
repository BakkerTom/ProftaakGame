using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProftaakGame
{
    public class ScreenManager
    {
        public ContentManager Content { get; private set; }
        private static ScreenManager instance;
        public GraphicsDevice Graphics { get; set; }

        public static ScreenManager Instance
        {
            get
            {
                if(instance==null)
                {
                    instance = new ScreenManager();
                }
                return instance;
            }
        }

        public void LoadContent(ContentManager content)
        {
            this.Content = new ContentManager(content.ServiceProvider, "Content");
        }


    }
}
