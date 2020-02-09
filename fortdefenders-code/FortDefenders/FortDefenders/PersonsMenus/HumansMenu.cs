using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FortDefenders.PersonsMenus
{
    public abstract class HumansMenu : HUD
    {
        private readonly Point CURRENT_TEX_POS = new Point(10, 40);
        private readonly Int16 WIDTH = 64;
        private readonly Int16 HEIGHT = 64;

        private readonly Int16 btnHeight = 32;
        private readonly Int16 btnWidth = 32;

        private readonly Vector2 txtPos;
        private readonly Vector2 txtNamePos;

        protected Button<HumansMenu> CancelBtn;
        protected Texture2D backgroundTexture;
        protected Texture2D curTex;
        protected SpriteFont font;
        protected Rectangle curTexRectangle;
        protected Rectangle backgroundRecktangle;
        protected SpriteBatch spriteBatch;
        protected Single health;
        protected HumansBase parent;
        protected String personRole;



        public HumansMenu(Game game) : base(game)
        {
            this.curTexRectangle = new Rectangle(CURRENT_TEX_POS.X, CURRENT_TEX_POS.Y, WIDTH, HEIGHT);
            this.backgroundRecktangle = new Rectangle(0, 0, this.Game.GraphicsDevice.Viewport.Width, this.Game.GraphicsDevice.Viewport.Height);
            this.txtPos = new Vector2(this.Game.GraphicsDevice.Viewport.Width - 100, 40.0f);
            this.txtNamePos = new Vector2(WIDTH + 15, 40);
            this.personRole = String.Empty;
        }

        protected override void LoadContent()
        {
            if (this.personRole == String.Empty)
            {
                throw new Exception("The role of the person must be specified. Example: this.personRole = \"Mayor\"");
            }

            this.spriteBatch = new SpriteBatch(this.Game.GraphicsDevice);
            this.font = this.Game.Content.Load<SpriteFont>("Fonts/Castellar");

            this.CancelBtn = new Button<HumansMenu>(this.Game, this)
            {
                BtnTexture = this.Game.Content.Load<Texture2D>("Textures/exitBtn"),
                Height = this.btnHeight,
                Width = this.btnWidth,
                Tag = "CANCEL_BTN",
                Position = new Vector2(this.GraphicsDevice.Viewport.Width - this.btnWidth - 10.0f,
                                       this.GraphicsDevice.Viewport.Height - 10.0f - this.btnHeight)
            };
            this.CancelBtn.OnButtonClicked += new Button<HumansMenu>.ButtonClicked(CancelBtn_OnButtonClicked);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (parent != null)
            {
                this.health = parent.Health;
                this.curTex = parent.FaceTex;

                this.CancelBtn.Update();
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.RasterizerState = new RasterizerState()
            {
                FillMode = FillMode.Solid
            };

            this.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, GraphicsDevice.RasterizerState);

            this.spriteBatch.Draw(this.backgroundTexture, this.backgroundRecktangle, Color.White);
            this.spriteBatch.Draw(this.curTex, this.curTexRectangle, Color.White);
            this.spriteBatch.DrawString(this.font, this.personRole, this.txtNamePos, Color.DarkBlue);
            this.spriteBatch.DrawString(this.font, String.Format("{0:000} %", this.health), this.txtPos, Color.DarkBlue);

            this.spriteBatch.End();

            this.CancelBtn.Draw();

            base.Draw(gameTime);
        }

        public virtual void SetParent(HumansBase parent)
        {
            this.parent = parent;
        }

        private void CancelBtn_OnButtonClicked(Object o, String tag)
        {
            this.CancelBtnClicked();
        }

        /// <summary>
        /// Call this method from the overriden one
        /// </summary>
        protected virtual void CancelBtnClicked()
        {
            this.OnHUDHiddenEvent();
            this.Visible = false;
            this.Enabled = false;
        }
    }
}
