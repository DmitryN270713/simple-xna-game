using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FortDefenders
{
    public sealed class WallMenu : ConstructionMenu
    {
        public WallMenu(Game game) : base(game)
        { 
        
        }

        protected sealed override void LoadContent()
        {
            this.backgroundTexture = this.Game.Content.Load<Texture2D>("Textures/wall/wall_menu_background");
            this.constructionName = "Wall";
            base.LoadContent();
        }

        public sealed override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public sealed override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
