using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;


namespace FortDefenders
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class GameScreen : DrawableGameComponent
    {
        private GameField gameField;
        private ZoomInOutHUD zoomHUD;
        private InputManager inputMng;
        private Vector2 currentPos;
        private Vector2 oldPos;
        private readonly Rectangle zoomInOutHUDRect;
        private PickingService pickingService;
        private Boolean zoomHUDShown;
        private Boolean selectorShown;
        private MenusCaller menusCaller;
        private BuildingsSelector buildingsSelector;

        public GameScreen(Game game)
            : base(game)
        {
            game.RegisterGameComponent<ZoomInOutHUD>(out this.zoomHUD, false, false);
            this.inputMng = game.GetService<InputManager>();
            this.oldPos = Vector2.Zero;

            Int32 width = game.GraphicsDevice.Viewport.Width;
            Int32 height = game.GraphicsDevice.Viewport.Height;

            this.zoomInOutHUDRect = new Rectangle(width - ZoomInOutHUD.RECTW, height - 200 - ZoomInOutHUD.RECTH,
                                                  ZoomInOutHUD.RECTW, ZoomInOutHUD.RECTH * 2);
            this.zoomHUDShown = false;
            this.selectorShown = false;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            this.Game.RegisterGameComponent<GameField>(out this.gameField, true, true);
            this.pickingService = this.Game.GetService<PickingService>();
            this.Game.RegisterGameComponent<MenusCaller>(out this.menusCaller, true, true);
            this.Game.RegisterGameComponent<BuildingsSelector>(out this.buildingsSelector, false, false);
            this.menusCaller.SetSelector(ref this.buildingsSelector);
            this.menusCaller.EnabledChanged += new EventHandler<EventArgs>(menusCaller_EnabledChanged);
            this.gameField.SetBuildingsSelector(ref this.buildingsSelector);
            this.buildingsSelector.EnabledChanged += new EventHandler<EventArgs>(buildingsSelector_EnabledChanged);

            base.Initialize();
        }

        private void menusCaller_EnabledChanged(Object sender, EventArgs e)
        {
            this.selectorShown = !this.menusCaller.Enabled;
        }

        private void buildingsSelector_EnabledChanged(Object sender, EventArgs e)
        {
            if (this.buildingsSelector.Enabled)
            {
                this.pickingService.Enabled = false;
                this.menusCaller.Enabled = false;
                this.menusCaller.OnHUDHiddenEvent();
                this.zoomHUD.Enabled = false;
                this.zoomHUD.Visible = false;
                this.selectorShown = true;
            }
            else
            {
                this.pickingService.Enabled = true;
                this.menusCaller.Enabled = true;
                this.buildingsSelector.OnHUDHiddenEvent();
                this.selectorShown = false;
            }
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            this.currentPos = this.inputMng.GetTapPosition();
            if(this.currentPos != oldPos)
            {
                this.EnableZoomHUD();
                this.oldPos = this.currentPos;
            }

            base.Update(gameTime);
        }

        private void EnableZoomHUD()
        {
            if (!this.zoomHUDShown && !this.selectorShown && this.zoomInOutHUDRect.Contains(Convert.ToInt32(this.currentPos.X), Convert.ToInt32(this.currentPos.Y)))
            {
                this.zoomHUD.Enabled = true;
                this.zoomHUD.Visible = true;
                this.pickingService.Enabled = false;
                this.zoomHUDShown = true;
            }
            else if (this.zoomHUDShown && !this.selectorShown && !this.zoomInOutHUDRect.Contains(Convert.ToInt32(this.currentPos.X), Convert.ToInt32(this.currentPos.Y)))
            {
                this.zoomHUD.OnHUDHiddenEvent();
                this.pickingService.Enabled = true;
                this.zoomHUD.Enabled = false;
                this.zoomHUD.Visible = false;
                this.zoomHUDShown = false;
            }
        }
    }
}
