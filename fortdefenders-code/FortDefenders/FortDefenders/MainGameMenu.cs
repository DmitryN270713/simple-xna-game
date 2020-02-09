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
    public class MainGameMenu : HUD
    {
        private const Int32 BTN_WIDTH = 256;
        private const Int32 BTN_HEIGHT = 64;
        private const Int32 OFFSET = 20;

        private Button<MainGameMenu> startBtn;
        private Button<MainGameMenu> exitBtn;
        private Texture2D startBtnTex;
        private Texture2D exitBtnTex;
        private Texture2D backgroundTex;
        private Rectangle backgroundRect;
        private readonly Vector2 startPos = new Vector2(150, 106);
        private SpriteBatch spriteBatch;

        #region Services and Components to enable

        private PickingService pickingService;
        private CameraService cameraService;
        private GameScreen gameScreen;
        private GameField gameField;
        private HumansParent humansParent;
        private BuildingsParent buildingsParent;
        private ResourcesParent resourcesParent;
        private MenusCaller menuCaller;
        private ResourcesHUD resourcesHUD;

        #endregion

        public MainGameMenu(Game game) : base(game)
        {
            this.backgroundRect = new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);  
        }

        public override void Initialize()
        {
            this.EnabledChanged += new EventHandler<EventArgs>(MainGameMenu_EnabledChanged);
            base.Initialize();
        }

        private void MainGameMenu_EnabledChanged(object sender, EventArgs e)
        {
            if (!this.Enabled)
            {
                this.OnHUDHiddenEvent();
            }
        }

        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);

            this.GetComponentsServicesToEnable();
            this.LoadTextures();
            this.CreateBtns();

            base.LoadContent();
        }

        /// <summary>
        /// Retrievs all services and components needed to start the game
        /// </summary>
        private void GetComponentsServicesToEnable()
        {
            this.cameraService = this.Game.GetService<CameraService>();
            this.pickingService = this.Game.GetService<PickingService>();
            this.gameScreen = this.Game.Components.OfType<GameScreen>().ElementAt(0);
            this.gameField = this.Game.Components.OfType<GameField>().ElementAt(0);
            this.humansParent = this.Game.Components.OfType<HumansParent>().ElementAt(0);
            this.buildingsParent = this.Game.Components.OfType<BuildingsParent>().ElementAt(0);
            this.resourcesParent = this.Game.Components.OfType<ResourcesParent>().ElementAt(0);
            this.menuCaller = this.Game.Components.OfType<MenusCaller>().ElementAt(0);
            this.resourcesHUD = this.Game.Components.OfType<ResourcesHUD>().ElementAt(0);
        }

        /// <summary>
        /// Activates needed services and components
        /// </summary>
        private void ActivateServicesComponents()
        {
            this.cameraService.Enabled = true;
            this.pickingService.Enabled = true;
            this.gameScreen.Enabled = true;
            this.gameScreen.Visible = true;
            this.gameField.Enabled = true;
            this.gameField.Visible = true;
            this.humansParent.Enabled = true;
            this.humansParent.Visible = true;
            this.buildingsParent.Enabled = true;
            this.buildingsParent.Visible = true;
            this.resourcesParent.Enabled = true;
            this.resourcesParent.Visible = true;
            this.menuCaller.Enabled = true;
            this.menuCaller.Visible = true;
            this.resourcesHUD.Enabled = true;
            this.resourcesHUD.Visible = true;
        }

        /// <summary>
        /// Loads textures to draw
        /// </summary>
        private void LoadTextures()
        {
            this.backgroundTex = this.Game.Content.Load<Texture2D>("Textures/MainMenu/background");
            this.startBtnTex = this.Game.Content.Load<Texture2D>("Textures/MainMenu/start");
            this.exitBtnTex = this.Game.Content.Load<Texture2D>("Textures/MainMenu/exit");
        }

        /// <summary>
        /// Creates buttons. Must be called after the LoadTextures() -method
        /// </summary>
        private void CreateBtns()
        {
            this.startBtn = new Button<MainGameMenu>(this.Game, this)
            {
                BtnTexture = startBtnTex,
                Height = BTN_HEIGHT,
                Width = BTN_WIDTH,
                Position = this.startPos,
                Tag = "START_GAME"
            };

            this.exitBtn = new Button<MainGameMenu>(this.Game, this)
            {
                BtnTexture = exitBtnTex,
                Height = BTN_HEIGHT,
                Width = BTN_WIDTH,
                Position = new Vector2(this.startPos.X, this.startPos.Y + OFFSET + BTN_HEIGHT),
                Tag = "EXIT_GAME"
            };

            this.startBtn.OnButtonClicked += new Button<MainGameMenu>.ButtonClicked(startBtn_OnButtonClicked);
            this.exitBtn.OnButtonClicked += new Button<MainGameMenu>.ButtonClicked(exitBtn_OnButtonClicked);
        }

        private void exitBtn_OnButtonClicked(Object o, String tag)
        {
            this.Game.Exit();
        }

        private void startBtn_OnButtonClicked(Object o, String tag)
        {
            this.Enabled = false;
            this.Visible = false;
            this.ActivateServicesComponents();
        }

        public override void Update(GameTime gameTime)
        {
            this.startBtn.Update();
            this.exitBtn.Update();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.RasterizerState = new RasterizerState()
            {
                FillMode = FillMode.Solid
            };

            this.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, GraphicsDevice.RasterizerState);

            this.spriteBatch.Draw(this.backgroundTex, this.backgroundRect, Color.White);

            this.spriteBatch.End();

            this.startBtn.Draw();
            this.exitBtn.Draw();
            
            base.Draw(gameTime);
        }
    }
}
