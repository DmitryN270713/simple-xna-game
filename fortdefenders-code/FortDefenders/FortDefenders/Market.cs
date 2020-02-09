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
    public sealed class Market : BuildingBase
    {


        private const Single NEXT_STAGE_TIME = 5.0f;     //5 seconds for testing;
        private const Int16 MAX_NUMBER_OF_BUILDINGS = 1;
        public const Int16 MAX_NUMBER_OF_PEOPLE_INSIDE = 4;
        public const Int16 WIDTH = 3;
        public const Int16 HEIGHT = 3;
        public const Int16 GOLD = 1;
        public const Int16 WOOD = 1;
        public const Int16 STONE = 1;
        public const Int16 FOOD = 1;
        public const Int16 NEEDS_HUMANS_TO_BUILD = 2;

        //Number of sellers inside
        public Int16 SellersCounter { get; set; }

        public Market(Game game) : base(NEXT_STAGE_TIME, game, CellState.Marketplace, 0, BuildingState.Begin)
        {

        }

        /// <summary>
        /// Must be implemented. Otherwise there are no textures available to draw them on the plane
        /// </summary>
        public sealed override void LoadTextures()
        {
            this.beginTex = this.game.Content.Load<Texture2D>("Textures/Marketplace/MarketTest");
            this.halfTex = this.game.Content.Load<Texture2D>("Textures/house_halfway");
            this.readyTex = this.game.Content.Load<Texture2D>("Textures/house_ready");
            this.curTexture = this.beginTex;

            //In your own class do the same thing, as it's done below. Use the same order, otherwise some funny bugs shall appear
            MarketMenu marketMenu = null;
            this.game.RegisterGameComponent<MarketMenu>(out marketMenu, false, false);
            this.menu = marketMenu;
            this.menu.SetParent(this);
            marketMenu.ConvertParent();

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

        public sealed override Int16 GetMaxNumberOfBuildings()
        {
            return MAX_NUMBER_OF_BUILDINGS;
        }

        public Int16 WoodBought()
        {
            if (currentHealth >= MAX_HEALTH)
            {
                this.resourcesHUD.SetCurrentResources(0, Convert.ToInt16(50 * SellersCounter/MAX_NUMBER_OF_PEOPLE_INSIDE), Convert.ToInt16(-100 * SellersCounter/MAX_NUMBER_OF_PEOPLE_INSIDE), 0);
                return WOOD;
            }
            return WOOD;
        }
        public Int16 StoneBought()
        {
            if (currentHealth >= MAX_HEALTH)
            {
                this.resourcesHUD.SetCurrentResources(50, 0, -100, 0);
                return STONE;
            }
            return STONE;
        }
        public Int16 FoodBought()
        {
            if (currentHealth >= MAX_HEALTH)
            {
                this.resourcesHUD.SetCurrentResources(0, 0, -100, 50);
                return FOOD;
            }
            return FOOD;
        }

        public Int16 FoodSold()
        {
            if (currentHealth >= MAX_HEALTH)
            {
                this.resourcesHUD.SetCurrentResources(0, 0, 50, -100);
                return FOOD;
            }
            return FOOD;
        }
        public Int16 WoodSold()
        {
            if (currentHealth >= MAX_HEALTH)
            {
                this.resourcesHUD.SetCurrentResources(0, -100, 50, 0);
                return WOOD;
            }
            return WOOD;
        }
        public Int16 StoneSold()
        {
            if (currentHealth >= MAX_HEALTH)
            {
                this.resourcesHUD.SetCurrentResources(-100, 0, 50, 0);
                return STONE;
            }
            return STONE;
        }

        public sealed override Int16 GetWorkersNeeded()
        {
            return NEEDS_HUMANS_TO_BUILD;
        }
    }
}

