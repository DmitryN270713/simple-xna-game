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
    public sealed class Bank : BuildingBase
    {
        public Int16 AMOUNT_AT_TIME = 1;
        private Single currentDelayBeforeExtraction;

        private const Single NEXT_STAGE_TIME = 1.0f;     //5 seconds for testing;
        private const Int16 MAX_NUMBER_OF_BUILDINGS = 1;
        public const Int16 MAX_NUMBER_OF_PEOPLE_INSIDE = 2;
        public const Int16 WIDTH = 7;
        public const Int16 HEIGHT = 7;
        public const Int16 GOLD = 1;
        public const Int16 WOOD = 1;
        public const Int16 STONE = 1;
        public const Int16 NEEDS_HUMANS_TO_BUILD = 2;

        //Number of bankers inside
        public Int16 BankersCounter { get; set; }


        public Bank(Game game) : base(NEXT_STAGE_TIME, game, CellState.Bank, 0, BuildingState.Begin)
        {
        }

        /// <summary>
        /// Must be implemented. Otherwise there are no textures available to draw them on the plane
        /// </summary>
        public sealed override void LoadTextures()
        {
            this.beginTex = this.game.Content.Load<Texture2D>("Textures/Bank/BankTest");
            this.halfTex = this.game.Content.Load<Texture2D>("Textures/house_halfway");
            this.readyTex = this.game.Content.Load<Texture2D>("Textures/Bank/BankFinished");
            this.curTexture = this.beginTex;

            //In your own class do the same thing, as it's done below. Use the same order, otherwise some funny bugs shall appear
            BankMenu bankMenu = null;
            this.game.RegisterGameComponent<BankMenu>(out bankMenu, false, false);
            this.menu = bankMenu;
            this.menu.SetParent(this);
            bankMenu.ConvertParent();

            base.LoadTextures();
        }

        public sealed override void Update(GameTime gameTime)
        {
            this.GenerateGold(gameTime);
            base.Update(gameTime);
        }

        private void GenerateGold(GameTime gameTime)
        {
            this.currentDelayBeforeExtraction -= (gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
            if (this.currentDelayBeforeExtraction <= 0)
            {
                this.currentDelayBeforeExtraction = 1;
                if (currentHealth >= MAX_HEALTH)
                {
                    //This is only an example, how the number of workers could be used. 
                    //Feel free to suggest and implement your own method.
                    this.resourcesHUD.SetCurrentResources(0, 0, Convert.ToInt16(3 * AMOUNT_AT_TIME * this.BankersCounter), 0);
                }
            }
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
