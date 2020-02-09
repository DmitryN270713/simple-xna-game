using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FortDefenders.CityHall
{
    public class CityhallMenu : ConstructionMenu
    {
        //Hire worker button's width and height
        private const Int16 BTN_WIDTH = 64;
        private const Int16 BTN_HEIGHT = 64;
        private const Int16 OFFSET = 10;
        public const Int16 WORKERS_AT_TIME = 1;
        private readonly Vector2 statPos = new Vector2(80, 70); //Fill free to change statistics text's position on the screen
        private readonly Point hireBtnPos;
        private readonly Int32[] resourcesNeeded = new Int32[4] { 0, 0, 1, 50 }; //{stone, wood, gold, food}
        private readonly Vector2 progressPos = new Vector2(80, 100); //Fill free to replace it with a real progress-bar

        private Button<CityhallMenu> hireBtn;
        private ResourcesHUD resourcesHUD;
        private Int16 currentPopulation;
        private Int16 currentMaxPopulation;
        private Cityhall cityhallObject;

        public CityhallMenu(Game game) : base(game)
        {
            this.hireBtnPos = new Point(game.GraphicsDevice.Viewport.Width - 10 - 64, 120);  //Fill free to place this button where ever you want 
            this.resourcesHUD = game.Components.OfType<ResourcesHUD>().ElementAt(0);
        }
        
        protected sealed override void LoadContent()
        {
            this.backgroundTexture = this.Game.Content.Load<Texture2D>("Textures/Cityhall/cityhall_background");
            this.constructionName = "Cityhall";
            this.CreateBtns();
            this.UpdateText();
            base.LoadContent();
        }

        public sealed override void Update(GameTime gameTime)
        {
            this.UpdateBtns();
            this.UpdateText();
            base.Update(gameTime);
        }

        public sealed override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            this.DrawBtns();

            GraphicsDevice.RasterizerState = new RasterizerState()
            {
                FillMode = FillMode.Solid
            };

            this.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, GraphicsDevice.RasterizerState);
            
            this.DrawText();

            this.spriteBatch.End();
        }

        #region Own methods

        private void CreateBtns()
        {
            this.hireBtn = new Button<CityhallMenu>(this.Game, this)
            {
                BtnTexture = this.Game.Content.Load<Texture2D>("Textures/Cityhall/hire_worker_btn"),
                Height = BTN_HEIGHT,
                Width = BTN_WIDTH,
                Position = new Vector2(this.GraphicsDevice.Viewport.Width - OFFSET - BTN_WIDTH, this.hireBtnPos.Y),
                Tag = "HIRE_WORKER"
            };
            this.hireBtn.OnButtonClicked += new Button<CityhallMenu>.ButtonClicked(hireBtn_OnButtonClicked);
        }

        private void hireBtn_OnButtonClicked(Object o, String tag)
        {
            if (this.cityhallObject.CanHire)
            {
                if (this.resourcesHUD.SetCurrentResources(Convert.ToInt16(-this.resourcesNeeded[0]),
                                                          Convert.ToInt16(-this.resourcesNeeded[1]),
                                                          Convert.ToInt16(-this.resourcesNeeded[2]),
                                                          Convert.ToInt16(-this.resourcesNeeded[3])))
                {
                    this.cityhallObject.UnitsToBeHired += WORKERS_AT_TIME;
                }
                else
                {
                    this.Enabled = false;
                    this.Visible = false;
                    this.OnHUDHiddenEvent();
                    MessageBox msgBox = null;
                    MessageBox.Show(this.Game, "Not enough resources\nto hire new workers", "Not enough resources",
                                    MessageBoxButton.OK, this.curTex, out msgBox);
                }
            }
        }

        private void DrawBtns()
        {
            this.hireBtn.Draw();
        }

        private void UpdateBtns()
        {
            this.hireBtn.Update();
        }

        private void DrawText()
        {
            this.spriteBatch.DrawString(this.font, String.Format("{0}/{1}", this.currentPopulation, this.currentMaxPopulation), this.statPos, Color.DarkBlue);
            this.spriteBatch.DrawString(this.font, String.Format("{0:0} %", this.cityhallObject.Progress), this.progressPos, Color.DarkBlue);
        }

        private void UpdateText()
        {
            this.resourcesHUD.GetPopulationInfo(out this.currentMaxPopulation, out this.currentPopulation);
        }

        public void ConvertParent()
        {
            this.cityhallObject = this.parent as Cityhall;
        }

        #endregion
    }
}
