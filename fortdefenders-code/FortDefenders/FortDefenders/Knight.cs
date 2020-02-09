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
    public sealed class Knight : HumansBase
    {
        public const Int16 START_X = 37;
        public const Int16 START_Z = 37;
        private const Single MOVING_SPEED = 1.0f;       //A meter per second
        private const String PARENT_DIR = "Textures/Knight";
        private const Single STEP_TIME = 0.5f;
        private KnightMenu knightMenu;

        public Boolean BarracksBuilt { get; set; }
        public Barracks.Barracks BarracksObject { get; set; }

        public Knight(Game game) : base(game, CellState.Knight, MOVING_SPEED,
                                  new Point(START_X, START_Z), 0, STEP_TIME)
        {
            this.knightMenu = null;
            this.BarracksBuilt = false;
        }

        public override void LoadTextures()
        {
            PersonTexturesLoader.LoadTextures(PARENT_DIR, this.game.Content, out this.textures);
            //this.texture[key][index of item in value's array(array of textures)]
            this.leftLegTex = this.textures[0][(Int32)PersonMovement.left];
            this.rightLegTex = this.textures[0][(Int32)PersonMovement.right];
            this.stopTex = this.textures[0][(Int32)PersonMovement.stop];
            this.FaceTex = this.game.Content.Load<Texture2D>("Textures/Knight/face_tex");

            this.game.RegisterGameComponent<KnightMenu>(out this.knightMenu, false, false);
            this.menu = this.knightMenu;
            this.menu.SetParent(this);

            this.knightMenu.ConvertParent();

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

        public void DecreaseSwordsmenCounter()
        {
            BarracksObject.DecreaseSwordsmenCounters();
        }

        public void SetUnitsCounter(Int16 unitsAmount)
        {
            this.knightMenu.UnitsCounter += unitsAmount;
        }

        #endregion
    }
}
