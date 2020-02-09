using System;
using System.Collections.Generic;
using System.Linq;
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
    public sealed class MarketMenu : ConstructionMenu
    {
        #region Mayor's part

        //Release button's width and height
        private const Int16 BTN_WIDTH = 64;
        private const Int16 BTN_HEIGHT = 64;
        //Laying off well-known amounts of sellers
        private const Int16 RELEASE_AMOUNT = 1;     //This value should be smaller than or equal to the MAX_NUMBER_OF_PEOPLE_INSIDE constant
        private const Int16 ADD_SELLERS = 1;
        //Button for laying off workers
        private Button<MarketMenu> releaseWorkersBtn;
        private Texture2D releaseBtnTex;
        private readonly Point releaseBtnPos;
        private BuildingsParent buildingsParent;
        private Mayor mayorObject;
        private readonly Vector2 statPos = new Vector2(79, 70); //Fill free to change statistics text's position on the screen
        private Market marketObject;

        #endregion

        private readonly Int16 WIDTH = 64;
        private readonly Int16 HEIGHT = 64;

        private Texture2D CoinTex;
        private Texture2D FoodTex;
        private Texture2D WoodTex;
        private Texture2D StoneTex;

        private Button<MarketMenu> WoodCoin;
        private Button<MarketMenu> StoneCoin;
        private Button<MarketMenu> FoodCoin;
        private Button<MarketMenu> Wood;
        private Button<MarketMenu> Stone;
        private Button<MarketMenu> Food;

        //This variable will be used for the cursor's position reseting
        private InputManager inputManager;

        SoundEffect wavSoundEffectCoin;
        SoundEffectInstance wavSoundEffectInstanceCoin;

        public MarketMenu(Game game) : base(game)
        {
            this.releaseBtnPos = new Point(game.GraphicsDevice.Viewport.Width - 10 - 64, 120);  //Fill free to place this button where ever you want 
            this.buildingsParent = game.Components.OfType<BuildingsParent>().ElementAt(0);
            this.buildingsParent.GetMayorObject(ref this.mayorObject);
            this.mayorObject.MarketExists = true;
            this.mayorObject.OnRecallSellers += new Mayor.RecallSellers(mayorObject_OnRecallSellers);
            this.mayorObject.OnAddSellers += new Mayor.AddSellers(mayorObject_OnAddSellers);
            this.inputManager = game.GetService<InputManager>();
            this.EnabledChanged += new EventHandler<EventArgs>(MarketMenu_EnabledChanged);
        }

        protected sealed override void LoadContent()
        {
            this.releaseBtnTex = this.Game.Content.Load<Texture2D>("Textures/Marketplace/release_btn");     //Texture for the release button

            this.backgroundTexture = this.Game.Content.Load<Texture2D>("Textures/Marketplace/market_menu_background");
            this.CoinTex = this.Game.Content.Load<Texture2D>("Textures/Marketplace/Coin");
            this.WoodTex = this.Game.Content.Load<Texture2D>("Textures/Marketplace/Tree");
            this.FoodTex = this.Game.Content.Load<Texture2D>("Textures/Marketplace/Food");
            this.StoneTex = this.Game.Content.Load<Texture2D>("Textures/Marketplace/Stone");

            wavSoundEffectCoin = this.Game.Content.Load<SoundEffect>("audio/CoinChink");
            wavSoundEffectInstanceCoin = wavSoundEffectCoin.CreateInstance();

            this.constructionName = "Market";
            this.CreateBtns();                  //Method that is responsible for the buttons creation

            base.LoadContent();
        }

        private void WoodCoin_OnButtonClicked(Object o, String tag)
        {
            wavSoundEffectInstanceCoin.Play();
            marketObject.WoodSold();
        }

        private void StoneCoin_OnButtonClicked(Object o, String tag)
        {
            wavSoundEffectInstanceCoin.Play();
            marketObject.StoneSold();
        }

        private void FoodCoin_OnButtonClicked(Object o, String tag)
        {
            marketObject.FoodSold();
        }

        private void Wood_OnButtonClicked(Object o, String tag)
        {            
            marketObject.WoodBought();
        }

        private void Stone_OnButtonClicked(Object o, String tag)
        {
            marketObject.StoneBought();
        }

        private void Food_OnButtonClicked(Object o, String tag)
        {
            marketObject.FoodBought();
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
            this.releaseWorkersBtn = new Button<MarketMenu>(this.Game, this)
            {
                BtnTexture = this.releaseBtnTex,
                Height = BTN_HEIGHT,
                Width = BTN_WIDTH,
                Position = new Vector2(this.releaseBtnPos.X, this.releaseBtnPos.Y),
                Tag = "RELEASE"
            };
            this.releaseWorkersBtn.OnButtonClicked += new Button<MarketMenu>.ButtonClicked(releaseWorkersBtn_OnButtonClicked);

            #region Esko's buttons

            this.WoodCoin = new Button<MarketMenu>(this.Game, this)
            {
                BtnTexture = this.CoinTex,
                Height = this.HEIGHT,
                Width = this.WIDTH,
                Position = new Vector2(this.Game.GraphicsDevice.Viewport.Width / 4 + 3 * WIDTH, this.Game.GraphicsDevice.Viewport.Height - HEIGHT * 2),
                Tag = "Coin_Button"
            };

            this.FoodCoin = new Button<MarketMenu>(this.Game, this)
            {
                BtnTexture = this.CoinTex,
                Height = this.HEIGHT,
                Width = this.WIDTH,
                Position = new Vector2(this.Game.GraphicsDevice.Viewport.Width / 4 + 3 * WIDTH, this.Game.GraphicsDevice.Viewport.Height - HEIGHT * 3),
                Tag = "Coin_Button"
            };

            this.StoneCoin = new Button<MarketMenu>(this.Game, this)
            {
                BtnTexture = this.CoinTex,
                Height = this.HEIGHT,
                Width = this.WIDTH,
                Position = new Vector2(this.Game.GraphicsDevice.Viewport.Width / 4 + 3 * WIDTH, this.Game.GraphicsDevice.Viewport.Height - HEIGHT * 4),
                Tag = "Coin_Button"
            };

            this.Wood = new Button<MarketMenu>(this.Game, this)
            {
                BtnTexture = this.WoodTex,
                Height = this.HEIGHT,
                Width = this.WIDTH,
                Position = new Vector2(this.Game.GraphicsDevice.Viewport.Width / 4 - WIDTH, this.Game.GraphicsDevice.Viewport.Height - HEIGHT * 2),
                Tag = "Wood_Button"
            };

            this.Food = new Button<MarketMenu>(this.Game, this)
            {
                BtnTexture = this.FoodTex,
                Height = this.HEIGHT,
                Width = this.WIDTH,
                Position = new Vector2(this.Game.GraphicsDevice.Viewport.Width / 4 - WIDTH, this.Game.GraphicsDevice.Viewport.Height - HEIGHT * 3),
                Tag = "Food_Button"
            };

            this.Stone = new Button<MarketMenu>(this.Game, this)
            {
                BtnTexture = this.StoneTex,
                Height = this.HEIGHT,
                Width = this.WIDTH,
                Position = new Vector2(this.Game.GraphicsDevice.Viewport.Width / 4 - WIDTH, this.Game.GraphicsDevice.Viewport.Height - HEIGHT * 4),
                Tag = "Stone_Button"
            };

            this.Stone.OnButtonClicked += new Button<MarketMenu>.ButtonClicked(Stone_OnButtonClicked);
            this.Wood.OnButtonClicked += new Button<MarketMenu>.ButtonClicked(Wood_OnButtonClicked);
            this.Food.OnButtonClicked += new Button<MarketMenu>.ButtonClicked(Food_OnButtonClicked);
            this.StoneCoin.OnButtonClicked += new Button<MarketMenu>.ButtonClicked(StoneCoin_OnButtonClicked);
            this.WoodCoin.OnButtonClicked += new Button<MarketMenu>.ButtonClicked(WoodCoin_OnButtonClicked);
            this.FoodCoin.OnButtonClicked += new Button<MarketMenu>.ButtonClicked(FoodCoin_OnButtonClicked);

            #endregion
        }

        private void releaseWorkersBtn_OnButtonClicked(Object o, String tag)
        {
            if (this.marketObject.SellersCounter > 0)
            {
                this.mayorObject.ReleaseSellers(RELEASE_AMOUNT);
                this.marketObject.SellersCounter -= RELEASE_AMOUNT;
            }
        }

        private void UpdateBtns()
        {
            this.releaseWorkersBtn.Update();

            this.WoodCoin.Update();
            this.FoodCoin.Update();
            this.StoneCoin.Update();
            this.Wood.Update();
            this.Stone.Update();
            this.Food.Update();
        }

        private void DrawBtns()
        {
            this.releaseWorkersBtn.Draw();

            this.Food.Draw();
            this.FoodCoin.Draw();
            this.WoodCoin.Draw();
            this.StoneCoin.Draw();
            this.Wood.Draw();
            this.Stone.Draw();
        }

        private void DrawText()
        {
            this.spriteBatch.DrawString(this.font, this.marketObject.SellersCounter + "/" + Market.MAX_NUMBER_OF_PEOPLE_INSIDE, this.statPos, Color.DarkBlue);
        }

        private void mayorObject_OnAddSellers(Object sender)
        {
            if (this.mayorObject.GetFreeUnits(ADD_SELLERS, PrivilegesCosts.PrivilegedWorker) && this.marketObject.SellersCounter < Market.MAX_NUMBER_OF_PEOPLE_INSIDE)
            {
                this.mayorObject.SendSellers(ADD_SELLERS);
                this.marketObject.SellersCounter += ADD_SELLERS;
            }
            else
            {
                this.mayorObject.HideMenus();
                this.mayorObject.ShowMessageBox();
            }
        }

        private void mayorObject_OnRecallSellers(Object sender)
        {
            if (this.marketObject.SellersCounter > 0)
            {
                this.mayorObject.ReleaseSellers(RELEASE_AMOUNT);
                this.marketObject.SellersCounter -= RELEASE_AMOUNT;
            }
        }

        public void ConvertParent()
        {
            if (this.marketObject == null)
            {
                this.marketObject = this.parent as Market;
            }
        }
        
        private void MarketMenu_EnabledChanged(Object sender, EventArgs e)
        {
            if (this.Enabled)
            {
                this.inputManager.SetTapPosition(Vector2.Zero);
            }
        }

        #endregion
    }
}
