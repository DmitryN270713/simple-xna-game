using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Threading;

namespace FortDefenders
{
    public class Button<T> where T : HUD
    {
        public delegate void ButtonClicked(Object o, String tag);
        public event ButtonClicked OnButtonClicked;

        private const Single FADE_TIME = 0.4f;
        private readonly Color pressedColor = Color.DarkBlue;
        private readonly Color normalColor = Color.White;

        public Int32 Height 
        {
            get { return this.height; }
            set 
            { 
                this.height = value;
                this.btnRectangle.Height = this.height;
            }
        }

        public Int32 Width
        {
            get { return this.width; }
            set
            {
                this.width = value;
                this.btnRectangle.Width = this.width;
            }
        }

        public Vector2 Position
        {
            get { return this.pos; }
            set
            {
                this.pos = value;
                this.btnRectangle.X = Convert.ToInt32(this.pos.X);
                this.btnRectangle.Y = Convert.ToInt32(this.pos.Y);
            }
        }

        public Texture2D BtnTexture { get; set; }
        public String Tag { get; set; }

        private InputManager inputMng;
        private Vector2 currentTap;
        private Vector2 oldTap;
        private Game game;
        private SpriteBatch spriteBatch;
        private Rectangle btnRectangle;
        private Int32 height;
        private Int32 width;
        private Vector2 pos;
        private Boolean canFadeOut;
        private Boolean canPlayAnimation;              
        private Single currentTime;
        private Color currentColor;
        private T parent;

        public Button(Game game, T parent)
        {
            this.game = game;
            this.spriteBatch = new SpriteBatch(this.game.GraphicsDevice);
            this.btnRectangle = new Rectangle();
            this.inputMng = game.GetService<InputManager>();
            this.currentTap = Vector2.Zero;
            this.oldTap = Vector2.Zero;
            this.parent = parent;
            this.parent.OnHUDHidden += new HUD.HUDHidden(parent_OnHUDHidden);

            this.Initialize();
        }

        private void parent_OnHUDHidden(Object o)
        {
            this.Initialize();
        }

        private void Initialize()
        {
            this.canFadeOut = false;
            this.currentTime = FADE_TIME;
            this.currentColor = this.normalColor;
            this.canPlayAnimation = false;
        }

        public void Draw()
        {
            this.game.GraphicsDevice.RasterizerState = new RasterizerState()
            {
                FillMode = FillMode.Solid
            };

            this.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, this.game.GraphicsDevice.RasterizerState);


            this.spriteBatch.Draw(this.BtnTexture, this.btnRectangle, this.currentColor);

            this.spriteBatch.End();
        }

        public void Update()
        {
            this.currentTap = this.inputMng.GetTapPosition();
            if (this.btnRectangle.Contains(Convert.ToInt32(this.currentTap.X), 
                Convert.ToInt32(this.currentTap.Y)) && this.parent.Visible)
            {
                if (this.currentTap != this.oldTap)
                {
                    this.oldTap = this.currentTap;
                    this.OnButtonClickedEvent();
                }
            }

            if (this.canPlayAnimation)
            {
                this.PlayLerpAnimation();
            }   
        }

        private void OnButtonClickedEvent()
        {
            if (this.canPlayAnimation)
            {
                this.canPlayAnimation = false;
            }
            this.Initialize();
            this.canPlayAnimation = true;

            if (this.OnButtonClicked != null && this.currentTap == this.oldTap)
            {
                this.OnButtonClicked(this, this.Tag); 
            }
        }

        private void PlayLerpAnimation()
        {
            if (!this.canFadeOut)
            {
                Single amount = (1 - this.currentTime / FADE_TIME);
                this.currentColor = Color.Lerp(this.currentColor, pressedColor, amount);
                this.currentTime -= (this.game.TargetElapsedTime.Milliseconds / 1000.0f);
                if (this.currentTime <= 0)
                {
                    this.canFadeOut = true;
                    this.currentTime = FADE_TIME;
                }
            }
            else
            {
                Single amount = (1 - this.currentTime / FADE_TIME);
                this.currentColor = Color.Lerp(this.currentColor, normalColor, amount);
                this.currentTime -= (this.game.TargetElapsedTime.Milliseconds / 1000.0f);
                if (this.currentTime <= 0)
                {
                    this.Initialize();
                }
            }
        }
    }
}
