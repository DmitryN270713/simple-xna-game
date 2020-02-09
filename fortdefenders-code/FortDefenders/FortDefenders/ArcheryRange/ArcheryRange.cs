using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FortDefenders.ArcheryRange
{
    public sealed class ArcheryRange : BuildingBase
    {
        private const Single DELAY_BEFORE_READY = 5.0f;

        private Single currentDelayBeforeExtraction;

        private const Single NEXT_STAGE_TIME = 1.0f;     //1 second for testing;
        private const Int16 MAX_NUMBER_OF_BUILDINGS = 1;
        public const Int16 WIDTH = 3;
        public const Int16 HEIGHT = 3;
        public const Int16 GOLD = 1;
        public const Int16 WOOD = 1;
        public const Int16 STONE = 1;
        public const Int16 NEEDS_HUMANS_TO_BUILD = 2;

        //List of persons for communication purposes
        private List<HumansBase> listOfPersons;
        private HumansParent humansParent;
        //Menu
        private ArcheryRangeMenu archeryMenu;
        //Archer object for communication purposes
        private Archer archerObject;
        //Number of ready soldiers
        public Int16 ArchersCounter
        { get; set; }
        //Number of units to be taught
        public Int16 UnitsToBeTaught { get; set; }
        //Progress in teaching
        public Single Progress { get; set; }



        public ArcheryRange(Game game) : base(NEXT_STAGE_TIME, game, CellState.ArcheryRange, 0, BuildingState.Begin)
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
            this.beginTex = this.game.Content.Load<Texture2D>("Textures/ArcheryRange/archery_begin");
            this.halfTex = this.game.Content.Load<Texture2D>("Textures/ArcheryRange/archery_halfway");
            this.readyTex = this.game.Content.Load<Texture2D>("Textures/ArcheryRange/archery_ready");
            this.curTexture = this.beginTex;

            //In your own class do the same thing, as it's done below. Use the same order, otherwise some funny bugs shall appear
            this.archeryMenu = null;
            this.game.RegisterGameComponent<ArcheryRangeMenu>(out this.archeryMenu, false, false);
            this.menu = this.archeryMenu;
            this.menu.SetParent(this);
            this.archeryMenu.ConvertParent();
            this.humansParent.GetListOfPersons(ref this.listOfPersons);
            
            this.archerObject = (from target in this.listOfPersons
                                 where target.Role == CellState.Archer
                                 select target).ElementAt(0) as Archer;
            this.archerObject.ArcheryRangeBuilt = true;
            this.archerObject.ArcheryRangeObject = this;
            this.archeryMenu.SetArcherObject(ref this.archerObject);

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
                    this.ArchersCounter += ArcheryRangeMenu.ADD_ARCHERS;
                    this.archerObject.SetUnitsCounter(ArcheryRangeMenu.ADD_ARCHERS);
                    this.UnitsToBeTaught -= ArcheryRangeMenu.ADD_ARCHERS;
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

        public void DecreaseArchersCounters()
        {
            this.archeryMenu.DecreaseArchersCounters();
        }

        #endregion
    }
}
