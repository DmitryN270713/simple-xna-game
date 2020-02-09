using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FortDefenders.PersonHelperClasses;

namespace FortDefenders
{
    public sealed class BankMenu : ConstructionMenu
    {
        //Release button's width and height
        private const Int16 BTN_WIDTH = 64;
        private const Int16 BTN_HEIGHT = 64;
        //Laying off well-known amounts of bankers
        private const Int16 RELEASE_AMOUNT = 1;     //This value should be smaller than or equal to the MAX_NUMBER_OF_PEOPLE_INSIDE constant
        private const Int16 ADD_BANKERS = 1;
        //Button for laying off workers
        private Button<BankMenu> releaseWorkersBtn;
        private Texture2D releaseBtnTex;
        private readonly Point releaseBtnPos;
        private BuildingsParent buildingsParent;
        private Mayor mayorObject;
        private readonly Vector2 statPos = new Vector2(79, 70); //Fill free to change statistics text's position on the screen
        private Bank bankObject;

        public BankMenu(Game game) : base(game)
        {
            this.releaseBtnPos = new Point(game.GraphicsDevice.Viewport.Width - 10 - 64, 120);  //Fill free to place this button where ever you want 
            this.buildingsParent = game.Components.OfType<BuildingsParent>().ElementAt(0);
            this.buildingsParent.GetMayorObject(ref this.mayorObject);
            this.mayorObject.BankExists = true;
            this.mayorObject.OnRecallBankers += new Mayor.RecallBankers(mayorObject_OnRecallBankers);
            this.mayorObject.OnAddBankers += new Mayor.AddBankers(mayorObject_OnAddBankers);
        }

        protected sealed override void LoadContent()
        {
            this.releaseBtnTex = this.Game.Content.Load<Texture2D>("Textures/Bank/release_btn");

            this.backgroundTexture = this.Game.Content.Load<Texture2D>("Textures/House_menu_background");
            this.constructionName = "Bank";
            this.CreateBtns();

            base.LoadContent();
        }

        public sealed override void Update(GameTime gameTime)
        {
            this.UpdateBtns();

            base.Update(gameTime);
        }

        public sealed override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice.RasterizerState = new RasterizerState()
            {
                FillMode = FillMode.Solid
            };

            this.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, GraphicsDevice.RasterizerState);

            this.DrawText();

            this.spriteBatch.End();

            this.DrawBtns();
        }

        #region Own methods

        /// <summary>
        /// Creates button(s)
        /// </summary>
        private void CreateBtns()
        {
            this.releaseWorkersBtn = new Button<BankMenu>(this.Game, this)
            {
                BtnTexture = this.releaseBtnTex,
                Height = BTN_HEIGHT,
                Width = BTN_WIDTH,
                Position = new Vector2(this.releaseBtnPos.X, this.releaseBtnPos.Y),
                Tag = "RELEASE"
            };
            this.releaseWorkersBtn.OnButtonClicked += new Button<BankMenu>.ButtonClicked(releaseWorkersBtn_OnButtonClicked);

        }

        private void releaseWorkersBtn_OnButtonClicked(Object o, String tag)
        {
            if (this.bankObject.BankersCounter > 0)
            {
                this.mayorObject.ReleaseBankers(RELEASE_AMOUNT);
                this.bankObject.BankersCounter -= RELEASE_AMOUNT;
            }
        }

        private void UpdateBtns()
        {
            this.releaseWorkersBtn.Update();
        }

        private void DrawBtns()
        {
            this.releaseWorkersBtn.Draw();
        }

        private void DrawText()
        {
            this.spriteBatch.DrawString(this.font, this.bankObject.BankersCounter + "/" + Bank.MAX_NUMBER_OF_PEOPLE_INSIDE, this.statPos, Color.DarkBlue);
        }

        private void mayorObject_OnAddBankers(Object sender)
        {
            if (this.mayorObject.GetFreeUnits(ADD_BANKERS, PrivilegesCosts.PrivilegedWorker) 
                && this.bankObject.BankersCounter < Bank.MAX_NUMBER_OF_PEOPLE_INSIDE)
            {
                this.mayorObject.SendBankers(ADD_BANKERS);
                this.bankObject.BankersCounter += ADD_BANKERS;
            }
            else
            {
                this.mayorObject.HideMenus();
                this.mayorObject.ShowMessageBox();
            }
        }

        private void mayorObject_OnRecallBankers(Object sender)
        {
            if (this.bankObject.BankersCounter > 0)
            {
                this.mayorObject.ReleaseBankers(RELEASE_AMOUNT);
                this.bankObject.BankersCounter -= RELEASE_AMOUNT;
            }
        }

        public void ConvertParent()
        {
            if (this.bankObject == null)
            {
                this.bankObject = this.parent as Bank;
            }
        }

        #endregion
    }
}
