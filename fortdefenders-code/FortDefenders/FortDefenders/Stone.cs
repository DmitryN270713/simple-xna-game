using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FortDefenders
{
    public sealed class Stone : ResourceBase
    {
        public const Int16 STONE_WIDTH = 1;
        public const Int16 STONE_HEIGHT = 1;
        public const Int16 START_STONE = 20;
        private const Int16 NEEDED_WORKERS = 1;

        private const Int16 MAX_AMOUNT = 100;
        private const Int16 AMOUNT_AT_TIME = 2;
        private const Single DELAY_EXTRACTION = 1.0f;

        public Stone(Game game) : base(MAX_AMOUNT, AMOUNT_AT_TIME, DELAY_EXTRACTION, game, CellState.Stone)
        { 
        
        }

        public override void LoadTextures()
        {
            this.beginTex = this.game.Content.Load<Texture2D>("Textures/resources/stone");
            this.endTex = this.game.Content.Load<Texture2D>("Textures/resources/stone_end");

            base.LoadTextures();
        }

        public sealed override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public sealed override void Draw()
        {
            base.Draw();
        }

        public override short GetNeededWorkers()
        {
            return NEEDED_WORKERS;
        }
    }
}
