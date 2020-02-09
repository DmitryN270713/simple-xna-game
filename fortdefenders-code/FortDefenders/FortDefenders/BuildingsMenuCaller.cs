using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace FortDefenders
{
    /// <summary>
    /// More navigation buttons shall be added into this class
    /// </summary>
    public sealed class BuildingsMenuCaller : HUD
    {
        public const Int16 HEIGHT = 64;
        private const Int16 WIDTH_BTN = 54;
        private const Int16 HEIGHT_BTN = 54;
        private const Int16 OFFSET = 5;
        private readonly Rectangle backgroundRect;

        private SpriteBatch spriteBatch;
        private Button<BuildingsMenuCaller> callerBtn;
        private Texture2D backgroundTex;
        private BuildingsSelector buildingsSelector;

        public BuildingsMenuCaller(Game game) : base(game)
        {
            this.backgroundRect = new Rectangle(0, game.GraphicsDevice.Viewport.Height - HEIGHT, game.GraphicsDevice.Viewport.Width, HEIGHT);
        }

        public void SetSelector(ref BuildingsSelector buildingsSelector)
        {
            this.buildingsSelector = buildingsSelector;
        }

        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(this.Game.GraphicsDevice);
            Texture2D backgroundBtn = this.Game.Content.Load<Texture2D>("Textures/MenuSelection/caller_background");
            this.backgroundTex = this.Game.Content.Load<Texture2D>("Textures/MenuSelection/menu_background_main");

            this.callerBtn = new Button<BuildingsMenuCaller>(this.Game, this)
            {
                BtnTexture = backgroundBtn,
                Height = HEIGHT_BTN,
                Width = WIDTH_BTN,
                Tag = "CALLER",
                Position = new Vector2(OFFSET, this.Game.GraphicsDevice.Viewport.Height - HEIGHT_BTN - OFFSET)
            };

            this.callerBtn.OnButtonClicked += new Button<BuildingsMenuCaller>.ButtonClicked(callerBtn_OnButtonClicked);

            base.LoadContent();
        }

        private void callerBtn_OnButtonClicked(Object o, String tag)
        {
            this.buildingsSelector.Enabled = true;
            this.buildingsSelector.Visible = true;
        }

        public override void Update(GameTime gameTime)
        {
            this.callerBtn.Update();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.RasterizerState = new RasterizerState()
            {
                FillMode = FillMode.Solid
            };

            this.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, GraphicsDevice.RasterizerState);

            this.spriteBatch.Draw(this.backgroundTex, this.backgroundRect, Color.White);

            this.spriteBatch.End();

            this.callerBtn.Draw();
            base.Draw(gameTime);
        }
    }
}
