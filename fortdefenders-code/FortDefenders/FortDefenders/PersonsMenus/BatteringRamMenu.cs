using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FortDefenders.PersonsMenus
{
    public sealed class BatteringRamMenu : HumansMenu
    {
        private const String UNITS_NAME = "Battering rams \nunder control ";
        private const Int16 BTN_HEIGHT = 64;
        private const Int16 BTN_WIDTH = 64;
        private const Int16 OFFSET = 10;

        private BatteringRam batteringRamObject;
        private Button<BatteringRamMenu> releaseBtn;
        private readonly Vector2 btnStartPoint;
        private readonly Vector2 statsPos = new Vector2(10, 140);

        public Int16 UnitsCounter { get; set; }

        public BatteringRamMenu(Game game) : base(game)
        {
            this.btnStartPoint = new Vector2(game.GraphicsDevice.Viewport.Width - OFFSET - BTN_WIDTH, 100);     //Fill free to move it somewhere else
        }

        protected sealed override void LoadContent()
        {
            this.backgroundTexture = this.Game.Content.Load<Texture2D>("Textures/BatteringRamPerson/battering_ram_menu_background");
            this.personRole = "Battering ram";
            this.CreateBtns();

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            this.UpdateBtns();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice.RasterizerState = new RasterizerState()
            {
                FillMode = FillMode.Solid
            };

            this.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, GraphicsDevice.RasterizerState);

            this.spriteBatch.DrawString(this.font, String.Format("{0}{1}", UNITS_NAME, this.UnitsCounter), this.statsPos, Color.DarkBlue);

            this.spriteBatch.End();
            this.DrawBtns();
        }

        #region Own methods

        /// <summary>
        /// Should be done from the Loadtextures method of the parent class
        /// Otherwise the NullReference exception will be thrown
        /// </summary>
        public void ConvertParent()
        {
            this.batteringRamObject = this.parent as BatteringRam;
        }

        private void CreateBtns()
        {
            this.releaseBtn = new Button<BatteringRamMenu>(this.Game, this)
            {
                BtnTexture = this.Game.Content.Load<Texture2D>("Textures/BatteringRamPerson/release_btn"),
                Height = BTN_HEIGHT,
                Width = BTN_WIDTH,
                Position = this.btnStartPoint,
                Tag = "RELEASE"
            };
            this.releaseBtn.OnButtonClicked += new Button<BatteringRamMenu>.ButtonClicked(releaseBtn_OnButtonClicked);
        }

        private void releaseBtn_OnButtonClicked(Object o, String tag)
        {
            if (this.batteringRamObject.BatteringRamRangeBuilt)
            {
                this.batteringRamObject.DecreaseBatteringRamsCounter();
            }
            else
            {
                this.Enabled = false;
                this.Visible = false;
                this.OnHUDHiddenEvent();
                MessageBox msgBox = null;
                MessageBox.Show(this.Game, "I have no enough authority for this.\nBuild battering ram range\n first.", "Commander said: ",
                                MessageBoxButton.OK, this.curTex, out msgBox);
            }
        }

        private void UpdateBtns()
        { 
            this.releaseBtn.Update();
        }

        private void DrawBtns()
        {
            this.releaseBtn.Draw();
        }

        #endregion
    }
}
