using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FortDefenders
{
    public sealed class HouseMenu : ConstructionMenu
    {
        public HouseMenu(Game game) : base(game)
        { 
            
        }
        
        protected sealed override void LoadContent()
        {
            this.backgroundTexture = this.Game.Content.Load<Texture2D>("Textures/house_menu_background");
            this.constructionName = "House";
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
