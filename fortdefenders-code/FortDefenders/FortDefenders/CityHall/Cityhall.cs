using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FortDefenders.CityHall
{
    public class Cityhall : BuildingBase
    {
        private const Single DELAY_BEFORE_READY = 5.0f;  //Delay before the next worker will be generated
        private const Single NEXT_STAGE_TIME = 5.0f;     //5 seconds for testing;
        private const Int16 MAX_NUMBER_OF_BUILDINGS = 1;
        public const Int16 WIDTH = 8;
        public const Int16 HEIGHT = 8;
        public const Int16 GOLD = 5;
        public const Int16 WOOD = 5;
        public const Int16 STONE = 5;
        public const Int16 NEEDS_HUMANS_TO_BUILD = 4;

        //Number of units to be hired
        public Int16 UnitsToBeHired { get; set; }
        //Progress in hiring
        public Single Progress { get; set; }
        //Shows, could be started the hiring process or not
        public Boolean CanHire { get; private set; }

        private Single currentDelayBeforeExtraction;



        public Cityhall(Game game) : base(NEXT_STAGE_TIME, game, CellState.CityHall, 0, BuildingState.Begin)
        {
            this.currentDelayBeforeExtraction = DELAY_BEFORE_READY;
            this.Progress = 0;
            this.OnBuildingConstructed += new BuildingConstructed(Cityhall_OnBuildingConstructed);
            this.CanHire = false;
        }

        private void Cityhall_OnBuildingConstructed(Object o, CellState type)
        {
            this.CanHire = true;
        }

        /// <summary>
        /// Must be implemented. Otherwise there are no textures available to draw them on the plane
        /// </summary>
        public sealed override void LoadTextures()
        {
            this.beginTex = this.game.Content.Load<Texture2D>("Textures/Cityhall/cityhall_beginnig");
            this.halfTex = this.game.Content.Load<Texture2D>("Textures/Cityhall/cityhall_halfway");
            this.readyTex = this.game.Content.Load<Texture2D>("Textures/Cityhall/TownhallSquare");
            this.curTexture = this.beginTex;

            //In your own class do the same thing, as it's done below. Use the same order, otherwise some funny bugs shall appear
            CityhallMenu cityhallMenu = null;
            this.game.RegisterGameComponent<CityhallMenu>(out cityhallMenu, false, false);
            this.menu = cityhallMenu;
            this.menu.SetParent(this);
            cityhallMenu.ConvertParent();

            base.LoadTextures();
        }

        public sealed override void Update(GameTime gameTime)
        {
            if (this.UnitsToBeHired > 0 && currentHealth >= MAX_HEALTH)
            {
                this.GenerateWorkers(gameTime);
            }
            base.Update(gameTime);
        }

        private void GenerateWorkers(GameTime gameTime)
        {
            this.currentDelayBeforeExtraction -= (gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
            this.Progress = (1.0f - this.currentDelayBeforeExtraction / DELAY_BEFORE_READY) * 100.0f;
            if (this.currentDelayBeforeExtraction <= 0)
            {
                this.currentDelayBeforeExtraction = DELAY_BEFORE_READY;
                if (currentHealth >= MAX_HEALTH)
                {
                    this.resourcesHUD.SetCurrentPopulation(CityhallMenu.WORKERS_AT_TIME);
                    this.UnitsToBeHired -= CityhallMenu.WORKERS_AT_TIME;
                    this.Progress = 0.0f;
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
