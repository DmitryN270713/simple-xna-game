using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using FortDefenders.PersonHelperClasses;

namespace FortDefenders
{
    public sealed class WindmillMenu : ConstructionMenu
    {
        //Release button's width and height
        private const Int16 BTN_WIDTH = 64;
        private const Int16 BTN_HEIGHT = 64;
        //Laying off well-known amounts of farmers
        private const Int16 RELEASE_AMOUNT = 1;     //This value should be smaller than or equal to the MAX_NUMBER_OF_PEOPLE_INSIDE constant
        private const Int16 ADD_FARMERS = 1;
        //Button for laying off workers
        private Button<WindmillMenu> releaseWorkersBtn;
        private Texture2D releaseBtnTex;
        private readonly Point releaseBtnPos;
        private BuildingsParent buildingsParent;
        private Mayor mayorObject;
        private readonly Vector2 statPos = new Vector2(79, 70); //Fill free to change statistics text's position on the screen
        private Windmill windmillObject;



        public WindmillMenu(Game game) : base(game)
        {
            this.releaseBtnPos = new Point(game.GraphicsDevice.Viewport.Width - 10 - 64, 120);  //Fill free to place this button where ever you want 
            this.buildingsParent = game.Components.OfType<BuildingsParent>().ElementAt(0);
            this.buildingsParent.GetMayorObject(ref this.mayorObject);
            this.mayorObject.WindmillExists = true;
            this.mayorObject.OnRecallFarmers += new Mayor.RecallFarmers(mayorObject_OnRecallFarmers);
            this.mayorObject.OnAddFarmers += new Mayor.AddFarmers(mayorObject_OnAddFarmers);
        }

        protected sealed override void LoadContent()
        {
            this.releaseBtnTex = this.Game.Content.Load<Texture2D>("Textures/Windmill/release_btn");

            this.backgroundTexture = this.Game.Content.Load<Texture2D>("Textures/house_menu_background");
            this.constructionName = "Windmill";
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
            this.releaseWorkersBtn = new Button<WindmillMenu>(this.Game, this) 
            { BtnTexture = this.releaseBtnTex, Height = BTN_HEIGHT, Width = BTN_WIDTH, 
              Position = new Vector2(this.releaseBtnPos.X, this.releaseBtnPos.Y), Tag = "RELEASE" };
            this.releaseWorkersBtn.OnButtonClicked += new Button<WindmillMenu>.ButtonClicked(releaseWorkersBtn_OnButtonClicked);

        }

        private void releaseWorkersBtn_OnButtonClicked(Object o, String tag)
        {
            if (this.windmillObject.FarmersCounter > 0)
            {
                this.mayorObject.ReleaseFarmers(RELEASE_AMOUNT);        
                this.windmillObject.FarmersCounter -= RELEASE_AMOUNT;
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
            this.spriteBatch.DrawString(this.font, this.windmillObject.FarmersCounter + "/" + Windmill.MAX_NUMBER_OF_PEOPLE_INSIDE, this.statPos, Color.DarkBlue);
        }

        private void mayorObject_OnAddFarmers(Object sender)
        {
            if (this.mayorObject.GetFreeUnits(ADD_FARMERS, PrivilegesCosts.CommonWorker) && this.windmillObject.FarmersCounter < Windmill.MAX_NUMBER_OF_PEOPLE_INSIDE)
            {
                this.mayorObject.SendFarmers(ADD_FARMERS);
                this.windmillObject.FarmersCounter += ADD_FARMERS;
            }
            else
            {
                this.mayorObject.HideMenus();
                this.mayorObject.ShowMessageBox();
            }
        }

        private void mayorObject_OnRecallFarmers(Object sender)
        {
            if (this.windmillObject.FarmersCounter > 0)
            {
                this.mayorObject.ReleaseFarmers(RELEASE_AMOUNT);
                this.windmillObject.FarmersCounter -= RELEASE_AMOUNT;
            }
        }

        public void ConvertParent()
        {
            if (this.windmillObject == null)
            {
                this.windmillObject = this.parent as Windmill;
            }
        }

        #endregion
    }
}
