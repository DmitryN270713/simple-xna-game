using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FortDefenders.PersonsMenus;
using Microsoft.Xna.Framework;
using FortDefenders.PersonHelperClasses;
using Microsoft.Xna.Framework.Graphics;

namespace FortDefenders
{
    public sealed class Archer : HumansBase
    {
        public const Int16 START_X = 33;
        public const Int16 START_Z = 33;
        private const Single MOVING_SPEED = 1.5f;       //A half and meters per second
        private const String PARENT_DIR = "Textures/Archer";
        private const Single STEP_TIME = 0.5f;
        private ArcherMenu archerMenu;

        public Boolean ArcheryRangeBuilt { get; set; }
        public ArcheryRange.ArcheryRange ArcheryRangeObject { get; set; }

        public Archer(Game game) : base(game, CellState.Archer, MOVING_SPEED,
                                  new Point(START_X, START_Z), 0, STEP_TIME)
        {
            this.archerMenu = null;
            this.ArcheryRangeBuilt = false;
        }

        public override void LoadTextures()
        {
            PersonTexturesLoader.LoadTextures(PARENT_DIR, this.game.Content, out this.textures);
            //this.texture[key][index of item in value's array(array of textures)]
            this.leftLegTex = this.textures[0][(Int32)PersonMovement.left];
            this.rightLegTex = this.textures[0][(Int32)PersonMovement.right];
            this.stopTex = this.textures[0][(Int32)PersonMovement.stop];
            this.FaceTex = this.game.Content.Load<Texture2D>("Textures/Archer/face_tex");

            this.game.RegisterGameComponent<ArcherMenu>(out this.archerMenu, false, false);
            this.menu = this.archerMenu;
            this.menu.SetParent(this);

            this.archerMenu.ConvertParent();

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

        public void DecreaseArchersCounter()
        {
            ArcheryRangeObject.DecreaseArchersCounters();
        }

        public void SetUnitsCounter(Int16 unitsAmount)
        {
            this.archerMenu.UnitsCounter += unitsAmount;
        }

        #endregion
    }
}
