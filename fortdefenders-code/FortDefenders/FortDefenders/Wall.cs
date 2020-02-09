using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FortDefenders
{
    public sealed class Wall : BuildingBase
    {
        private const Single NEXT_STAGE_TIME = 5.0f;     //5 seconds for testing;
        private const Int16 MAX_NUMBER_OF_BUILDINGS = 0;    //Means as many as needed
        public const Int16 GOLD = 0;
        public const Int16 WOOD = 10;
        public const Int16 STONE = 10;
        public const Int16 NEEDS_HUMANS_TO_BUILD = 0;      //Doesn't need any to build


        public Wall(Game game) : base(NEXT_STAGE_TIME, game, CellState.Wall, 100, BuildingState.Ready) 
        {
           
        }

        public sealed override void LoadTextures()
        {
            this.beginTex = this.game.Content.Load<Texture2D>("Textures/wall/wall_begin");
            this.halfTex = this.game.Content.Load<Texture2D>("Textures/wall/wall_halfway");
            this.readyTex = this.game.Content.Load<Texture2D>("Textures/wall/wall_ready");
            this.SwitchStateNormal();

            WallMenu wallMenu = null;
            this.game.RegisterGameComponent<WallMenu>(out wallMenu, false, false);
            this.menu = wallMenu;
            this.menu.SetParent(this);

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

        public override Int16 GetMaxNumberOfBuildings()
        {
            return MAX_NUMBER_OF_BUILDINGS;
        }

        public sealed override short GetWorkersNeeded()
        {
            return NEEDS_HUMANS_TO_BUILD;
        }
    }
}
