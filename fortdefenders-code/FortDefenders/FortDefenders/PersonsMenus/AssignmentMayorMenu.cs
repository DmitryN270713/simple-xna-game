using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace FortDefenders.PersonsMenus
{
    public class AssignmentMayorMenu : HUD
    {
        public delegate void RemoveRequestSent(Object sender, WorkerRole role);
        public event RemoveRequestSent OnRemoveRequestSent;
        public delegate void AddRequestSent(Object sender, WorkerRole role);
        public event AddRequestSent OnAddRequestSent;

        private const Int16 BTN_WIDTH = 32;
        private const Int16 BTN_HEIGHT = 32;
        private const Int16 OFFSET_Y = 10;
        private const Int16 OFFSET_X = 336;

        private Texture2D backgroundTex;
        private Texture2D addTex;
        private Texture2D removeTex;
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private Rectangle backgroundRect;
        private Button<AssignmentMayorMenu> CancelBtn;
        private MayorMenu mayorMenu;
        private readonly Point startBtnsPos = new Point(100, 70);
        private readonly Point startTextPosition = new Point(200, 75);
        private Func<Int16>[] countersToScreen;
        private Button<AssignmentMayorMenu>[] btnsRecall;
        private Button<AssignmentMayorMenu>[] btnsAdd;
        private Vector2[] txtPositions;
        private String[] txtToDraw;
        private String freeCounter;
        private readonly Vector2 freeCounterPos = new Vector2(200, 45);


        public AssignmentMayorMenu(Game game) : base(game)
        {
            this.backgroundRect = new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);
            this.btnsRecall = new Button<AssignmentMayorMenu>[6];
            this.btnsAdd = new Button<AssignmentMayorMenu>[6];
            this.txtPositions = new Vector2[6];
            this.txtToDraw = new String[6];
        }

        protected sealed override void LoadContent()
        {
            this.mayorMenu = this.Game.Components.OfType<MayorMenu>().ElementAt(0);
            this.countersToScreen = new Func<Int16>[6] { mayorMenu.GetBuildersCounter, mayorMenu.GetLumberjacksCounter,
                                                         mayorMenu.GetMinersCounter, mayorMenu.GetFarmersCounter,
                                                         mayorMenu.GetBankersCounter, mayorMenu.GetSellersCounter };

            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.font = this.Game.Content.Load<SpriteFont>("Fonts/Castellar_HUD");

            this.CancelBtn = new Button<AssignmentMayorMenu>(this.Game, this)
            {
                BtnTexture = this.Game.Content.Load<Texture2D>("Textures/exitBtn"),
                Height = BTN_HEIGHT,
                Width = BTN_WIDTH,
                Tag = "CANCEL_BTN",
                Position = new Vector2(this.GraphicsDevice.Viewport.Width - BTN_WIDTH - 10.0f,
                                       this.GraphicsDevice.Viewport.Height - 10.0f - BTN_HEIGHT)
            };
            this.CancelBtn.OnButtonClicked += new Button<AssignmentMayorMenu>.ButtonClicked(CancelBtn_OnButtonClicked);

            this.backgroundTex = this.Game.Content.Load<Texture2D>("Textures/Mayor/mayor_menu_background");
            this.addTex = this.Game.Content.Load<Texture2D>("Textures/Mayor/add");
            this.removeTex = this.Game.Content.Load<Texture2D>("Textures/Mayor/remove");

            this.CreateButtons();
            this.FillTextPositions();
            this.UpdateText();
            this.freeCounter = "Available workers: " + this.mayorMenu.GetFreeCounter();

            base.LoadContent();
        }

        private void CancelBtn_OnButtonClicked(Object o, String tag)
        {
            this.OnHUDHiddenEvent();
            this.Visible = false;
            this.Enabled = false;
        }

        public sealed override void Update(GameTime gameTime)
        {
            this.CancelBtn.Update();
            this.UpdateText();
            this.UpdateBtns();
            base.Update(gameTime);
        }

        public sealed override void Draw(GameTime gameTime)
        {
            GraphicsDevice.RasterizerState = new RasterizerState()
            {
                FillMode = FillMode.Solid
            };

            this.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, GraphicsDevice.RasterizerState);
            this.spriteBatch.Draw(this.backgroundTex, this.backgroundRect, Color.White);
            this.DrawText();
            this.spriteBatch.End();

            this.CancelBtn.Draw();
            this.DrawBtns();

            base.Draw(gameTime);
        }

        #region Buttons and events

        private void CreateButtons()
        {
            for (Int16 i = 0; i < 6; i++)
            {
                Button<AssignmentMayorMenu> btnRecall = new Button<AssignmentMayorMenu>(this.Game, this)
                {
                    BtnTexture = this.removeTex,
                    Height = BTN_HEIGHT,
                    Width = BTN_WIDTH,
                    Position = new Vector2(this.startBtnsPos.X, i * OFFSET_Y + i * BTN_HEIGHT + this.startBtnsPos.Y),
                    Tag = i.ToString()
                };
                btnRecall.OnButtonClicked += new Button<AssignmentMayorMenu>.ButtonClicked(btnRecall_OnButtonClicked);
                this.btnsRecall[i] = btnRecall; 
            }
            for (Int16 i = 0; i < 6; i++)
            {
                Button<AssignmentMayorMenu> btnAdd = new Button<AssignmentMayorMenu>(this.Game, this)
                {
                    BtnTexture = this.addTex,
                    Height = BTN_HEIGHT,
                    Width = BTN_WIDTH,
                    Position = new Vector2(this.startBtnsPos.X + BTN_WIDTH + OFFSET_X, i * OFFSET_Y + i * BTN_HEIGHT + this.startBtnsPos.Y),
                    Tag = i.ToString()
                };
                btnAdd.OnButtonClicked += new Button<AssignmentMayorMenu>.ButtonClicked(btnAdd_OnButtonClicked);
                this.btnsAdd[i] = btnAdd;
            }
        }

        private void btnAdd_OnButtonClicked(Object o, String tag)
        {
            if (tag == "0")
            {
                this.OnHUDHiddenEvent();
                this.Enabled = false;
                this.Visible = false;
            }
            this.OnAddRequestSentEvent(tag);
        }

        private void btnRecall_OnButtonClicked(Object o, String tag)
        {
            if (tag == "0")
            {
                this.OnHUDHiddenEvent();
                this.Enabled = false;
                this.Visible = false;
            }
            this.OnRemoveRequestSentEvent(tag);
        }

        private void UpdateBtns()
        {
            for (Int16 i = 0; i < 6; i++)
            {
                this.btnsRecall[i].Update();
                this.btnsAdd[i].Update();
            }
        }

        private void DrawBtns()
        {
            for (Int16 i = 0; i < 6; i++)
            {
                this.btnsRecall[i].Draw();
                this.btnsAdd[i].Draw();
            }
        }

        #endregion

        #region Text on the screen

        private void FillTextPositions()
        {
            for (Int32 i = 0; i < 6; i++)
            {
                this.txtPositions[i] = new Vector2(this.startTextPosition.X, this.startTextPosition.Y + OFFSET_Y * i + BTN_WIDTH * i);
            }
        }

        private void UpdateText()
        {
            for (Int32 i = 0; i < 6; i++)
            {
                this.txtToDraw[i] = ((WorkerRole)i).ToString() + ": " + this.countersToScreen[i].Invoke();
            }
            this.freeCounter = "Available workers: " + this.mayorMenu.GetFreeCounter();
        }


        private void DrawText()
        {
            for (Int32 i = 0; i < 6; i++)
            {
                this.spriteBatch.DrawString(this.font, this.txtToDraw[i], this.txtPositions[i], Color.DarkBlue);
            }
            this.spriteBatch.DrawString(this.font, this.freeCounter, this.freeCounterPos, Color.DarkBlue);
        }

        #endregion

        private void OnAddRequestSentEvent(String tag)
        {
            if (this.OnAddRequestSent != null)
            {
                this.OnAddRequestSent(this, (WorkerRole)Convert.ToInt16(tag));
            }
        }

        /// <summary>
        /// Recall workers from the object request
        /// </summary>
        /// <param name="tag">must be a number</param>
        private void OnRemoveRequestSentEvent(String tag)
        {
            if (this.OnRemoveRequestSent != null)
            {
                this.OnRemoveRequestSent(this, (WorkerRole)Convert.ToInt16(tag));
            }
        }
    }
}
