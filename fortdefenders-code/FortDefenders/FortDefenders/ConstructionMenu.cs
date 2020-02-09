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
    public abstract class ConstructionMenu : HUD
    {
        private readonly Point CURRENT_TEX_POS = new Point(10, 40);
        private readonly Int16 WIDTH = 64;
        private readonly Int16 HEIGHT = 64;

        private readonly Int16 btnHeight = 32;
        private readonly Int16 btnWidth = 32;

        private readonly Vector2 txtPos;
        private readonly Vector2 txtNamePos;

        protected Button<ConstructionMenu> OKBtn;
        protected Button<ConstructionMenu> CancelBtn;
        protected Texture2D backgroundTexture;
        protected Texture2D curTex;
        protected SpriteFont font;
        protected Rectangle curTexRectangle;
        protected Rectangle backgroundRecktangle;
        protected SpriteBatch spriteBatch;
        protected Single health;
        protected BuildingBase parent;
        protected String constructionName;

        public ConstructionMenu(Game game) : base(game)
        {
            this.curTexRectangle = new Rectangle(CURRENT_TEX_POS.X, CURRENT_TEX_POS.Y, WIDTH, HEIGHT);
            this.backgroundRecktangle = new Rectangle(0, 0, this.Game.GraphicsDevice.Viewport.Width, this.Game.GraphicsDevice.Viewport.Height);
            this.txtPos = new Vector2(this.Game.GraphicsDevice.Viewport.Width - 100, 40.0f);
            this.txtNamePos = new Vector2(WIDTH + 15, 40);
            this.constructionName = String.Empty;
        }

        protected override void LoadContent()
        {
            if (this.constructionName == String.Empty)
            {
                throw new Exception("The name of the construction must be specified. Example: this.constructionName = \"House\"");
            }

            this.spriteBatch = new SpriteBatch(this.Game.GraphicsDevice);
            this.font = this.Game.Content.Load<SpriteFont>("Fonts/Castellar");

            this.OKBtn = new Button<ConstructionMenu>(this.Game, this)
            {
                BtnTexture = this.Game.Content.Load<Texture2D>("Textures/okBtn"),
                Height = this.btnHeight,
                Width = this.btnWidth,
                Tag = "OK_BTN",
                Position = new Vector2(this.GraphicsDevice.Viewport.Width - 2.0f * this.btnWidth - 25.0f, 
                                       this.GraphicsDevice.Viewport.Height - 10.0f - this.btnHeight)
            };
            this.OKBtn.OnButtonClicked += new Button<ConstructionMenu>.ButtonClicked(OKBtn_OnButtonClicked);

            this.CancelBtn = new Button<ConstructionMenu>(this.Game, this)
            {
                BtnTexture = this.Game.Content.Load<Texture2D>("Textures/exitBtn"),
                Height = this.btnHeight,
                Width = this.btnWidth,
                Tag = "CANCEL_BTN",
                Position = new Vector2(this.GraphicsDevice.Viewport.Width - this.btnWidth - 10.0f,
                                       this.GraphicsDevice.Viewport.Height - 10.0f - this.btnHeight)
            };
            this.CancelBtn.OnButtonClicked += new Button<ConstructionMenu>.ButtonClicked(CancelBtn_OnButtonClicked);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (parent != null)
            {
                this.health = parent.GetCurrentHealth();
                this.curTex = parent.GetCurrentTexture();
                this.OKBtn.Update();
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
            this.spriteBatch.DrawString(this.font, this.constructionName, this.txtNamePos, Color.DarkBlue);
            this.spriteBatch.DrawString(this.font, String.Format("{0:000} %", this.health), this.txtPos, Color.DarkBlue);

            this.spriteBatch.End();

            this.OKBtn.Draw();
            this.CancelBtn.Draw();

            base.Draw(gameTime);
        }

        public virtual void SetParent(BuildingBase parent)
        {
            this.parent = parent;
        }

        private void OKBtn_OnButtonClicked(Object o, String tag)
        {
            this.OKBtnClicked();
        }

        private void CancelBtn_OnButtonClicked(Object o, String tag)
        {
            this.CancelBtnClicked();
        }

        /// <summary>
        /// Call this method from the overriden one
        /// </summary>
        protected virtual void OKBtnClicked()
        {
            this.OnHUDHiddenEvent();
            this.Visible = false;
            this.Enabled = false;
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
