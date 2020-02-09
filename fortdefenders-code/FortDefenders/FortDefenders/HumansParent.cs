using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

namespace FortDefenders
{
    public sealed class HumansParent : DrawableGameComponent
    {
        private GameField gameField;
        private Matrix terrainWorld;
        private VertexPositionNormalTexture[] currentPersonVertices;
        private List<HumansBase> listOfPersons;
        private HumansBase selectedPerson;
        private MenusCaller menusCaller;
        private BuildingsParent buildingsParent;
        private ResourcesParent resourcesParent;
        private Dictionary<CellState, HumansRole> listOfPersonsToCreate;

        public Boolean PersonSelected { get; set; }
        public CellState SelectedRole { get; private set; }



        public HumansParent(Game game) : base(game)
        {
            this.listOfPersons = new List<HumansBase>();
            this.menusCaller = null;
            this.listOfPersonsToCreate = HumansParentHelper.GetList(this.CreatePersonsObject);
        }

        public void SetParent(Object parent)
        {
            this.gameField = parent as GameField;
            this.gameField.OnPersonClicked += new GameField.PersonClecked(gameField_OnPersonClicked);
            this.gameField.OnTargetForPersonClicked += new GameField.TargetForPersonClicked(gameField_OnTargetForPersonClicked);
        }

        #region Menus

        void gameField_OnTargetForPersonClicked(Object o, Point targetCell)
        {
            this.selectedPerson.SetTargetPosition(targetCell);
        }

        private void gameField_OnPersonClicked(Object o, CellState role)
        {
            if (this.PersonSelected)
            {
                var result = from target in this.listOfPersons
                             where target.Role == role
                             select target;
                this.selectedPerson = result.ElementAt(0);
                this.SelectedRole = role;

                if (this.menusCaller == null)
                {
                    this.menusCaller = this.Game.Components.OfType<MenusCaller>().ElementAt(0);
                    this.menusCaller.OnCallerButtonPressed += new MenusCaller.CallerButtonPressed(menusCaller_OnCallerButtonPressed);
                    this.menusCaller.OnPersonButtonPressed += new MenusCaller.PersonButtonPressed(menusCaller_OnPersonButtonPressed);
                }
                this.menusCaller.SetShowPersonOnBar(this.selectedPerson.FaceTex, selectedPerson.Role);
            }
            else
            {
                this.menusCaller.SetShowPersonOnBar(null, CellState.Forbidden);
                this.selectedPerson = null;
            }
        }

        private void menusCaller_OnCallerButtonPressed(Object o)
        {
            this.menusCaller.SetShowPersonOnBar(null, CellState.Forbidden);
            this.selectedPerson = null;
            this.PersonSelected = false;
        }

        private void menusCaller_OnPersonButtonPressed(object o, CellState role)
        {
            if (this.selectedPerson != null)
            {
                this.selectedPerson.ShowHumansMenu();
            }
        }

        #endregion

        public void SetTerrainWorld(ref Matrix terrainWorld)
        {
            this.terrainWorld = terrainWorld;
        }

        public void SetMatrices(ref Matrix world, ref Matrix view, ref Matrix projection)
        {
            if (this.listOfPersons.Count > 0)
            {
                foreach (HumansBase h in this.listOfPersons)
                {
                    h.SetMatrices(ref world, ref view, ref projection, ref this.terrainWorld);
                }
            }
        }

        public void SetBuildingsResourcesParent(ref BuildingsParent buildingsParent, ref ResourcesParent resourcesParent)
        {
            this.buildingsParent = buildingsParent;
            this.resourcesParent = resourcesParent;
        }

        public void InitializeCommanders()
        {
            this.InitializeHumanObject(CellState.Mayor);
            this.InitializeHumanObject(CellState.Knight);
            this.InitializeHumanObject(CellState.Archer);
            this.InitializeHumanObject(CellState.BatteringRam);
        }

        private void InitializeHumanObject(CellState role)
        {
            if (this.listOfPersonsToCreate.ContainsKey(role))
            {
                this.listOfPersonsToCreate[role].InvokeObjectCreation();

                if (role == CellState.Mayor)
                {
                    Mayor mayor = (from target in this.listOfPersons
                                   where target.Role == CellState.Mayor
                                   select target).ElementAt(0) as Mayor;
                    this.buildingsParent.SetMayorObject(ref mayor);
                    this.resourcesParent.SetMayorObject(ref mayor);
                }
            }
        }

        private void CreatePersonsObject(Type type, Int16 X, Int16 Z, CellState role)
        {
            ConstructorInfo ctor = type.GetConstructors().First();
            this.gameField.SetPersonVertices(X, Z, role, out this.currentPersonVertices);
            HumansBase obj = ctor.Invoke(new Object[] { this.Game }) as HumansBase;
            MethodInfo methodSetVertices = type.GetMethod("SetVertices");
            methodSetVertices.Invoke(obj, new Object[] { this.currentPersonVertices });
            MethodInfo methodLoadTexture = type.GetMethod("LoadTextures");
            methodLoadTexture.Invoke(obj, null);
            MethodInfo methodSetTargetPos = type.GetMethod("SetTargetPosition");
            methodSetTargetPos.Invoke(obj, new Object[] { new Point(X, Z) });
            this.listOfPersons.Add(obj);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (HumansBase h in this.listOfPersons)
            {
                h.Update(gameTime);
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (this.listOfPersons.Count > 0)
            {
                this.Game.GraphicsDevice.RasterizerState = new RasterizerState()
                {
                    FillMode = FillMode.Solid,
                    CullMode = CullMode.CullClockwiseFace
                };

                this.Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                foreach (HumansBase h in this.listOfPersons)
                {
                    h.Draw();
                }
            }
            base.Draw(gameTime);
        }

        public void GetListOfPersons(ref List<HumansBase> listOfPersons)
        {
            listOfPersons = this.listOfPersons;
        }
    }
}
