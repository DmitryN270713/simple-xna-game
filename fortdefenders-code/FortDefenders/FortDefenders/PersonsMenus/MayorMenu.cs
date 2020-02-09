using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace FortDefenders.PersonsMenus
{
    public class MayorMenu : HumansMenu
    {
        private const Int16 BTN_WIDTH = 128;
        private const Int16 BTN_HEIGHT = 32;
        private const Int16 OFFSET = 10;
        private readonly Vector2 populationOutlinePos;
        private readonly Point btnStartPos = new Point(80, 120);

        #region Variables

        private ResourcesHUD resourcesHUD;
        private Int16 currentHumansMax;
        private Int16 currentHumansNumber;
        private Texture2D assignTex;
        private Texture2D trainingTex;
        private AssignmentMayorMenu assignmentMenu;
        private TrainingMayorMenu trainingMenu;

        private Int16 freeCounter; 
        private Int16 farmersCounter;
        private Int16 minersCounter;
        private Int16 lumberjackCounter;
        private Int16 swordsmenCounter;
        private Int16 archersCounter;
        private Int16 batteringRamsCounter;
        private Int16 buildersCounter;
        private Int16 bankersCounter;
        private Int16 sellersCounter;

        private Button<MayorMenu> assignmentBtn;
        private Button<MayorMenu> trainingBtn;
        private MenusCaller menusCaller;
        private PickingService pickingService;
        private InputManager inputManager;
        private Boolean isCityhallBuilt;
        private Dictionary<WorkerRole, TypesOfWorkers> listOfRecallAddWorkersEvents;
        private Dictionary<SoldierRole, TypeOfSoldiers> listOfAddSoldiersEvents;

        #endregion


        public MayorMenu(Game game) : base(game)
        {
            this.resourcesHUD = game.Components.OfType<ResourcesHUD>().ElementAt(0);
            this.populationOutlinePos = new Vector2(game.GraphicsDevice.Viewport.Width - 300, 40);
            game.RegisterGameComponent<AssignmentMayorMenu>(out this.assignmentMenu, false, false);
            this.assignmentMenu.OnRemoveRequestSent += new AssignmentMayorMenu.RemoveRequestSent(assignmentMenu_OnRemoveRequestSent);
            this.assignmentMenu.OnAddRequestSent += new AssignmentMayorMenu.AddRequestSent(assignmentMenu_OnAddRequestSent);
            game.RegisterGameComponent<TrainingMayorMenu>(out this.trainingMenu, false, false);
            this.trainingMenu.OnAddRequestSent += new TrainingMayorMenu.AddRequestSent(trainingMenu_OnAddRequestSent);
            this.inputManager = game.GetService<InputManager>();
            this.isCityhallBuilt = false;
            this.listOfRecallAddWorkersEvents = AssignmentHelper.GetListOfEvents();
            this.listOfAddSoldiersEvents = TrainingMenuHelper.GetListOfEvents();
        }

        protected sealed override void LoadContent()
        {
            this.backgroundTexture = this.Game.Content.Load<Texture2D>("Textures/Mayor/mayor_menu_background");
            this.personRole = "Mayor";
            this.CreateMenuButtons();
            this.SetMenus();
            this.UpdateHumansInforamtion();
            this.UpdateCounters();

            base.LoadContent();
        }

        public sealed override void Update(GameTime gameTime)
        {
            this.UpdateHumansInforamtion();
            this.UpdateBtns();
            this.UpdateCounters();

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

            this.spriteBatch.DrawString(this.font, String.Format("{0}/{1}", this.currentHumansNumber, this.currentHumansMax), this.populationOutlinePos, Color.DarkBlue);

            this.spriteBatch.End();

            this.DrawButtons();
        }

        #region Class own methods

        private void CreateMenuButtons()
        {
            this.assignTex = this.Game.Content.Load<Texture2D>("Textures/Mayor/assign");
            this.trainingTex = this.Game.Content.Load<Texture2D>("Textures/Mayor/training");
            this.assignmentBtn = new Button<MayorMenu>(this.Game, this) { BtnTexture = this.assignTex, Width = BTN_WIDTH, Height = BTN_HEIGHT, 
                                                                          Position = new Vector2(this.btnStartPos.X, this.btnStartPos.Y), Tag = "ASSIGN_BTN" };
            this.assignmentBtn.OnButtonClicked += new Button<MayorMenu>.ButtonClicked(assignmentBtn_OnButtonClicked);
            this.trainingBtn = new Button<MayorMenu>(this.Game, this) { BtnTexture = this.trainingTex, Width = BTN_WIDTH, Height = BTN_HEIGHT,
                                                                        Position = new Vector2(this.btnStartPos.X, this.btnStartPos.Y + BTN_HEIGHT + OFFSET), 
                                                                        Tag = "TRAINING_BTN" };
            this.trainingBtn.OnButtonClicked += new Button<MayorMenu>.ButtonClicked(trainingBtn_OnButtonClicked);
        }

        private void trainingBtn_OnButtonClicked(Object o, String tag)
        {
            if (this.GoToNextPagePreparing())
            {
                this.trainingMenu.Enabled = true;
                this.trainingMenu.Visible = true;
            }
        }

        private void assignmentBtn_OnButtonClicked(Object o, String tag)
        {
            if (this.GoToNextPagePreparing())
            {
                this.assignmentMenu.Enabled = true;
                this.assignmentMenu.Visible = true;
            }
        }

        private Boolean GoToNextPagePreparing()
        {
            this.Enabled = false;
            this.Visible = false;
            this.OnHUDHiddenEvent();

            if (this.isCityhallBuilt)
            {
                this.pickingService.Enabled = false;
                this.menusCaller.Enabled = false;
                this.menusCaller.Visible = false;
                this.inputManager.SetTapPosition(Vector2.Zero);
                return true;
            }
            else
            {
                MessageBox msg = null;
                MessageBox.Show(this.Game, "I have no enough authority for this.\nA cityhall must be built first",
                    "Mayor said: ", MessageBoxButton.OK, this.curTex, out msg);
                return false;
            }
        }

        private void UpdateHumansInforamtion()
        {
            this.resourcesHUD.GetPopulationInfo(out this.currentHumansMax, out this.currentHumansNumber);
        }

        private void UpdateBtns()
        {
            this.assignmentBtn.Update();
            this.trainingBtn.Update();
        }

        private void DrawButtons()
        {
            this.assignmentBtn.Draw();
            this.trainingBtn.Draw();
        }

        #region Menus

        private void SetMenus()
        {
            this.assignmentMenu.EnabledChanged += new EventHandler<EventArgs>(assignmentMenu_EnabledChanged);
            this.trainingMenu.EnabledChanged += new EventHandler<EventArgs>(trainingMenu_EnabledChanged);
        }

        private void trainingMenu_EnabledChanged(Object sender, EventArgs e)
        {
            if (!trainingMenu.Enabled)
            {
                this.inputManager.SetTapPosition(Vector2.Zero);
                this.Enabled = true;
                this.Visible = true;
            }
        }

        private void assignmentMenu_EnabledChanged(Object sender, EventArgs e)
        {
            if (!assignmentMenu.Enabled)
            {
                this.inputManager.SetTapPosition(Vector2.Zero);
                this.Enabled = true;
                this.Visible = true;
            }
        }

        public void SetServicesComponents(ref PickingService pickingService, ref MenusCaller menusCaller)
        {
            this.menusCaller = menusCaller;
            this.pickingService = pickingService;
        }

        #endregion

        #region Get/Set counters

        public Int16 GetFreeCounter()
        {
            this.UpdateHumansInforamtion();
            this.UpdateCounters();
            return this.freeCounter;
        }

        private void UpdateCounters()
        {
            this.freeCounter = Convert.ToInt16(this.currentHumansNumber - this.farmersCounter - 
            this.minersCounter - this.lumberjackCounter - this.swordsmenCounter - this.archersCounter -
            this.batteringRamsCounter - this.buildersCounter - this.sellersCounter - this.bankersCounter);
        }

        #region Civilian part

        public void SetBuildersTask(Int16 neededWorkers)
        {
            this.buildersCounter += neededWorkers;
            this.freeCounter -= neededWorkers;
        }

        public void ReleaseBuilders(Int16 freeWorkers)
        {
            this.buildersCounter -= freeWorkers;
            this.freeCounter += freeWorkers;
        }

        public void SetLumberjacksTask(Int16 neededWorkers)
        {
            this.lumberjackCounter += neededWorkers;
            this.freeCounter -= neededWorkers;
        }

        public void ReleaseLumberjacks(Int16 freeWorkers)
        {
            this.lumberjackCounter -= freeWorkers;
            this.freeCounter += freeWorkers;
        }

        public void SetMinersTask(Int16 neededWorkers)
        {
            this.minersCounter += neededWorkers;
            this.freeCounter -= neededWorkers;
        }

        public void ReleaseMiners(Int16 freeWorkers)
        {
            this.minersCounter -= freeWorkers;
            this.freeCounter += freeWorkers;
        }

        public void SetFarmersTask(Int16 neededWorkers)
        {
            this.farmersCounter += neededWorkers;
            this.freeCounter -= neededWorkers;
        }

        public void ReleaseFarmers(Int16 freeWorkers)
        {
            this.farmersCounter -= freeWorkers;
            this.freeCounter += freeWorkers;
        }

        public void SetBankersTask(Int16 neededWorkers)
        {
            this.bankersCounter += neededWorkers;
            this.freeCounter -= neededWorkers;
        }

        public void ReleaseBankers(Int16 freeWorkers)
        {
            this.bankersCounter -= freeWorkers;
            this.freeCounter += freeWorkers;
        }

        public void SetSellersTask(Int16 neededWorkers)
        {
            this.sellersCounter += neededWorkers;
            this.freeCounter -= neededWorkers;
        }

        public void ReleaseSellers(Int16 freeWorkers)
        {
            this.sellersCounter -= freeWorkers;
            this.freeCounter += freeWorkers;
        }

        #endregion

        #region Military part

        public void SetSwordsmenTask(Int16 neededUnits)
        {
            this.swordsmenCounter += neededUnits;
            this.freeCounter -= neededUnits;
        }

        public void ReleaseSwordsmen(Int16 freeUnits)
        {
            this.swordsmenCounter -= freeUnits;
            this.freeCounter += freeUnits;
        }

        public void SetArchersTask(Int16 neededUnits)
        {
            this.archersCounter += neededUnits;
            this.freeCounter -= neededUnits;
        }

        public void ReleaseArchers(Int16 freeUnits)
        {
            this.archersCounter -= freeUnits;
            this.freeCounter += freeUnits;
        }

        public void SetBatteringRamsTask(Int16 neededUnits)
        {
            this.batteringRamsCounter += neededUnits;
            this.freeCounter -= neededUnits;
        }

        public void ReleaseBatteringRams(Int16 freeUnits)
        {
            this.batteringRamsCounter -= freeUnits;
            this.freeCounter += freeUnits;
        }

        #endregion

        public void LegalizeAuthority()
        {
            this.isCityhallBuilt = true;
        }

        public void HideMenus()
        {
            this.assignmentMenu.Enabled = false;
            this.assignmentMenu.Visible = false;
            this.assignmentMenu.OnHUDHiddenEvent();
            this.trainingMenu.Enabled = false;
            this.trainingMenu.Visible = false;
            this.trainingMenu.OnHUDHiddenEvent();
            this.Enabled = false;
            this.Visible = false;
            this.OnHUDHiddenEvent();
        }

        public Int16 GetBuildersCounter()
        {
            return this.buildersCounter;
        }

        public Int16 GetBankersCounter()
        {
            return this.bankersCounter;
        }

        public Int16 GetFarmersCounter()
        {
            return this.farmersCounter;
        }

        public Int16 GetLumberjacksCounter()
        {
            return this.lumberjackCounter;
        }

        public Int16 GetMinersCounter()
        {
            return this.minersCounter;
        }

        public Int16 GetSellersCounter()
        {
            return this.sellersCounter;
        }

        public Int16 GetSwordsmenCounter()
        {
            return this.swordsmenCounter;
        }

        public Int16 GetArchersCounter()
        {
            return this.archersCounter;
        }

        public Int16 GetBatteringRamCounter()
        {
            return this.batteringRamsCounter;
        }

        #endregion

        #region Menus events

        private void assignmentMenu_OnRemoveRequestSent(Object sender, WorkerRole role)
        {
            if (role == WorkerRole.Builder)
            {
                this.Enabled = false;
                this.Visible = false;
            }

            if (this.listOfRecallAddWorkersEvents.ContainsKey(role))
            {
                this.listOfRecallAddWorkersEvents[role].InvokeRecallRequest();
            }
        }

        private void assignmentMenu_OnAddRequestSent(Object sender, WorkerRole role)
        {
            if (role == WorkerRole.Builder)
            {
                this.Enabled = false;
                this.Visible = false;
            }

            if (this.listOfRecallAddWorkersEvents.ContainsKey(role))
            {
                this.listOfRecallAddWorkersEvents[role].InvokeAddRequest();
            }
        }

        private void trainingMenu_OnAddRequestSent(Object sender, SoldierRole role)
        {
            if (this.listOfAddSoldiersEvents.ContainsKey(role))
            {
                this.listOfAddSoldiersEvents[role].InvokeAddRequest();
            }
        }

        #endregion

        #endregion
    }
}
