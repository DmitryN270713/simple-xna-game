using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FortDefenders.PersonsMenus;
using Microsoft.Xna.Framework;
using FortDefenders.PersonHelperClasses;
using Microsoft.Xna.Framework.Graphics;
using FortDefenders.BatteringRamRangeFort;

namespace FortDefenders
{
    public sealed class BatteringRam : HumansBase
    {
        public const Int16 START_X = 32;
        public const Int16 START_Z = 35;
        private const Single MOVING_SPEED = 0.5f;       //A half meters per second
        private const String PARENT_DIR = "Textures/BatteringRamPerson";
        private const Single STEP_TIME = 0.5f;
        private BatteringRamMenu batteringRamMenu;

        public Boolean BatteringRamRangeBuilt { get; set; }
        public BatteringRamRange BatteringRamRangeObject { get; set; }

        public BatteringRam(Game game) : base(game, CellState.BatteringRam, MOVING_SPEED,
                                  new Point(START_X, START_Z), 0, STEP_TIME)
        {
            this.batteringRamMenu = null;
            this.BatteringRamRangeBuilt = false;
        }

        public override void LoadTextures()
        {
            PersonTexturesLoader.LoadTextures(PARENT_DIR, this.game.Content, out this.textures);
            //this.texture[key][index of item in value's array(array of textures)]
            this.leftLegTex = this.textures[0][(Int32)PersonMovement.left];
            this.rightLegTex = this.textures[0][(Int32)PersonMovement.right];
            this.stopTex = this.textures[0][(Int32)PersonMovement.stop];
            this.FaceTex = this.game.Content.Load<Texture2D>("Textures/BatteringRamPerson/face_tex");

            this.game.RegisterGameComponent<BatteringRamMenu>(out this.batteringRamMenu, false, false);
            this.menu = this.batteringRamMenu;
            this.menu.SetParent(this);

            this.batteringRamMenu.ConvertParent();

            base.LoadTextures();

            //this.mayorMenu.SetServicesComponents(ref this.pickingService, ref this.menusCaller);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw()
        {
            base.Draw();
        }

        #region Own methods

        public void DecreaseBatteringRamsCounter()
        {
            BatteringRamRangeObject.DecreaseBatteringRamsCounters();
        }

        public void SetUnitsCounter(Int16 unitsAmount)
        {
            this.batteringRamMenu.UnitsCounter += unitsAmount;
        }

        #endregion
    }
}
