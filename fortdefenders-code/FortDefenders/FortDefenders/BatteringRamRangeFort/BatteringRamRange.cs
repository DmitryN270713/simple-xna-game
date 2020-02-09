using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FortDefenders.BatteringRamRangeFort
{
    public sealed class BatteringRamRange : BuildingBase
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
        private BatteringRamRangeMenu batteringRamRangeMenu;
        //Battering ram object for communication purposes
        private BatteringRam batteringRamObject;
        //Number of ready squads
        public Int16 BatteringRamsCounter
        { get; set; }
        //Number of units to be taught
        public Int16 UnitsToBeTaught { get; set; }
        //Progress in teaching
        public Single Progress { get; set; }



        public BatteringRamRange(Game game) : base(NEXT_STAGE_TIME, game, CellState.BatteringRamRange, 0, BuildingState.Begin)
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
            this.beginTex = this.game.Content.Load<Texture2D>("Textures/BatteringRam/battering_ram_begin");
            this.halfTex = this.game.Content.Load<Texture2D>("Textures/BatteringRam/battering_ram_halfway");
            this.readyTex = this.game.Content.Load<Texture2D>("Textures/BatteringRam/battering_ram_ready");
            this.curTexture = this.beginTex;

            //In your own class do the same thing, as it's done below. Use the same order, otherwise some funny bugs shall appear
            this.batteringRamRangeMenu = null;
            this.game.RegisterGameComponent<BatteringRamRangeMenu>(out this.batteringRamRangeMenu, false, false);
            this.menu = this.batteringRamRangeMenu;
            this.menu.SetParent(this);
            this.batteringRamRangeMenu.ConvertParent();
            this.humansParent.GetListOfPersons(ref this.listOfPersons);
            
            this.batteringRamObject = (from target in this.listOfPersons
                                 where target.Role == CellState.BatteringRam
                                 select target).ElementAt(0) as BatteringRam;
            this.batteringRamObject.BatteringRamRangeBuilt = true;
            this.batteringRamObject.BatteringRamRangeObject = this;
            this.batteringRamRangeMenu.SetBatteringRamObject(ref this.batteringRamObject);

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
                    this.BatteringRamsCounter += BatteringRamRangeMenu.ADD_BATTERING_RAMS / BatteringRamRangeMenu.ADD_BATTERING_RAMS;
                    this.batteringRamObject.SetUnitsCounter(BatteringRamRangeMenu.ADD_BATTERING_RAMS / BatteringRamRangeMenu.ADD_BATTERING_RAMS);
                    this.UnitsToBeTaught -= BatteringRamRangeMenu.ADD_BATTERING_RAMS;
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

        public void DecreaseBatteringRamsCounters()
        {
            this.batteringRamRangeMenu.DecreaseBatteringRamsCounters();
        }

        #endregion
    }
}
