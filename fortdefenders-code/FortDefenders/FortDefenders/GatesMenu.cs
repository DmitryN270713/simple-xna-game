using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FortDefenders
{
    public sealed class GatesMenu : ConstructionMenu
    {
        private readonly Int16 WIDTH = 64;
        private readonly Int16 HEIGHT = 64;

        private Texture2D openBtnTex;
        private Texture2D closeBtnTex;

        private Button<GatesMenu> openCloseBtn;

        public GatesMenu(Game game) : base(game)
        {

        }

        protected sealed override void LoadContent()
        {
            this.backgroundTexture = this.Game.Content.Load<Texture2D>("Textures/gate/gate_menu_background");
            this.openBtnTex = this.Game.Content.Load<Texture2D>("Textures/gate/gate_open_btn");
            this.closeBtnTex = this.Game.Content.Load<Texture2D>("Textures/gate/gate_close_btn");

            this.openCloseBtn = new Button<GatesMenu>(this.Game, this)
            {
                BtnTexture = this.closeBtnTex,
                Height = this.HEIGHT,
                Width = this.WIDTH,
                Position = new Vector2(10, this.Game.GraphicsDevice.Viewport.Height - 10.0f -this.HEIGHT),
                Tag = "OPEN_CLOSE_BTN"
            };
            this.openCloseBtn.OnButtonClicked += new Button<GatesMenu>.ButtonClicked(openCloseBtn_OnButtonClicked);

            this.constructionName = "Gates";
            base.LoadContent();
        }

        private void openCloseBtn_OnButtonClicked(Object o, String tag)
        {
            Gates gates = this.parent as Gates;
            GateState gatesState = gates.OpenCloseState;

            if (gatesState == GateState.Closed)
            {
                gates.OpenCloseState = GateState.Empty;
                this.openCloseBtn.BtnTexture = this.openBtnTex;
            }
            else
            {
                gates.OpenCloseState = GateState.Empty;
                this.openCloseBtn.BtnTexture = this.closeBtnTex;
            }
        }

        public sealed override void Update(GameTime gameTime)
        {
            this.openCloseBtn.Update();

            base.Update(gameTime);
        }

        public sealed override void Draw(GameTime gameTime)
        {
            this.openCloseBtn.Draw();

            base.Draw(gameTime);
        }
    }
}
