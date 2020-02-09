using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace FortDefenders
{
    public sealed class BuildingsSelector : HUD
    {
        public delegate void BuildingSelected(Object o, CellState type);
        public event BuildingSelected OnBuildingSelected;

        private const Int16 COLUMNS = 4;
        private const Int16 ROWS = 2;
        private const Int16 ICON_WIDTH = 64;
        private const Int16 ICON_HEIGHT = 64;
        private const Int16 OFFSET = 10;
        private const Int16 START_X = 157;
        private const Int16 START_Y = 111;
        private const Int16 BTN_WIDTH = 128;
        private const Int16 BTN_HEIGHT = 32;

        private readonly Rectangle backgroundRect;

        private Texture2D backgroundTex;
        private SpriteBatch spriteBatch;
        private List<Button<BuildingsSelector>> lsIcons;
        private Button<BuildingsSelector> cancelBtn;

        public BuildingsSelector(Game game) : base(game)
        {
            this.backgroundRect = new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);
            this.lsIcons = new List<Button<BuildingsSelector>>();
        }

        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(this.Game.GraphicsDevice);
            this.backgroundTex = this.Game.Content.Load<Texture2D>("Textures/MenuSelection/selector_background");
            Texture2D cancelBtnTex = this.Game.Content.Load<Texture2D>("Textures/MenuSelection/cancelBtn");

            this.cancelBtn = new Button<BuildingsSelector>(this.Game, this)
            {
                BtnTexture = cancelBtnTex,
                Height = BTN_HEIGHT,
                Width = BTN_WIDTH,
                Position = new Vector2(this.Game.GraphicsDevice.Viewport.Width - OFFSET - BTN_WIDTH,
                                       this.Game.GraphicsDevice.Viewport.Height - OFFSET - BTN_HEIGHT),
                Tag = "CANCEL_BTN"
            };

            this.cancelBtn.OnButtonClicked += new Button<BuildingsSelector>.ButtonClicked(cancelBtn_OnButtonClicked);
            this.CreateIcons();

            base.LoadContent();
        }

        private void CreateIcons()
        {
            for (Int16 i = 0; i < ROWS; i++)
            {
                for (Int16 j = 0; j < COLUMNS; j++)
                {
                    Texture2D texture = this.Game.Content.Load<Texture2D>("Textures/MenuSelection/" + (j + i * COLUMNS).ToString());
                    Button<BuildingsSelector> btn = new Button<BuildingsSelector>(this.Game, this)
                    {
                        BtnTexture = texture,
                        Height = ICON_HEIGHT,
                        Width = ICON_WIDTH,
                        Position = new Vector2(START_X + j * (ICON_WIDTH + OFFSET), START_Y + i * (OFFSET + ICON_HEIGHT)),
                        Tag = (j + i * COLUMNS).ToString()
                    };

                    btn.OnButtonClicked += new Button<BuildingsSelector>.ButtonClicked(btn_OnButtonClicked);
                    this.lsIcons.Add(btn);
                }
            }
        }

        private void btn_OnButtonClicked(Object o, String tag)
        {
            CellState type = (CellState)(Convert.ToInt32(tag) + 1);
            this.OnBuildingSelectedEvent(type);
        }

        private void cancelBtn_OnButtonClicked(Object o, String tag)
        {
            this.Enabled = false;
            this.Visible = false;
        }

        public override void Update(GameTime gameTime)
        {
            this.cancelBtn.Update();
            this.UpdateIcons();
            base.Update(gameTime);
        }

        private void UpdateIcons()
        {
            Int16 count = Convert.ToInt16(this.lsIcons.Count);
            for (Int16 i = 0; i < count; i++)
            {
                this.lsIcons[i].Update();
            }
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

            this.cancelBtn.Draw();
            this.DrawIcons();

            base.Draw(gameTime);
        }

        private void DrawIcons()
        {
            Int16 count = Convert.ToInt16(this.lsIcons.Count);
            for (Int16 i = 0; i < count; i++)
            {
                this.lsIcons[i].Draw();
            }
        }

        private void OnBuildingSelectedEvent(CellState type)
        {
            if (this.OnBuildingSelected != null)
            {
                this.OnBuildingSelected(this, type);
            }
        }
    }
}
