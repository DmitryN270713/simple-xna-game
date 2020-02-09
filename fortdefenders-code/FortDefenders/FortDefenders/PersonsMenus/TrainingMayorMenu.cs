using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FortDefenders.PersonsMenus
{
    public sealed class TrainingMayorMenu : HUD
    {
        public delegate void AddRequestSent(Object sender, SoldierRole role);
        public event AddRequestSent OnAddRequestSent;

        private const Int16 BTN_WIDTH = 128;
        private const Int16 BTN_HEIGHT = 64;
        private const Int16 FUNCBTN_WIDTH = 32;
        private const Int16 FUNCBTN_HEIGHT = 32;
        private const Int16 OFFSET_Y = 15;
        private const Int16 OFFSET_X = 200;

        private Texture2D backgroundTex;
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private Rectangle backgroundRect;
        private Button<TrainingMayorMenu> cancelBtn;
        private readonly Point startBtnsPos = new Point(70, 70);
        private readonly Point startTxtPos;
        private Button<TrainingMayorMenu>[] trainingBtns;
        private MayorMenu mayorMenu;
        private Func<Int16>[] counters;
        private Vector2[] txtPositions;
        private String[] txtToDraw;


        public TrainingMayorMenu(Game game) : base(game)
        {
            this.trainingBtns = new Button<TrainingMayorMenu>[3];
            this.startTxtPos = new Point(this.startBtnsPos.X + BTN_WIDTH + OFFSET_X, 85);
            this.txtPositions = new Vector2[3];
            this.txtToDraw = new String[3];
            this.backgroundRect = new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);
        }

        protected override void LoadContent()
        {
            this.mayorMenu = this.Game.Components.OfType<MayorMenu>().ElementAt(0);
            counters = new Func<Int16>[3] { this.mayorMenu.GetSwordsmenCounter, 
                                            this.mayorMenu.GetArchersCounter, 
                                            this.mayorMenu.GetBatteringRamCounter };
            this.CreateButtons();
            this.FillTxtPositions();

            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.font = this.Game.Content.Load<SpriteFont>("Fonts/Castellar_HUD");
            this.backgroundTex = this.Game.Content.Load<Texture2D>("Textures/Mayor/mayor_menu_background");
            this.UpdateText();

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            this.UpdateBtns();
            this.UpdateText();
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
            this.DrawText();
            this.spriteBatch.End();

            this.DrawBtns();
            base.Draw(gameTime);
        }

        private void CreateButtons()
        {
            this.cancelBtn = new Button<TrainingMayorMenu>(this.Game, this)
            {
                BtnTexture = this.Game.Content.Load<Texture2D>("Textures/exitBtn"),
                Height = FUNCBTN_HEIGHT,
                Width = FUNCBTN_WIDTH,
                Tag = "CANCEL_BTN",
                Position = new Vector2(this.GraphicsDevice.Viewport.Width - FUNCBTN_WIDTH - 10.0f,
                                       this.GraphicsDevice.Viewport.Height - 10.0f - FUNCBTN_HEIGHT)
            };
            this.cancelBtn.OnButtonClicked += new Button<TrainingMayorMenu>.ButtonClicked(cancelBtn_OnButtonClicked);

            for (Int32 i = 0; i < 3; i++)
            {
                Button<TrainingMayorMenu> btn = new Button<TrainingMayorMenu>(this.Game, this)
                {
                    BtnTexture = this.Game.Content.Load<Texture2D>("Textures/Mayor/" + i),
                    Height = BTN_HEIGHT,
                    Width = BTN_WIDTH,
                    Position = new Vector2(this.startBtnsPos.X, this.startBtnsPos.Y + i * BTN_HEIGHT + i * OFFSET_Y),
                    Tag = "" + i
                };
                btn.OnButtonClicked += new Button<TrainingMayorMenu>.ButtonClicked(btn_OnButtonClicked);
                this.trainingBtns[i] = btn;
            }
        }

        private void FillTxtPositions()
        {
            for (Int16 i = 0; i < 3; i++)
            {
                this.txtPositions[i] = new Vector2(this.startTxtPos.X, 
                    this.startTxtPos.Y + BTN_HEIGHT * i + OFFSET_Y * i);
            }
        }

        private void cancelBtn_OnButtonClicked(Object o, String tag)
        {
            this.OnHUDHiddenEvent();
            this.Visible = false;
            this.Enabled = false;
        }

        private void btn_OnButtonClicked(Object o, String tag)
        {
            this.OnAddRequestSentEvent(tag);
        }

        private void UpdateBtns()
        {
            this.cancelBtn.Update();
            for (Int16 i = 0; i < 3; i++)
            {
                this.trainingBtns[i].Update();
            }
        }

        private void DrawBtns()
        {
            this.cancelBtn.Draw();
            for (Int16 i = 0; i < 3; i++)
            {
                this.trainingBtns[i].Draw();
            }
        }

        private void UpdateText()
        {
            for (Int16 i = 0; i < 3; i++)
            {
                this.txtToDraw[i] = "" + this.counters[i].Invoke();
            }
        }

        private void DrawText()
        {
            for (Int16 i = 0; i < 3; i++)
            {
                this.spriteBatch.DrawString(this.font, this.txtToDraw[i], this.txtPositions[i], Color.DarkBlue);
            }
        }

        private void OnAddRequestSentEvent(String role)
        {
            if (this.OnAddRequestSent != null)
            {
                this.OnAddRequestSent(this, ((SoldierRole)Convert.ToInt32(role)));
            }
        }
    }
}
