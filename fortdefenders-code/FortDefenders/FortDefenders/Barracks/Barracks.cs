using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FortDefenders.Barracks
{
    public sealed class Barracks : BuildingBase
    {
        private const Single DELAY_BEFORE_READY = 5.0f;

        private Single currentDelayBeforeExtraction;

        private const Single NEXT_STAGE_TIME = 1.0f;     //1 second for testing;
        private const Int16 MAX_NUMBER_OF_BUILDINGS = 1;
        public const Int16 WIDTH = 7;
        public const Int16 HEIGHT = 7;
        public const Int16 GOLD = 1;
        public const Int16 WOOD = 1;
        public const Int16 STONE = 1;
        public const Int16 NEEDS_HUMANS_TO_BUILD = 2;

        //List of persons for communication purposes
        private List<HumansBase> listOfPersons;
        private HumansParent humansParent;
        //Menu
        private BarracksMenu barracksMenu;
        //Knight object for communication purposes
        private Knight knightObject;
        //Number of ready soldiers
        public Int16 SwordsmenCounter
        { get; set; }
        //Number of units to be taught
        public Int16 UnitsToBeTaught { get; set; }
        //Progress in teaching
        public Single Progress { get; set; }



        public Barracks(Game game) : base(NEXT_STAGE_TIME, game, CellState.Barracks, 0, BuildingState.Begin)
        {
            this.currentDelayBeforeExtraction = DELAY_BEFORE_READY;
            this.humansParent = game.Components.OfType<HumansParent>().ElementAt(0);
            this.Progress = 0;
        }

        /// <summary>
        /// Must be implemented. Otherwise there are no textures available to draw them on the plane
        /// </summary>
        public sealed override void LoadTextures()
        {
            this.beginTex = this.game.Content.Load<Texture2D>("Textures/Barracks/barracks_begin");
            this.halfTex = this.game.Content.Load<Texture2D>("Textures/Barracks/barracks_halfway");
            this.readyTex = this.game.Content.Load<Texture2D>("Textures/Barracks/BarracksFinished");
            this.curTexture = this.beginTex;

            //In your own class do the same thing, as it's done below. Use the same order, otherwise some funny bugs shall appear
            this.barracksMenu = null;
            this.game.RegisterGameComponent<BarracksMenu>(out this.barracksMenu, false, false);
            this.menu = this.barracksMenu;
            this.menu.SetParent(this);
            this.barracksMenu.ConvertParent();
            this.humansParent.GetListOfPersons(ref this.listOfPersons);
            
            this.knightObject = (from target in this.listOfPersons
                                 where target.Role == CellState.Knight
                                 select target).ElementAt(0) as Knight;
            this.knightObject.BarracksBuilt = true;
            this.knightObject.BarracksObject = this;
            this.barracksMenu.SetKnightObject(ref this.knightObject);

            base.LoadTextures();
        }

        public sealed override void Update(GameTime gameTime)
        {
            if (this.UnitsToBeTaught > 0)
            {
                this.GenerateSoldier(gameTime);
            }
            base.Update(gameTime);
        }

        private void GenerateSoldier(GameTime gameTime)
        {
            this.currentDelayBeforeExtraction -= (gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
            this.Progress = (1.0f - this.currentDelayBeforeExtraction / DELAY_BEFORE_READY) * 100.0f;
            if (this.currentDelayBeforeExtraction <= 0)
            {
                this.currentDelayBeforeExtraction = DELAY_BEFORE_READY;
                if (currentHealth >= MAX_HEALTH)
                {
                    this.SwordsmenCounter += BarracksMenu.ADD_SWORDSMEN;
                    this.knightObject.SetUnitsCounter(BarracksMenu.ADD_SWORDSMEN);
                    this.UnitsToBeTaught -= BarracksMenu.ADD_SWORDSMEN;
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

        public sealed override Int16 GetWorkersNeeded()
        {
            return NEEDS_HUMANS_TO_BUILD;
        }

        #region Own methods

        public void DecreaseSwordsmenCounters()
        {
            this.barracksMenu.DecreaseSwordsmenCounters();
        }

        #endregion
    }
}
