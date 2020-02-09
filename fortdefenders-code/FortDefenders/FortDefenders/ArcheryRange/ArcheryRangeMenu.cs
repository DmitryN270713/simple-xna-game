using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FortDefenders.PersonHelperClasses;

namespace FortDefenders.ArcheryRange
{
    public sealed class ArcheryRangeMenu : ConstructionMenu
    {
        //Release button's width and height
        private const Int16 BTN_WIDTH = 64;
        private const Int16 BTN_HEIGHT = 64;
        //Laying off well-known amounts of archers
        public const Int16 ADD_ARCHERS = 1;
        //Button for laying off soldiers
        private Button<ArcheryRangeMenu> releaseSoldiersBtn;
        private Texture2D releaseBtnTex;
        private readonly Point releaseBtnPos;
        private BuildingsParent buildingsParent;
        private Mayor mayorObject;
        private readonly Vector2 statPos = new Vector2(79, 70); //Fill free to change statistics text's position on the screen
        private readonly Vector2 progressPos = new Vector2(79, 100); //Fill free to replace it with a real progress-bar
        private ArcheryRange archeryObject;
        private Archer archerObject;

        public ArcheryRangeMenu(Game game) : base(game)
        {
            this.releaseBtnPos = new Point(game.GraphicsDevice.Viewport.Width - 10 - 64, 120);  //Fill free to place this button where ever you want 
            this.buildingsParent = game.Components.OfType<BuildingsParent>().ElementAt(0);
            this.buildingsParent.GetMayorObject(ref this.mayorObject);
            this.mayorObject.OnAddArchers += new Mayor.AddArchers(mayorObject_OnAddArchers);
        }

        protected sealed override void LoadContent()
        {
            this.releaseBtnTex = this.Game.Content.Load<Texture2D>("Textures/ArcheryRange/release_btn");

            this.backgroundTexture = this.Game.Content.Load<Texture2D>("Textures/ArcheryRange/archery_menu_background");
            this.constructionName = "Archery range";
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
            this.releaseSoldiersBtn = new Button<ArcheryRangeMenu>(this.Game, this)
            {
                BtnTexture = this.releaseBtnTex,
                Height = BTN_HEIGHT,
                Width = BTN_WIDTH,
                Position = new Vector2(this.releaseBtnPos.X, this.releaseBtnPos.Y),
                Tag = "RELEASE"
            };
            this.releaseSoldiersBtn.OnButtonClicked += new Button<ArcheryRangeMenu>.ButtonClicked(releaseSoldiersBtn_OnButtonClicked);

        }

        private void releaseSoldiersBtn_OnButtonClicked(Object o, String tag)
        {
            this.DecreaseArchersCounters();
        }

        public void DecreaseArchersCounters()
        {
            if (this.archeryObject.ArchersCounter > 0)
            {
                this.mayorObject.ReleaseArchers(ADD_ARCHERS);
                this.archeryObject.ArchersCounter -= ADD_ARCHERS;
                this.archerObject.SetUnitsCounter(-ADD_ARCHERS);
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
            this.spriteBatch.DrawString(this.font, this.archeryObject.UnitsToBeTaught + "/" + this.archeryObject.ArchersCounter, this.statPos, Color.DarkBlue);
            this.spriteBatch.DrawString(this.font, String.Format("{0:0} %", this.archeryObject.Progress), this.progressPos, Color.DarkBlue);
        }

        private void mayorObject_OnAddArchers(Object sender)
        {
            if (this.mayorObject.GetFreeUnits(ADD_ARCHERS, PrivilegesCosts.Soldier))
            {
                this.mayorObject.SendArchers(ADD_ARCHERS);
                this.archeryObject.UnitsToBeTaught += ADD_ARCHERS;
            }
            else
            {
                this.mayorObject.HideMenus();
                this.mayorObject.ShowMessageBox();
            }
        }

        public void ConvertParent()
        {
            if (this.archeryObject == null)
            {
                this.archeryObject = this.parent as ArcheryRange;
            }
        }

        public void SetArcherObject(ref Archer archerObject)
        {
            this.archerObject = archerObject;
        }

        #endregion
    }
}
