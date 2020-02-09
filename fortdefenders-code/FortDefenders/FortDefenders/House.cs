using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;


namespace FortDefenders
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public sealed class House : BuildingBase
    {
        private const Single NEXT_STAGE_TIME = 30.0f;     //5 seconds for testing;
        private const Int16 MAX_NUMBER_OF_BUILDINGS = 5;
        public const Int16 WIDTH = 2;
        public const Int16 HEIGHT = 3;
        public const Int16 GOLD = 1;
        public const Int16 WOOD = 1;
        public const Int16 STONE = 1;
        public const Int16 ADDED_POPULATION = 10;
        public const Int16 NEEDS_HUMANS_TO_BUILD = 2;

        private Boolean canCheckToAdd;

        public House(Game game) : base(NEXT_STAGE_TIME, game, CellState.House, 0, BuildingState.Begin)
        {
            this.canCheckToAdd = true;
        }

        /// <summary>
        /// Must be implemented. Otherwise there are no textures available to draw them on the plane
        /// </summary>
        public sealed override void LoadTextures()
        {
            this.beginTex = this.game.Content.Load<Texture2D>("Textures/house_begin");
            this.halfTex = this.game.Content.Load<Texture2D>("Textures/house_halfway");
            this.readyTex = this.game.Content.Load<Texture2D>("Textures/house_ready");
            this.curTexture = this.beginTex;

            //In your own class do the same thing, as it's done below. Use the same order, otherwise some funny bugs shall appear
            HouseMenu houseMenu = null;
            this.game.RegisterGameComponent<HouseMenu>(out houseMenu, false, false);
            this.menu = houseMenu;
            this.menu.SetParent(this);

            base.LoadTextures();
        }

        public sealed override void Update(GameTime gameTime)
        {
            if (canCheckToAdd)
            {
                if (this.currentHealth >= MAX_HEALTH)
                {
                    this.resourcesHUD.SetCurrentMaxPopulation();
                    this.canCheckToAdd = false;
                }
            }

            base.Update(gameTime);
        }

        public sealed override void Draw()
        {
            base.Draw();
        }

        public sealed override Int16 GetMaxNumberOfBuildings()
        {
            return MAX_NUMBER_OF_BUILDINGS;
        }

        public sealed override short GetWorkersNeeded()
        {
            return NEEDS_HUMANS_TO_BUILD;
        }
    }
}
