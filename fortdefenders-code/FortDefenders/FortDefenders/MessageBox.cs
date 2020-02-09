using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace FortDefenders
{
    public enum MessageBoxResult
    { 
        OK = 0,
        Cancel = 1
    }

    public class MessageBox : HUD
    {
        public delegate void ExposeResult(Object o, MessageBoxResult result);
        public event ExposeResult OnResult;
        public delegate void CloseMessageBox(Object sender);
        public event CloseMessageBox OnClose;

        private const Int16 WIDTH = 128;
        private const Int16 HEIGHT = 32;
        private const Int16 ICON_WIDTH = 64;
        private const Int16 ICON_HEIGHT = 64;
        private readonly Rectangle backgroundRect;
        private readonly Rectangle iconRect;
        private readonly Vector2 titlePos;
        private readonly Vector2 txtPos;

        private static MessageBox msgBox = null;
        public String Message { get; set; }
        public String Title { get; set; }
        public Texture2D Icon { get; set; }
        private List<Button<MessageBox>> lsBtns;
        private PickingService pickingService;
        private MessageBoxButton btnType;
        private Texture2D backgroundTex;
        private Texture2D okBtnTex;
        private Texture2D cancelBtnTex;
        private SpriteBatch spriteBatch;
        private SpriteFont fontTxt;
        private SpriteFont fontTitle;
        private MenusCaller buildingsMenuCaller;


        public MessageBoxButton Buttons 
        {
            get { return this.btnType; }
            set 
            { 
                this.btnType = value;
                if (this.okBtnTex != null && this.cancelBtnTex != null && this.backgroundTex != null)
                {
                    this.lsBtns.Clear();
                    this.CreateBtns();
                }
            }
        }


        private MessageBox(Game game) : base(game)
        {
            try
            {
                this.pickingService = game.GetService<PickingService>();
                this.pickingService.Enabled = false;
                this.buildingsMenuCaller = game.Components.OfType<MenusCaller>().ElementAt(0);
                this.buildingsMenuCaller.Enabled = false;
                this.buildingsMenuCaller.Visible = false;
            }
            catch (Exception)
            { }

            this.lsBtns = new List<Button<MessageBox>>();
            this.backgroundRect = new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);
            this.iconRect = new Rectangle(10, ICON_HEIGHT, ICON_WIDTH, ICON_HEIGHT);
            this.titlePos = new Vector2(25 + ICON_WIDTH, ICON_HEIGHT);
            this.txtPos = new Vector2(10, 2 * ICON_HEIGHT + 40);
        }

        /// <summary>
        /// To recieve a result, subscribe to the OnResult -event. Don't forget to unsubscribe from the event to avoid unpleasent surprises.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="msg"></param>
        /// <param name="title"></param>
        /// <param name="btn"></param>
        /// <param name="icon"></param>
        /// <param name="msgBoxObj"></param>
        public static void Show(Game game, String msg, String title, MessageBoxButton btn, Texture2D icon, out MessageBox msgBoxObj)
        {
            if (msgBox == null)
            {
                msgBox = new MessageBox(game)
                {
                    Message = msg,
                    Title = title,
                    Buttons = btn,
                    Icon = icon
                };
                msgBoxObj = msgBox;

                msgBox.Enabled = true;
                msgBox.Visible = true;
                game.Components.Add(msgBox);
            }
            else
            {
                msgBoxObj = msgBox;

                msgBox.Message = msg;
                msgBox.Title = title;
                msgBox.Buttons = btn;
                msgBox.Icon = icon;

                msgBox.Enabled = true;
                msgBox.Visible = true;
            }
        }

        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(this.Game.GraphicsDevice);

            this.okBtnTex = this.Game.Content.Load<Texture2D>("Textures/MessageBox/okBtn");
            this.cancelBtnTex = this.Game.Content.Load<Texture2D>("Textures/MessageBox/cancelBtn");
            this.backgroundTex = this.Game.Content.Load<Texture2D>("Textures/MessageBox/background");
            this.fontTitle = this.Game.Content.Load<SpriteFont>("Fonts/Castellar");
            this.fontTxt = this.Game.Content.Load<SpriteFont>("Fonts/Castellar_HUD");

            this.CreateBtns();

            this.EnabledChanged += new EventHandler<EventArgs>(MessageBox_EnabledChanged);
            base.LoadContent();
        }

        private void MessageBox_EnabledChanged(Object sender, EventArgs e)
        {
            if (this.pickingService != null)
            {
                this.pickingService.Enabled = (this.Enabled ? false : true);
                this.buildingsMenuCaller.Enabled = (this.Enabled ? false : true);
                this.buildingsMenuCaller.Visible = (this.Enabled ? false : true);
                this.OnCloseEvent();
            }
        }

        private void CreateBtns()
        {
            switch (this.btnType)
            { 
                case MessageBoxButton.OK:
                    this.CreateOneBtn();
                    break;
                case MessageBoxButton.OKCancel:
                    this.CreateTwoBtn();
                    break;
                default:
                    break;
            }
        }

        private void CreateOneBtn()
        {
            Button<MessageBox> okBtn = new Button<MessageBox>(this.Game, this)
            {
                BtnTexture = this.okBtnTex,
                Height = HEIGHT,
                Width = WIDTH,
                Tag = "OK_BTN",
                Position = new Vector2(this.Game.GraphicsDevice.Viewport.Width - 10 - WIDTH, this.Game.GraphicsDevice.Viewport.Height - 10 - HEIGHT)
            };
            okBtn.OnButtonClicked += new Button<MessageBox>.ButtonClicked(okBtn_OnButtonClicked);
            this.lsBtns.Add(okBtn);
        }

        private void okBtn_OnButtonClicked(Object o, String tag)
        {
            this.OnHUDHiddenEvent();
            this.OnResultEvent(MessageBoxResult.OK);
            this.Enabled = false;
            this.Visible = false;
        }

        private void CreateTwoBtn()
        {
            Button<MessageBox> okBtn = new Button<MessageBox>(this.Game, this)
            {
                BtnTexture = this.okBtnTex,
                Height = HEIGHT,
                Width = WIDTH,
                Tag = "OK_BTN",
                Position = new Vector2(this.Game.GraphicsDevice.Viewport.Width - 20 - 2 * WIDTH, this.Game.GraphicsDevice.Viewport.Height - 10 - HEIGHT)
            };

            Button<MessageBox> Cancel = new Button<MessageBox>(this.Game, this)
            {
                BtnTexture = this.cancelBtnTex,
                Height = HEIGHT,
                Width = WIDTH,
                Tag = "CANCEL_BTN",
                Position = new Vector2(this.Game.GraphicsDevice.Viewport.Width - 10 - WIDTH, this.Game.GraphicsDevice.Viewport.Height - 10 - HEIGHT)
            };

            okBtn.OnButtonClicked += new Button<MessageBox>.ButtonClicked(okBtn_OnButtonClicked);
            Cancel.OnButtonClicked += new Button<MessageBox>.ButtonClicked(Cancel_OnButtonClicked);
            this.lsBtns.Add(okBtn);
            this.lsBtns.Add(Cancel);
        }

        private void Cancel_OnButtonClicked(Object o, String tag)
        {
            this.OnHUDHiddenEvent();
            this.OnResultEvent(MessageBoxResult.Cancel);
            this.Enabled = false;
            this.Visible = false;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (Button<MessageBox> btn in this.lsBtns)
            {
                btn.Update();    
            }

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
            this.spriteBatch.Draw(this.Icon, this.iconRect, Color.White);
            this.spriteBatch.DrawString(this.fontTitle, this.Title, this.titlePos, Color.DarkBlue);
            this.spriteBatch.DrawString(this.fontTxt, this.Message, this.txtPos, Color.DarkBlue);

            this.spriteBatch.End();

            foreach (Button<MessageBox> btn in this.lsBtns)
            {
                btn.Draw();
            }

            base.Draw(gameTime);
        }

        private void OnResultEvent(MessageBoxResult result)
        {
            if (this.OnResult != null)
            {
                this.OnResult(this, result);
            }
        }

        private void OnCloseEvent()
        {
            if (this.OnClose != null)
            {
                this.OnClose(this);
            }
        }
    }
}
