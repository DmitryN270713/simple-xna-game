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
    public class ZoomInOutHUD : HUD
    {
        public const Int32 RECTW = 32;
        public const Int32 RECTH = 32;

        private const Single MAX_ZOOM = 4.0f;

        private Texture2D zoomInTex;
        private Texture2D zoomOutTex;
        private CameraService cameraService;

        private Button<ZoomInOutHUD> inBtn;
        private Button<ZoomInOutHUD> outBtn;
        private Single coef;

        public ZoomInOutHUD(Game game)
            : base(game)
        {
            this.cameraService = game.GetService<CameraService>();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            this.zoomInTex = this.Game.Content.Load<Texture2D>("Textures/zoom_in");
            this.inBtn = new Button<ZoomInOutHUD>(this.Game, this) 
            {
                Height = RECTH,
                Width = RECTW,
                Position = new Vector2(this.Game.GraphicsDevice.Viewport.Width - 10 - RECTW, this.Game.GraphicsDevice.Viewport.Height - 200 - RECTH),
                Tag = "ZOOM_IN",
                BtnTexture = this.zoomInTex
            };
            this.inBtn.OnButtonClicked += new Button<ZoomInOutHUD>.ButtonClicked(inBtn_OnButtonClicked);
            
            this.zoomOutTex = this.Game.Content.Load<Texture2D>("Textures/zoom_out");
            this.outBtn = new Button<ZoomInOutHUD>(this.Game, this)
            {
                Height = RECTH,
                Width = RECTW,
                Position = new Vector2(this.Game.GraphicsDevice.Viewport.Width - 10 - RECTW, this.Game.GraphicsDevice.Viewport.Height - 190),
                Tag = "ZOOM_OUT",
                BtnTexture = this.zoomOutTex
            };
            this.outBtn.OnButtonClicked += new Button<ZoomInOutHUD>.ButtonClicked(outBtn_OnButtonClicked);

            base.LoadContent();
        }

        private void outBtn_OnButtonClicked(Object o, String tag)
        {
            if (this.coef > 1)
            {
                this.coef--;
                this.cameraService.SetCurrentZoom(this.coef);
            }
        }

        private void inBtn_OnButtonClicked(Object o, String tag)
        {
            if (this.coef < MAX_ZOOM)
            {
                this.coef++;
                this.cameraService.SetCurrentZoom(this.coef);
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            this.inBtn.Update();
            this.outBtn.Update();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            this.inBtn.Draw();
            this.outBtn.Draw();

            base.Draw(gameTime);
        }
    }
}
