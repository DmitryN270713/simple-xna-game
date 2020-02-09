using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FortDefenders.PersonHelperClasses;
using FortDefenders.PersonsMenus;

namespace FortDefenders
{
    public sealed class Mayor : HumansBase
    {
        #region Events

        public delegate void RecallBuilders(Object sender);
        public event RecallBuilders OnRecallBuilders;
        public delegate void AddBuilders(Object sender);
        public event AddBuilders OnAddBuilders;
        public delegate void RecallLumberjacks(Object sender);
        public event RecallLumberjacks OnRecallLumberjacks;
        public delegate void AddLumberjacks(Object sender);
        public event AddLumberjacks OnAddLumberjacks;
        public delegate void RecallMiners(Object sender);
        public event RecallMiners OnRecallMiners;
        public delegate void AddMiners(Object sender);
        public event AddMiners OnAddMiners;
        public delegate void RecallFarmers(Object sender);
        public event RecallFarmers OnRecallFarmers;
        public delegate void AddFarmers(Object sender);
        public event AddFarmers OnAddFarmers;
        public delegate void RecallBankers(Object sender);
        public event RecallBankers OnRecallBankers;
        public delegate void AddBankers(Object sender);
        public event AddBankers OnAddBankers;
        public delegate void RecallSellers(Object sender);
        public event RecallSellers OnRecallSellers;
        public delegate void AddSellers(Object sender);
        public event AddSellers OnAddSellers;
        public delegate void AddSwordmen(Object sender);
        public event AddSwordmen OnAddSwordsmen;
        public delegate void AddArchers(Object sender);
        public event AddArchers OnAddArchers;
        public delegate void AddBatteringRam(Object sender);
        public event AddBatteringRam OnAddBatteringRam;

        #endregion


        private const Single MOVING_SPEED = 0.5f;       //A half meter per second
        private const String PARENT_DIR = "Textures/Mayor";
        private const Single STEP_TIME = 0.5f;
        public const Int16 START_X = 35;
        public const Int16 START_Z = 35;
        private readonly Dictionary<PrivilegesCosts, Int16[]> calculatePayment;

        private MayorMenu mayorMenu;
        private ResourcesHUD resourcesHUD;
        public Boolean WindmillExists { get; set; }
        public Boolean BankExists { get; set; }
        public Boolean MarketExists { get; set; }
        public Boolean BarracksExists { get; set; }


        public Mayor(Game game) : base(game, CellState.Mayor, MOVING_SPEED, 
                                  new Point(START_X, START_Z), 
                                  ResourcesHUD.START_POPULATION, STEP_TIME)
        {
            this.mayorMenu = null;
            this.WindmillExists = false;
            this.BankExists = false;
            this.MarketExists = false;
            this.BarracksExists = false;
            AssignmentHelper.FillList(this.OnRecallBuildersEvent, this.OnAddBuildersEvent, this.OnRecallLumberjacksEvent, this.OnAddLumberjacksEvent,
                                      this.OnRecallMinersEvent, this.OnAddMinersEvent, this.OnRecallFarmersEvent, this.OnAddFarmersEvent,
                                      this.OnRecallBankersEvent, this.OnAddBankersEvent, this.OnRecallSellersEvent, this.OnAddSellersEvent);
            TrainingMenuHelper.FillList(this.OnAddSwordmenEvent, this.OnAddArchersEvent, this.OnAddBatteringRamEvent);
            this.resourcesHUD = game.Components.OfType<ResourcesHUD>().ElementAt(0);
            this.calculatePayment = new Dictionary<PrivilegesCosts, Int16[]>() 
            {
                { PrivilegesCosts.CommonWorker, new Int16[4] { 0, 0, 0, 1} },  //{stone, wood, gold, food}
                { PrivilegesCosts.PrivilegedWorker, new Int16[4] { 0, 0, 1, 4 } },
                { PrivilegesCosts.Soldier, new Int16[4] { 5, 1, 5, 5 } }
            };
        }

        public override void LoadTextures()
        {
            PersonTexturesLoader.LoadTextures(PARENT_DIR, this.game.Content, out this.textures);
            //this.texture[key][index of item in value's array(array of textures)]
            this.leftLegTex = this.textures[0][(Int32)PersonMovement.left];
            this.rightLegTex = this.textures[0][(Int32)PersonMovement.right];
            this.stopTex = this.textures[0][(Int32)PersonMovement.stop];
            this.FaceTex = this.game.Content.Load<Texture2D>("Textures/Mayor/face_tex");

            this.game.RegisterGameComponent<MayorMenu>(out this.mayorMenu, false, false);
            this.menu = this.mayorMenu;
            this.menu.SetParent(this);

            base.LoadTextures();

            this.mayorMenu.SetServicesComponents(ref this.pickingService, ref this.menusCaller);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw()
        {
            base.Draw();
        }

        #region Check available workers, assign tasks

        public Boolean GetFreeUnits(Int16 neededWorkers, PrivilegesCosts costsType)
        {
            Int16 availableWorkers = this.mayorMenu.GetFreeCounter();
            Int16[] payment = this.calculatePayment[costsType];
            Boolean isEnoughResources = this.ResourcesChecker(payment[0] * neededWorkers, payment[1] * neededWorkers, 
                                                              payment[2] * neededWorkers, payment[3] * neededWorkers);
            if (neededWorkers <= availableWorkers && isEnoughResources)
            {
                return true;
            }
            return false;
        }

        public void ShowMessageBox()
        {
            MessageBox msgBox = null;
            MessageBox.Show(this.game, "Not enough free units\nor not enough resources", "Mayor said: ", MessageBoxButton.OK,
                            this.FaceTex, out msgBox);
        }

        #region Civilian

        public void SendBuilders(Int16 neededWorkers)
        {
            this.mayorMenu.SetBuildersTask(neededWorkers);
        }

        public void ReleaseBuilders(Int16 freeWorkers)
        {
            this.mayorMenu.ReleaseBuilders(freeWorkers);
        }

        public void SendLumberjacks(Int16 neededWorkers)
        {
            this.mayorMenu.SetLumberjacksTask(neededWorkers);
        }

        public void ReleaseLumberjacks(Int16 freeWorkers)
        {
            this.mayorMenu.ReleaseLumberjacks(freeWorkers);
        }

        public void SendMiners(Int16 neededWorkers)
        {
            this.mayorMenu.SetMinersTask(neededWorkers);
        }

        public void ReleaseMiners(Int16 freeWorkers)
        {
            this.mayorMenu.ReleaseMiners(freeWorkers);
        }

        public void SendFarmers(Int16 neededWorkers)
        {
            this.mayorMenu.SetFarmersTask(neededWorkers);
        }

        public void ReleaseFarmers(Int16 freeWorkers)
        {
            this.mayorMenu.ReleaseFarmers(freeWorkers);
        }

        public void SendBankers(Int16 neededWorkers)
        {
            this.mayorMenu.SetBankersTask(neededWorkers);
        }

        public void ReleaseBankers(Int16 freeWorkers)
        {
            this.mayorMenu.ReleaseBankers(freeWorkers);
        }

        public void SendSellers(Int16 neededWorkers)
        {
            this.mayorMenu.SetSellersTask(neededWorkers);
        }

        public void ReleaseSellers(Int16 freeWorkers)
        {
            this.mayorMenu.ReleaseSellers(freeWorkers);
        }

        #endregion

        #region Military

        public void SendSwordsmen(Int16 neededUnits)
        {
            this.mayorMenu.SetSwordsmenTask(neededUnits);
        }

        public void ReleaseSwordsmen(Int16 freeUnits)
        {
            this.mayorMenu.ReleaseSwordsmen(freeUnits);
        }

        public void SendArchers(Int16 neededUnits)
        {
            this.mayorMenu.SetArchersTask(neededUnits);
        }

        public void ReleaseArchers(Int16 freeUnits)
        {
            this.mayorMenu.ReleaseArchers(freeUnits);
        }

        public void SendBatteringRams(Int16 neededUnits)
        {
            this.mayorMenu.SetBatteringRamsTask(neededUnits);
        }

        public void ReleaseBatteringRams(Int16 freeUnits)
        {
            this.mayorMenu.ReleaseBatteringRams(freeUnits);
        }

        #endregion

        /// <summary>
        /// Start respect mayor's authority =)
        /// </summary>
        public void LegalizeAuthority()
        {
            this.mayorMenu.LegalizeAuthority();
        }

        /// <summary>
        /// Checks resources before units hiring
        /// </summary>
        /// <param name="resources">stone, wood, gold, food</param>
        /// <returns></returns>
        private Boolean ResourcesChecker(params Int32[] resources)
        {
            if (this.resourcesHUD.SetCurrentResources(Convert.ToInt16(-resources[0]), Convert.ToInt16(-resources[1]), 
                                                      Convert.ToInt16(-resources[2]), Convert.ToInt16(-resources[3])))
            {
                return true;
            }
            return false;
        }

        public void HideMenus()
        {
            this.mayorMenu.HideMenus();
        }

        #region Events

        #region Civilian events

        private void OnRecallBuildersEvent()
        {
            if (this.OnRecallBuilders != null)
            {
                this.OnRecallBuilders(this);
            }
        }

        private void OnAddBuildersEvent()
        {
            if (this.OnAddBuilders != null)
            {
                this.OnAddBuilders(this);
            }
        }

        private void OnRecallLumberjacksEvent()
        {
            if (this.OnRecallLumberjacks != null)
            {
                this.OnRecallLumberjacks(this);
            }
        }

        private void OnAddLumberjacksEvent()
        {
            if (this.OnAddLumberjacks != null)
            {
                this.OnAddLumberjacks(this);
            }
        }

        private void OnRecallMinersEvent()
        {
            if (this.OnRecallMiners != null)
            {
                this.OnRecallMiners(this);
            }
        }

        private void OnAddMinersEvent()
        {
            if (this.OnAddMiners != null)
            {
                this.OnAddMiners(this);
            }
        }

        private void OnRecallFarmersEvent()
        {
            if (this.OnRecallFarmers != null && this.WindmillExists)
            {
                this.OnRecallFarmers(this);
            }
        }

        private void OnAddFarmersEvent()
        {
            if (this.OnAddFarmers != null && this.WindmillExists)
            {
                this.OnAddFarmers(this);
            }
        }

        private void OnRecallBankersEvent()
        {
            if (this.OnRecallBankers != null && this.BankExists)
            {
                this.OnRecallBankers(this);
            }
        }

        private void OnAddBankersEvent()
        {
            if (this.OnAddBankers != null && this.BankExists)
            {
                this.OnAddBankers(this);
            }
        }

        private void OnRecallSellersEvent()
        {
            if (this.OnRecallSellers != null && this.MarketExists)
            {
                this.OnRecallSellers(this);
            }
        }

        private void OnAddSellersEvent()
        {
            if (this.OnAddSellers != null && this.MarketExists)
            {
                this.OnAddSellers(this);
            }
        }

        #endregion

        #region Military events

        private void OnAddSwordmenEvent()
        {
            if (this.OnAddSwordsmen != null)
            {
                this.OnAddSwordsmen(this);
            }
        }

        private void OnAddArchersEvent()
        {
            if (this.OnAddArchers != null)
            {
                this.OnAddArchers(this);
            }
        }

        private void OnAddBatteringRamEvent()
        {
            if (this.OnAddBatteringRam != null)
            {
                this.OnAddBatteringRam(this);
            }
        }

        #endregion //Military events

        #endregion  //Events

        #endregion //Check available workers, assign tasks
    }
}
