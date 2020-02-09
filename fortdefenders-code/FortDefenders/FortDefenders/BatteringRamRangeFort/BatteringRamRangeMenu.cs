using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FortDefenders.PersonHelperClasses;

namespace FortDefenders.BatteringRamRangeFort
{
    public sealed class BatteringRamRangeMenu : ConstructionMenu
    {
        //Release button's width and height
        private const Int16 BTN_WIDTH = 64;
        private const Int16 BTN_HEIGHT = 64;
        //Laying off well-known amounts of soldiers inside the battery ram
        public const Int16 ADD_BATTERING_RAMS = 4;
        //Button for laying off soldiers
        private Button<BatteringRamRangeMenu> releaseSoldiersBtn;
        private Texture2D releaseBtnTex;
        private readonly Point releaseBtnPos;
        private BuildingsParent buildingsParent;
        private Mayor mayorObject;
        private readonly Vector2 statPos = new Vector2(79, 70); //Fill free to change statistics text's position on the screen
        private readonly Vector2 progressPos = new Vector2(79, 100); //Fill free to replace it with a real progress-bar
        private BatteringRamRange batteringRamRangeObject;
        private BatteringRam batteringRamObject;

        public BatteringRamRangeMenu(Game game) : base(game)
        {
            this.releaseBtnPos = new Point(game.GraphicsDevice.Viewport.Width - 10 - 64, 120);  //Fill free to place this button where ever you want 
            this.buildingsParent = game.Components.OfType<BuildingsParent>().ElementAt(0);
            this.buildingsParent.GetMayorObject(ref this.mayorObject);
            this.mayorObject.OnAddBatteringRam += new Mayor.AddBatteringRam(mayorObject_OnAddBatteringRam);
        }

        protected sealed override void LoadContent()
        {
            this.releaseBtnTex = this.Game.Content.Load<Texture2D>("Textures/BatteringRam/release_btn");

            this.backgroundTexture = this.Game.Content.Load<Texture2D>("Textures/BatteringRam/battering_ram_menu_background");
            this.constructionName = "Battering R. range";
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
            this.releaseSoldiersBtn = new Button<BatteringRamRangeMenu>(this.Game, this)
            {
                BtnTexture = this.releaseBtnTex,
                Height = BTN_HEIGHT,
                Width = BTN_WIDTH,
                Position = new Vector2(this.releaseBtnPos.X, this.releaseBtnPos.Y),
                Tag = "RELEASE"
            };
            this.releaseSoldiersBtn.OnButtonClicked += new Button<BatteringRamRangeMenu>.ButtonClicked(releaseSoldiersBtn_OnButtonClicked);

        }

        private void releaseSoldiersBtn_OnButtonClicked(Object o, String tag)
        {
            this.DecreaseBatteringRamsCounters();
        }

        public void DecreaseBatteringRamsCounters()
        {
            if (this.batteringRamRangeObject.BatteringRamsCounter > 0)
            {
                this.mayorObject.ReleaseBatteringRams(ADD_BATTERING_RAMS);
                this.batteringRamRangeObject.BatteringRamsCounter -= ADD_BATTERING_RAMS / ADD_BATTERING_RAMS;
                this.batteringRamObject.SetUnitsCounter(-ADD_BATTERING_RAMS / ADD_BATTERING_RAMS);
            }
        }

        private void UpdateBtns()
        {
            this.releaseSoldiersBtn.Update();
        }

        private void DrawBtns()
        {
            this.releaseSoldiersBtn.Draw();
        }

        private void DrawText()
        {
            this.spriteBatch.DrawString(this.font, this.batteringRamRangeObject.UnitsToBeTaught + "/" + this.batteringRamRangeObject.BatteringRamsCounter, this.statPos, Color.DarkBlue);
            this.spriteBatch.DrawString(this.font, String.Format("{0:0} %", this.batteringRamRangeObject.Progress), this.progressPos, Color.DarkBlue);
        }

        private void mayorObject_OnAddBatteringRam(Object sender)
        {
            if (this.mayorObject.GetFreeUnits(ADD_BATTERING_RAMS, PrivilegesCosts.Soldier))
            {
                this.mayorObject.SendBatteringRams(ADD_BATTERING_RAMS);
                this.batteringRamRangeObject.UnitsToBeTaught += ADD_BATTERING_RAMS;
            }
            else
            {
                this.mayorObject.HideMenus();
                this.mayorObject.ShowMessageBox();
            }
        }

        public void ConvertParent()
        {
            if (this.batteringRamRangeObject == null)
            {
                this.batteringRamRangeObject = this.parent as BatteringRamRange;
            }
        }

        public void SetBatteringRamObject(ref BatteringRam batteringRamObject)
        {
            this.batteringRamObject = batteringRamObject;
        }

        #endregion
    }
}
